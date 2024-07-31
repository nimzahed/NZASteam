using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSteamLib.Lobby.Message
{
    public static class NMessageEvents
    {
        public delegate void MessageRecieved(ulong sender, ulong lobby, byte[] buffer);
        public static event MessageRecieved onMessageRecieved = (a, b, c) => { };

        public static void OnMessageRecieved(ulong sender, ulong lobby, byte[] buffer)
        {
            onMessageRecieved(sender, lobby, buffer);
        }
    }
}
