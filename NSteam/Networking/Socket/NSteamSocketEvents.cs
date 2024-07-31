using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSteamLib.Networking.Socket
{
    public static class NSteamSocketEvents
    {

        public enum NDisconnectRes
        {
            None,
            ClosedByRemoteHost,
            TimeOut,
        }

        public delegate void SteamNetworkingConnecting(SteamNetConnectionStatusChangedCallback_t param);
        public static event SteamNetworkingConnecting onSteamNetworkingConnecting = (a) => { };

        public delegate void PlayerConnected(SteamNetConnectionStatusChangedCallback_t param);
        public static event PlayerConnected onPlayerConnected = (a) => { };

        public delegate void PlayerDisconnected(SteamNetConnectionStatusChangedCallback_t param, ESteamNetConnectionEnd result);
        public static event PlayerDisconnected onPlayerDisconnected = (a,b) => { };

        public static void Init()
        {

        }

        public static void Shutdown()
        {

        }

        public static void OnSteamNetworkingConnecting(SteamNetConnectionStatusChangedCallback_t param) { onSteamNetworkingConnecting(param); }
        public static void OnPlayerConnected(SteamNetConnectionStatusChangedCallback_t param) { onPlayerConnected(param); } 
        public static void OnPlayerDisconnected(SteamNetConnectionStatusChangedCallback_t param, ESteamNetConnectionEnd result) { onPlayerDisconnected(param, result); }
    }
}
