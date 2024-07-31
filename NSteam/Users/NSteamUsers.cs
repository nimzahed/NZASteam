using NSteamLib.Lobby;
using Steamworks;
using System.Collections.Generic;

# nullable enable 
namespace NSteamLib.Users
{
    public static class NSteamUsers
    {

        public enum ProfileSize
        {
            Small,
            Medium,
            Large
        }
        public enum NPchDialog
        {
            steamid,
            chat,
            jointrade,
            stats,
            achievements,
            friendadd,
            friendremove,
            friendrequestaccept,
            friendrequestignore
        }
        public static List<NUserData> RefreshFriends(EFriendFlags filter = EFriendFlags.k_EFriendFlagAll)
        {
           // NSteam.logger?.Log("Requesting FriendData");
            List<NUserData> friends = new List<NUserData>();
            int friendCount = SteamFriends.GetFriendCount(filter);
            for (int i = 0; i < friendCount; i++)
            {

                CSteamID friendid = SteamFriends.GetFriendByIndex(i, filter);
                string name = SteamFriends.GetFriendPersonaName(friendid);
                EPersonaState state = SteamFriends.GetFriendPersonaState(friendid);
                NImageData imageData = GetTargetImage(friendid);
                friends.Add(new NUserData(name, friendid, imageData, state));

            }
            return friends;
        }

        public static NUserData? GetUserData(ulong steamID)
        {
            if (steamID != 0)
            {
                CSteamID friendid = new CSteamID(steamID);
                string name = SteamFriends.GetFriendPersonaName(friendid);
                EPersonaState state = SteamFriends.GetFriendPersonaState(friendid);
                NImageData imageData = GetTargetImage(friendid);
                return new NUserData(name, friendid, imageData, state);
            }
            return null;
        }

        public static NImageData GetTargetImage(CSteamID steamID, ProfileSize size = ProfileSize.Medium)
        {
            int avatarInt;
            NImageData imageData = new NImageData(new byte[0], 0,0);

            switch (size)
            {
                case ProfileSize.Small:
                    avatarInt = SteamFriends.GetSmallFriendAvatar(steamID);
                    break;
                case ProfileSize.Medium:
                    avatarInt = SteamFriends.GetMediumFriendAvatar(steamID);
                    break;
                case ProfileSize.Large:
                    avatarInt = SteamFriends.GetLargeFriendAvatar(steamID);
                    break;
                default:
                    avatarInt = SteamFriends.GetMediumFriendAvatar(steamID);
                    break;
            }

            // Variables to hold the width and height of the avatar
            uint width, height;

            // Get the size of the avatar
            bool isValid = SteamUtils.GetImageSize(avatarInt, out width, out height);
            if (isValid)
            {
                // Create a byte array to store the avatar image data
                byte[] avatarStream = new byte[4 * (int)width * (int)height];

                // Get the avatar image data
                isValid = SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);
                if (isValid)
                {
                    imageData = new NImageData(avatarStream, (int)height, (int)width);
                }

            }
            return imageData;
        }

        public static void OpenInviteOverlay()
        {
            if (NLobby.IsInLobby)
            {
                SteamFriends.ActivateGameOverlayInviteDialog(NLobby.LobbySteamId);
            }
        }

        public static void OpenUserDialog(CSteamID user, NPchDialog dialog = NPchDialog.chat)
        {
            SteamFriends.ActivateGameOverlayToUser(dialog.ToString(), user);
        }
    }
}
