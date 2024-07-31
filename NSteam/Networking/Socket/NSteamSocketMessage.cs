
using NSteam.NetworkDictionary;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace NSteamLib.Networking.Socket
{
    public static class NSteamSocketMessage
    {

        public delegate void MessageReceive(byte[] message, HSteamNetConnection from);
        public static event MessageReceive onMessageReceive = (a,b) => { };

        static int maxMessagesOnConnection = 512;

        public static void Init(int _maxMessagesOnConnection = 512)
        {
            maxMessagesOnConnection = _maxMessagesOnConnection < 10 ? 10 : _maxMessagesOnConnection;
        }
        public static void Shutdown()
        {
            maxMessagesOnConnection = 512;
        }



        public enum SendType
        {
            k_nSteamNetworkingSend_Unreliable = 0,
            k_nSteamNetworkingSend_Reliable,
            k_nSteamNetworkingSend_NoNagle,
            k_nSteamNetworkingSend_NoDelay
        }

        public static void SendMessageToall(byte[] message, SendType type = 0)
        {
            foreach (var item in NSteamSocket.ClientsData)
            {
                if (item != HSteamNetConnection.Invalid)
                {
                    SendMessage(item, message, type);
                }
            }
        }

        public static void SendMessage(HSteamNetConnection conn, byte[] message, SendType type = 0, bool flushFast = false)
        {
            if (NSteamSocket.IsInSession && conn != HSteamNetConnection.Invalid)
            {
                IntPtr buffer = Marshal.AllocHGlobal(message.Length); // Allocate unmanaged memory for the byte array
                Marshal.Copy(message, 0, buffer, message.Length); // Copy the byte array to the unmanaged memory

                SteamNetworkingSockets.SendMessageToConnection(conn, buffer, (uint)message.Length, (int)type, out _);

                if (flushFast)
                {
                    SteamNetworkingSockets.FlushMessagesOnConnection(conn);
                }
                Marshal.FreeHGlobal(buffer);

            }
        }


        public static void ReceiveMessagesOnConnection(HSteamNetConnection conn)
        {

            IntPtr[] messagePtrArray = new IntPtr[maxMessagesOnConnection];
            int msgCount = SteamNetworkingSockets.ReceiveMessagesOnConnection(conn, messagePtrArray, maxMessagesOnConnection);

            if (msgCount <= 0)
                return;

            for (int i = 0; i < msgCount; i++)
            {
                // Convert the pointer to a SteamNetworkingMessage_t object
                SteamNetworkingMessage_t netMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messagePtrArray[i]);

                // Convert the message data from IntPtr to byte[]
                byte[] messageData = new byte[netMessage.m_cbSize];
                Marshal.Copy(netMessage.m_pData, messageData, 0, netMessage.m_cbSize);
                string[] datas = NetDicionary.DeSerialize(messageData);
                if (datas[0] == "AddMe")
                {
                    try
                    {
                        HSteamNetConnection newConn = new HSteamNetConnection(uint.Parse(datas[1]));
                        newConn.m_HSteamNetConnection = uint.Parse(datas[1]);
                        NSteamSocket.AddClient(newConn);
                    }
                    catch (Exception) { };
                }
                else
                {
                    onMessageReceive(messageData, conn);
                }

                // Handle the message (for example, by printing it)
                // Release the message back to the pool
                SteamNetworkingMessage_t.Release(messagePtrArray[i]);

            }
        }

        public static void ReceiveMessagesFromAll()
        {
            for (int i = 0; i < NSteamSocket.ClientsData.Count; i++)
            {
                if (NSteamSocket.ClientsData.Count >= i)
                {
                    ReceiveMessagesOnConnection(NSteamSocket.ClientsData[i]);
                }
            }
        }
    }
}
