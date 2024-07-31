
using NSteam.NetworkDictionary;
using Steamworks;
using System.Collections.Generic;

# nullable enable 
namespace NSteamLib.Networking.Socket
{
    public static class NSteamSocket
    {
        static List<HSteamNetConnection> Clients = new List<HSteamNetConnection>();
        public static List<HSteamNetConnection> ClientsData => Clients;
        static int maxClients = int.MaxValue;
        public static int MaxClients => maxClients;
        static int gameport = 7777;
        public static int Port => gameport;

        static HSteamListenSocket listenSocket = HSteamListenSocket.Invalid;
        public static HSteamListenSocket ListenSocket => listenSocket;

        static HSteamNetConnection connectionSocket = HSteamNetConnection.Invalid;
        public static HSteamNetConnection ConnectionSocket => connectionSocket;


        public static bool IsInSession => (connectionSocket != HSteamNetConnection.Invalid || listenSocket != HSteamListenSocket.Invalid);
        public static bool IsSessionHost => listenSocket != HSteamListenSocket.Invalid;


        public static Callback<SteamNetConnectionStatusChangedCallback_t>? OnNetConnectionStatusChanged;

        public static void Init()
        {
            NSteamSocketEvents.Init();
            NSteamSocketMessage.Init();
            OnNetConnectionStatusChanged = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(NetConnectionStatusChanged);
        }
        public static void Shutdown()
        {
            NSteamSocketEvents.Shutdown();
            NSteamSocketMessage.Shutdown();
            OnNetConnectionStatusChanged = null;
        }


        public static bool CreateListenSocketP2P(int _maxClients = 4,int port = 7777, SteamNetworkingConfigValue_t[]? pOptions = null)
        {
            maxClients = maxClients < 2 ? 2 : maxClients;
            if (!IsInSession)
            {
                int optioncount = pOptions == null ? 0 : pOptions.Length;
                //NSteam.logger?.Log("Creating P2P Socket");
                listenSocket = SteamNetworkingSockets.CreateListenSocketP2P(port, optioncount, pOptions);
                if (IsInSession)
                {
                    //NSteam.logger?.Log("P2P Listen Socket Created");
                    maxClients = _maxClients;
                    gameport = port;
                    return true;
                }
               // NSteam.logger?.Log("P2P Listen not Created");
            }
            else
            {
               // NSteam.logger?.Log("You are in P2P Session");
            }

            return false;
        }
    
        public static bool ConnectP2P(CSteamID host, int port = 7777, SteamNetworkingConfigValue_t[]? pOptions = null)
        {
            if (!IsInSession)
            {
                int optioncount = pOptions == null ? 0 : pOptions.Length;
                SteamNetworkingIdentity hostIdentity = new SteamNetworkingIdentity();
                hostIdentity.SetSteamID(host);
                connectionSocket = SteamNetworkingSockets.ConnectP2P(ref hostIdentity, port, optioncount, pOptions);
                if (IsInSession)
                {
                    gameport = port;
                    //NSteam.logger?.Log("Joined to P2P successfully");

                    return true;
                }
               // NSteam.logger?.Log("Couldn't join to server");

            }
            else
            {
                //NSteam.logger?.Log("You are in P2P Session");
            }

            return false;
        }

        public static bool AcceptConnection(HSteamNetConnection h_conn)
        {
            EResult result = SteamNetworkingSockets.AcceptConnection(h_conn);
            if (result == EResult.k_EResultInvalidParam)
            {
                return false;
            }
            else if (result == EResult.k_EResultInvalidState)
            {
                return false;
            }
            return true;
        }


        public static bool CloseListenSocket()
        {
            if (IsInSession && IsSessionHost)
            {
                if (SteamNetworkingSockets.CloseListenSocket(listenSocket))
                {
                    ClearConnections();
                    maxClients = int.MaxValue;
                    return true;
                }
            }
            return false;
        }

