using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSteamLib.Lobby
{
    public static class NLobbyEvents
    {

        public delegate void LobbyCreated(ulong id);
        public static event LobbyCreated onLobbyCreated = (a) => { };

        public delegate void LobbyLeaved(ulong id, string resion);
        public static event LobbyLeaved onLobbyLeaved = (a, b) => { };

        public delegate void LobbysRefreshed(List<NLobbyData> data);
        public static event LobbysRefreshed onLobbysRefreshed = (a) => { };

        public delegate void LobbyEntered(ulong lobby);
        public static event LobbyEntered onLobbyEntered = (a) => { };

        public delegate void MemberEntered(ulong member, ulong lobby);
        public static event MemberEntered onMemberEntered = (a,b) => { };

        public delegate void LobbyGameServerSetted(ulong lobby, ulong session, uint ip, ushort port);
        public static event LobbyGameServerSetted onLobbyGameServerSetted = ( lobby, session, ip, port) => { };

        public static void OnLobbyCreated(ulong lobby) { onLobbyCreated(lobby); }
        public static void OnLobbysRefreshed(List<NLobbyData> data) { onLobbysRefreshed(data); }
        public static void OnLobbyEntered(ulong lobby) { onLobbyEntered(lobby); }
        public static void OnLobbyLeaved(ulong id, string resion) { onLobbyLeaved(id, resion); }
        public static void OnMemberEntered(ulong member, ulong lobby) { onMemberEntered(member, lobby); }
        public static void OnLobbyGameServerSetted(ulong lobby, ulong session, uint ip, ushort port) { onLobbyGameServerSetted(lobby, session, ip, port); }
    }
}
