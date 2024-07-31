using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSteamLib.Users
{
    public class NUserData
    {

        public string name;
        public CSteamID steamID;
        public NImageData Image;
        public EPersonaState personaState;

        public NUserData(string _name, CSteamID _steamId, NImageData _image, EPersonaState _personaState)
        {
            this.name = _name;
            this.steamID = _steamId;
            this.Image = _image;
            this.personaState = _personaState;
        }

    }
    public class NImageData
    {
        public byte[] imageData;
        public int width, height;

        public NImageData(byte[] imageData, int width, int height)
        {
            this.imageData = imageData;
            this.width = width;
            this.height = height;
        }
    }
}
