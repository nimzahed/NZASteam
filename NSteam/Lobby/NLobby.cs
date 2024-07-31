using NSteamLib.Lobby.Message;
using NSteamLib.Users;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

# nullable enable 
namespace NSteamLib.Lobby
{
    public static class NLobby
    {
        static ulong lobbyId = 0;
        public static ulong LobbyID => lobbyId;
        public static CSteamID LobbySteamId => new CSteamID(lobbyId);
        public static bool IsInLobby => LobbyID != 0;

        static List<NUserData> memberDatas = new List<NUserData>();
        public static List<NUserData> MemberDatas => memberDatas;

        public static List<NLobbyData> lobbyDatas = new List<NLobbyData>();
        

        public static CallResult<LobbyCreated_t>? lobbyCreate;
        public static CallResult<LobbyEnter_t>? lobbyEnter;
        public static CallResult<LobbyMatchList_t>? lobbyRequest;

        public static void Init()
        {
            NLobbyMessage.Init();
            NLobbyManage.Init();

            lobbyCreate = CallResult<LobbyCreated_t>.Create(LobbyCreateResult);
            lobbyEnter = CallResult<LobbyEnter_t>.Create(LobbyEnterResult);
            lobbyRequest = CallResult<LobbyMatchList_t>.Create(LobbyRequestResult);

        }
        public static void Shutdown()
        {
            NLobbyMessage.Shutdown();
            NLobbyManage.Shutdown();

            lobbyCreate = null;
            lobbyEnter = null;
            lobbyRequest = null;

        }

        public static bool CreateLobby(int maxMembers = 2, ELobbyType lobbyType = (ELobbyType)2)
        {
            //NSteam.logger?.Log("Creating Lobby..");

            maxMembers = maxMembers < 2 ? 2 : maxMembers; maxMembers = maxMembers > 250 ? 250 : maxMembers;

            if (IsInLobby) { return false; }
            
            
            // create lobby
            SteamAPICall_t call = SteamMatchmaking.CreateLobby(lobbyType, maxMembers);
            lobbyCreate?.Set(call);
            return true;
            
        }

        public static void RefreshLobbys(ELobbyDistanceFilter distance = (ELobbyDistanceFilter)3, int maxResult = 50)
        {
            maxResult = maxResult > 50 ? 50 : maxResult < 0 ? 0 : maxResult;

            SteamMatchmaking.AddRequestLobbyListResultCountFilter(maxResult);
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(distance);
            // set lobby list filter by game name (its just for test when using 480 game for test)
            SteamMatchmaking.AddRequestLobbyListStringFilter("Game", NSteam.GameName, ELobbyComparison.k_ELobbyComparisonEqual);
            
            // request lobby list and copy it to callback varible
            SteamAPICall_t call = SteamMatchmaking.RequestLobbyList();
            // call an event when recieve callback
            lobbyRequest?.Set(call);

        }


        private static void LobbyCreateResult(LobbyCreated_t param, bool bIOFailure)
        {
            lobbyId = 0;
            switch (param.m_eResult)
            {
                case EResult.k_EResultOK:
                    // get lobby id for EZ access
                    lobbyId = param.m_ulSteamIDLobby;

                    // make lobby id varible
                    CSteamID lobby = new CSteamID(param.m_ulSteamIDLobby);

                    // set lobby data's GameName and Lobby Name (GameName is jus for when debugging when using gameid 480)
                    // anyway it will make less bugs
                    NLobbyManage.SetLobbyData("Name", SteamFriends.GetPersonaName() + "'s Lobby");
                    NLobbyManage.SetLobbyData("Game", NSteam.GameName);
                    NLobbyEvents.OnLobbyCreated(lobbyId);
                    break;
                case EResult.k_EResultFail:
                    //NSteam.logger?.Log("The server responded, but with an unknown internal error.");
                    break;
                case EResult.k_EResultTimeout:
                   // NSteam.logger?.Log("The message was sent to the Steam servers, but it didn't respond.");
                    break;
                case EResult.k_EResultLimitExceeded:
                    //NSteam.logger?.Log("Your game client has created too many lobbies and is being rate limited.");
                    break;
                case EResult.k_EResultAccessDenied:
                    //NSteam.logger?.Log("Your game isn't set to allow lobbies, or your client does haven't rights to play the game");
                    break;
                case EResult.k_EResultNoConnection:
                    //NSteam.logger?.Log("Your Steam client doesn't have a connection to the back-end.");
                    break;
                default:
                   // NSteam.logger?.Log("IDK What Happened !!");
                    break;
            }

        }

