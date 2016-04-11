using Sledge.Settings.Models;

namespace Sledge.Settings.GameDetection.GameFiles
{
    public class SteamApp
    {
        public int AppID { get; set; }
        public Engine Engine { get; set; }

        public string GameDirectory { get; set; }
        public string ModDirectory { get; set; }
        public string ExecutableName { get; set; }

        public SteamApp(int appId, Engine engine)
        {
            AppID = appId;
            Engine = engine;
        }
    }
}
