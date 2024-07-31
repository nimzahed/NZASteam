using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSteamLib.Lobby
{
    public struct NLobbyData
    {
        public string name;
        public CSteamID owner;
        public CSteamID lobby;
        public int memberCount;
        public int maxMembers;

        public NLobbyData(string _name, CSteamID _owner, CSteamID _lobby, int _memberCount, int _maxMembers)
        {
            this.name = _name;
            this.owner = _owner;
            this.lobby = _lobby;
            this.memberCount = _memberCount;
            this.maxMembers = _maxMembers;
        }
    }
}
