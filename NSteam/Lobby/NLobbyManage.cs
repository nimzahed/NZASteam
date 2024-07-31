using NSteamLib.Networking.Socket;
using NSteamLib.Users;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;


# nullable enable 
namespace NSteamLib.Lobby
{
    public static class NLobbyManage
    {

        public static Callback<LobbyDataUpdate_t>? LobbyDataUpdate;

        public static Callback<LobbyGameCreated_t>? LobbyGameCreate;

        public delegate void LobbyDataUpdated(ulong lobby, ulong changed, bool isLobby);
        public static event LobbyDataUpdated onLobbyDataUpdated = (a,b,c) => { };

        public static void Init()
        {
            LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(LobbyDataUpdatedResult);
            LobbyGameCreate = Callback<LobbyGameCreated_t>.Create(LobbyGameCreatedResult);
        }
        public static void Shutdown()
        {
            LobbyDataUpdate = null;
            LobbyGameCreate = null;
        }


        public static bool IsLeader(ulong user = 0)
        {
            bool leader = false;
            if (NLobby.LobbyID != 0)
            {
                if (user == 0)
                {
                    leader = SteamMatchmaking.GetLobbyOwner(new CSteamID(NLobby.LobbyID)) == SteamUser.GetSteamID();
                }
                else
                {
                    leader = SteamMatchmaking.GetLobbyOwner(new CSteamID(NLobby.LobbyID)) == new CSteamID(user);
                }
            }
            return leader;

        }
        public static bool SetLobbyData(string key, string value)
        {
            CSteamID lobby = new CSteamID(NLobby.LobbyID);
            return SteamMatchmaking.SetLobbyData(lobby, key, value);

        }

        public static bool SetLobbyJoinable(bool joinable)
        {
            if (NLobby.IsInLobby)
            {

                return SteamMatchmaking.SetLobbyJoinable(NLobby.LobbySteamId, joinable);
            }

            return false;
        }

        public static bool SetLobbyMemberLimit(int max)
        {
            max = max < 2 ? 2 : max;
            max = max > 250 ? 250: max;
            if (NLobby.IsInLobby)
            {
                return SteamMatchmaking.SetLobbyMemberLimit(NLobby.LobbySteamId, max);
            }
            return false;
        }

        public static bool SetLobbyType(ELobbyType type)
        {
            if (NLobby.IsInLobby)
            {
                return SteamMatchmaking.SetLobbyType(NLobby.LobbySteamId, type);
            }
            return false;
        }

        public static string GetLobbyData(string key, CSteamID lobby = new CSteamID())
        {
            string val = "";
            if (lobby == new CSteamID())
            {
                val = SteamMatchmaking.GetLobbyData(new CSteamID(NLobby.LobbyID), key);
            }
            else
            {
                val = SteamMatchmaking.GetLobbyData(lobby, key);
            }
            return val;
        }

        public static void SetMemberData(string key, string value)
        {
            if (NLobby.LobbyID != 0)
            {
                SteamMatchmaking.SetLobbyMemberData(new CSteamID(NLobby.LobbyID), key, value);

            }
        }

        public static string GetMemberData(string key, CSteamID member = new CSteamID())
        {
            string str = "";
            if (NLobby.LobbyID != 0)
            {
                if (member == new CSteamID())
                {
                    str = SteamMatchmaking.GetLobbyMemberData(new CSteamID(NLobby.LobbyID), SteamUser.GetSteamID(), key); ;

                }
                else
                {
                    str = SteamMatchmaking.GetLobbyMemberData(new CSteamID(NLobby.LobbyID), member, key);

                }
            }
            return str;
        }

        public static bool SetOwner(CSteamID newOwner)
        {
            return SteamMatchmaking.SetLobbyOwner(new CSteamID(NLobby.LobbyID), newOwner);

        }
        public static bool Invite(CSteamID steamID)
        {
            return SteamMatchmaking.InviteUserToLobby(new CSteamID(NLobby.LobbyID), steamID);
        }


        private static void LobbyDataUpdatedResult(LobbyDataUpdate_t param)
        {

            EResult res = (EResult)param.m_bSuccess;
            if (res == EResult.k_EResultOK)
            {
                ulong lobby = param.m_ulSteamIDLobby;
                ulong changed = param.m_ulSteamIDMember;
                onLobbyDataUpdated(lobby, changed, lobby == changed);
            }
        }


        private static void LobbyGameCreatedResult(LobbyGameCreated_t param)
        {
            NLobbyEvents.OnLobbyGameServerSetted(param.m_ulSteamIDLobby, param.m_ulSteamIDGameServer, param.m_unIP, param.m_usPort);
        }

        public static List<NUserData?> GetLobbyMembers()
        {
            List<NUserData?> nUserDatas = new List<NUserData?>();
            if (NLobby.IsInLobby)
            {
                int membersnum =  SteamMatchmaking.GetNumLobbyMembers(NLobby.LobbySteamId);
                for (int i = 0; i < membersnum; i++)
                {
                    CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(NLobby.LobbySteamId,i);
                    nUserDatas.Add(NSteamUsers.GetUserData((ulong)member));
                }
            }
            return nUserDatas;
        }

        public static bool SetLobbyGameServer()
        {
            if (NLobby.IsInLobby && IsLeader() && NSteamSocket.IsSessionHost)
            {
                SteamMatchmaking.SetLobbyGameServer(NLobby.LobbySteamId, 0x7f000001, (ushort)NSteamSocket.Port, SteamUser.GetSteamID());
                return true;
            }
            return false;
        }

        public static LobbyGameCreated_t GetLobbyGameServer()
        {
            LobbyGameCreated_t data = new LobbyGameCreated_t();
            data.m_ulSteamIDGameServer = 0;
            if (NLobby.IsInLobby)
            {
                SteamMatchmaking.GetLobbyGameServer(NLobby.LobbySteamId, out uint ip, out ushort port, out CSteamID steamid);
                data.m_usPort = port;
                data.m_unIP = ip;
                data.m_ulSteamIDLobby = NLobby.LobbyID;
                data.m_ulSteamIDGameServer = ((ulong)steamid);
            }
            return data;
        }

    }
}