        public static List<NUserData> RefreshLobbyMembers()
        {
            List<NUserData> members = new List<NUserData>();
            if (IsInLobby)
            {
                int membersCount = SteamMatchmaking.GetNumLobbyMembers(LobbySteamId);
                for (int i = 0; i < membersCount; i++)
                {
                    CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(LobbySteamId, i);
                    NUserData? data = NSteamUsers.GetUserData((ulong)member);
                    if (data != null)
                    {
                        members.Add(data);
                    }
                }
            }
            memberDatas = members;
            return members;
        }

        private static void LobbyRequestResult(LobbyMatchList_t param, bool bIOFailure)
        {

            if (bIOFailure)
                return;
            lobbyDatas.Clear();
            //NSteam.logger?.Log("Lobby List Refreshed.");
            uint numLobbies = param.m_nLobbiesMatching;
            for (int i = 0; i < numLobbies; i++)
            {

                // varible to make lobby id by lobby index
                CSteamID lobby = SteamMatchmaking.GetLobbyByIndex(i);
                // get name , members count and members limit
                string name = SteamMatchmaking.GetLobbyData(lobby, "Name");
                int members = SteamMatchmaking.GetNumLobbyMembers(lobby);
                int limit = SteamMatchmaking.GetLobbyMemberLimit(lobby);
                CSteamID owner = SteamMatchmaking.GetLobbyOwner(lobby);

                // add lobby to list
                lobbyDatas.Add(new NLobbyData(name, owner, lobby, members, limit));
               // NSteam.logger?.Log($"Lobby{i}: {name} - {members}/{limit} - {lobby} - owner {owner}");
            }
            NLobbyEvents.OnLobbysRefreshed(lobbyDatas);

        }

        public static void LeaveLobby(string resion = "Leave")
        {
          //  NSteam.logger?.Log("Leaving Lobby.");
            if (!IsInLobby) { /*NSteam.logger?.Log("your not in lobby to leave."); */return; };



            // leave
            SteamMatchmaking.LeaveLobby(new CSteamID(lobbyId));
            // wait a second to make sure client leaved lobby
            //Thread.Sleep(1000);
            NLobbyEvents.OnLobbyLeaved(lobbyId, resion);
            // set in lobby to false
            lobbyId = 0;

        }

        public static void JoinLobby(ulong lobbyid)
        {
            // leave lobby if Ur in lobby
            if (IsInLobby) { LeaveLobby("Connecting to Other Server"); };
           // NSteam.logger?.Log("Joining To Server.");
            CSteamID lobby = new CSteamID(lobbyid);
            SteamAPICall_t call = SteamMatchmaking.JoinLobby(lobby);
            lobbyEnter?.Set(call);
        }

        private static void LobbyEnterResult(LobbyEnter_t param, bool bIOFailure)
        {
            if (param.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                lobbyId = param.m_ulSteamIDLobby;
                CSteamID lobby = new CSteamID(lobbyId);
                // Hey me, I'm in Lobby Did you Know That? :|
                //NSteam.logger?.Log($"Joined To : {SteamMatchmaking.GetLobbyData(lobby, "Name")} -  {lobby}");
                NLobbyEvents.OnLobbyEntered(lobbyId);
            }
            else
            {
                if (param.m_bLocked)
                {
                    //NSteam.logger?.Log("Lobby Enter Failed! Only Invited Users Can Join.");
                }
                else
                {
                    //NSteam.logger?.Log("Lobby Enter Failed.");
                }
            }
        }

        
    }
}
