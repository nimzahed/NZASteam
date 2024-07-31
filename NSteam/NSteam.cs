using NSteamLib.Lobby;
using NSteamLib.Networking.Socket;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSteamLib
{
     
    public static class NSteam
    {
       
       //public static Logger? logger;

        static string gameName = "";
        static bool apiRunning = false;
        public static string GameName => gameName;
        public static bool IsApiRunning => apiRunning;


        public static void Init(string _gameName = "", string logFile = "log")
        {
            //logger = new Logger(logFile, "Logs");


            if (SteamAPI.RestartAppIfNecessary(new AppId_t(480))) return;
            if (!SteamAPI.IsSteamRunning()) { /*logger.Log("Steam Is Not Running");*/ return; }
            if (!SteamAPI.Init()) { /*logger.Log("Steam not Initialized");*/ return; }
            if (_gameName == "" || _gameName.Trim(' ') == "") { _gameName = "Default Game Name"; }
            gameName = _gameName;
            apiRunning = true;
            //logger.Log("Steam Initialized");
            NLobby.Init();
            NSteamSocket.Init();
        }

        public static void Shutdown()
        {
            //logger?.Dispose();
            NLobby.Shutdown();
            NSteamSocket.Shutdown();
            apiRunning = false;
            SteamAPI.Shutdown();
        }
    }
}
