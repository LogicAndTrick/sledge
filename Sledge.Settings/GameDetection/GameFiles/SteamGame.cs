using System;
using System.Collections.Generic;

namespace Sledge.Settings.GameDetection.GameFiles
{
    public class SteamGame
    {
        public int AppID { get; set; }
        public string Name { get; set; }
        public List<String> Dependencies { get; set; }

        public SteamGame(int appID, string name, List<string> dependencies)
        {
            AppID = appID;
            Name = name;
            Dependencies = dependencies;
        }
    }
}
