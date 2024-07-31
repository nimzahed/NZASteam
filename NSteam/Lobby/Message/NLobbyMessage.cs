using Steamworks;

# nullable enable 
namespace NSteamLib.Lobby.Message
{
    public static class NLobbyMessage
    {


        public static Callback<LobbyChatMsg_t>? LobbyChatMsg;
        public static Callback<LobbyChatUpdate_t>? LobbyChatUpdate;
        public static Callback<GameRichPresenceJoinRequested_t>? RichJoinRequested ;
        public static Callback<GameLobbyJoinRequested_t>? LobbyJoinRequested;

        public static void Init()
        {


            LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessage);
            LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdated);
            RichJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(OnRichJoinRequested);
            LobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);

        }
        public static void Shutdown()
        {
            LobbyChatMsg = null;
            LobbyChatUpdate = null;
            RichJoinRequested = null;
            LobbyJoinRequested = null;
        }


        public static bool Send(byte[] buffer)
        {
            //NSteam.logger?.Log("Sending Lobby Message");
            if (NLobby.LobbyID != 0)
            {
                return SteamMatchmaking.SendLobbyChatMsg(new CSteamID(NLobby.LobbyID), buffer, buffer.Length);

            }
            return false;
        }

        private static void OnLobbyChatMessage(LobbyChatMsg_t param)
        {
            ulong lobbyid = param.m_ulSteamIDLobby;
            ulong userid = param.m_ulSteamIDUser;
            EChatEntryType entryType = (EChatEntryType)param.m_eChatEntryType;
            //NSteam.logger?.Log("Got a Lobby Msg type : " + entryType.ToString());

            switch (entryType)
            {
                case EChatEntryType.k_EChatEntryTypeInvalid:
                    break;
                case EChatEntryType.k_EChatEntryTypeChatMsg:
                    byte[] pvData = new byte[4096];
                    int lenght = SteamMatchmaking.GetLobbyChatEntry(new CSteamID(lobbyid), (int)param.m_iChatID, out CSteamID steamid, pvData, pvData.Length, out EChatEntryType newtype);
                    NMessageEvents.OnMessageRecieved(userid, lobbyid, pvData);

                    break;
                case EChatEntryType.k_EChatEntryTypeTyping:
                    break;
                case EChatEntryType.k_EChatEntryTypeInviteGame:
                    break;
                case EChatEntryType.k_EChatEntryTypeEmote:
                    break;
                case EChatEntryType.k_EChatEntryTypeLeftConversation:
                    break;
                case EChatEntryType.k_EChatEntryTypeEntered:
                    break;
                case EChatEntryType.k_EChatEntryTypeWasKicked:
                    break;
                case EChatEntryType.k_EChatEntryTypeWasBanned:
                    break;
                case EChatEntryType.k_EChatEntryTypeDisconnected:
                    break;
                case EChatEntryType.k_EChatEntryTypeHistoricalChat:
                    break;
                case EChatEntryType.k_EChatEntryTypeLinkBlocked:
                    break;
                default:
                    break;
            }

        }

        private static void OnLobbyChatUpdated(LobbyChatUpdate_t param)
        {
            ulong lobbyid = param.m_ulSteamIDLobby;
            ulong changed = param.m_ulSteamIDUserChanged;
            ulong changer = param.m_ulSteamIDMakingChange;
            EChatMemberStateChange eChatMemberStateChange =(EChatMemberStateChange)param.m_rgfChatMemberStateChange;
            string name = SteamFriends.GetFriendPersonaName(new CSteamID(changed));
            switch (eChatMemberStateChange)
            {
                case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
                    //NSteam.logger?.Log(name + " Joined the Lobby");
                    NLobbyEvents.OnMemberEntered(changed, lobbyid);
                    break;
                case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
                    //NSteam.logger?.Log(name + " Left The Lobby.");
                    NLobbyEvents.OnLobbyLeaved(changed, "Leave");
                    break;
                case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
                    //NSteam.logger?.Log(name + " Disconnected The Lobby.");
                    NLobbyEvents.OnLobbyLeaved(changed, "Disconnected");
                    break;
                case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
                    //NSteam.logger?.Log(name + " Kickd out of The Lobby.");
                    NLobbyEvents.OnLobbyLeaved(changed, "Kicked");
                    break;
                case EChatMemberStateChange.k_EChatMemberStateChangeBanned:
                    //NSteam.logger?.Log(name + " banned off of The Lobby.");
                    NLobbyEvents.OnLobbyLeaved(changed, "Banned");
                    break;
                default:
                    break;
            }
        }

        private static void OnRichJoinRequested(GameRichPresenceJoinRequested_t param)
        {

        }
        private static void OnLobbyJoinRequested(GameLobbyJoinRequested_t param)
        {
            CSteamID friend = param.m_steamIDFriend;
            CSteamID lobby = param.m_steamIDLobby;
            //NSteam.logger?.Log($"Joining to {SteamFriends.GetFriendPersonaName(friend)} Lobby");
            NLobby.JoinLobby(((ulong)lobby));
        }
    }
}