        static void ClearConnections(ESteamNetConnectionEnd EndRes = ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Remote_Timeout, string pszRes = "Timed Out",bool linger = true)
        {
            foreach (HSteamNetConnection connection in ClientsData)
            {
                SteamNetworkingSockets.FlushMessagesOnConnection(connection);
                Clients.Remove(connection);
            }
        }

        public static bool CloseConnectedSocket( ESteamNetConnectionEnd connectionEnd, string pszDebug, bool linger = true)
        {
            if (IsInSession && !IsSessionHost)
            {

                if (SteamNetworkingSockets.CloseConnection(connectionSocket, (int)connectionEnd, pszDebug, linger))
                {
                    ClearConnections();
                    maxClients = int.MaxValue;
                    connectionSocket = HSteamNetConnection.Invalid;
                    return true;
                }
            }
            return false;
        }

        private static void NetConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t param)
        {
           // NSteam.logger?.Log($"{param.m_hConn} : {param.m_eOldState} to {param.m_info.m_eState}");
            CSteamID steamID = param.m_info.m_identityRemote.GetSteamID();
            bool IsSelfIdentity = param.m_hConn == connectionSocket;

            switch (param.m_info.m_eState)
            {
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None:
                    if (param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer ||
                        param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
                    {
                        if (IsSelfIdentity)
                        {
                            connectionSocket = HSteamNetConnection.Invalid;
                            listenSocket = HSteamListenSocket.Invalid;
                        }
                    }
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
                    NSteamSocketEvents.OnSteamNetworkingConnecting(param);
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FindingRoute:
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                    if (param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting ||
                        param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FindingRoute)
                    {
                        if (Clients.Count <= MaxClients)
                        {
                            if (IsSelfIdentity)
                            {
                                AddClient(connectionSocket);
                            }
                            else
                            {
                               // NSteam.logger?.Log($"{SteamFriends.GetFriendPersonaName(steamID)} Connected");
                                AddClient(param.m_hConn);
                                if (IsSessionHost)
                                {
                                    foreach (var item in Clients)
                                    {
                                        byte[] data = NetDicionary.Serialize("AddMe", item.ToString());
                                        NSteamSocketMessage.SendMessageToall(data, NSteamSocketMessage.SendType.k_nSteamNetworkingSend_Reliable);
                                    }
                                }
                            }
                            NSteamSocketEvents.OnPlayerConnected(param);
                        }
                    }
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
                    if (IsSelfIdentity) { connectionSocket = HSteamNetConnection.Invalid; listenSocket = HSteamListenSocket.Invalid; };
                    //NSteam.logger?.Log($"{SteamFriends.GetFriendPersonaName(steamID)} Disconnected");

                    if (param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting ||
                        param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
                    {
                        SteamNetworkingSockets.CloseConnection(param.m_hConn, param.m_info.m_eEndReason, param.m_info.m_szConnectionDescription,true);
                    }
                    if (param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
                    {
                        Clients.Remove(param.m_hConn);
                    }
                    NSteamSocketEvents.OnPlayerDisconnected(param, (ESteamNetConnectionEnd)param.m_info.m_eEndReason);
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                    if (IsSelfIdentity) { connectionSocket = HSteamNetConnection.Invalid; listenSocket = HSteamListenSocket.Invalid; };
                    //NSteam.logger?.Log($"{SteamFriends.GetFriendPersonaName(steamID)} Disconnected");

                    if (param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting ||
                        param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
                    {
                        SteamNetworkingSockets.CloseConnection(param.m_hConn, param.m_info.m_eEndReason, param.m_info.m_szConnectionDescription, true);
                    }
                    if (param.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
                    {
                        Clients.Remove(param.m_hConn);
                    }

                    NSteamSocketEvents.OnPlayerDisconnected(param, (ESteamNetConnectionEnd)param.m_info.m_eEndReason);
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FinWait:
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Linger:
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Dead:
                    break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState__Force32Bit:
                    break;
                default:
                    break;
            }

        }
        public static void AddClient(HSteamNetConnection conn)
        {
            if (Clients.Count <= MaxClients)
            {
                if (!Clients.Contains(conn))
                {
                    Clients.Add(conn);
                }
            }
        }
    }

}
