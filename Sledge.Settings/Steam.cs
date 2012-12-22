namespace Sledge.Settings
{
    public static class Steam
    {
        public static string SteamDirectory { get; set; }
        public static string SteamUsername { get; set; }

        static Steam()
        {
            SteamDirectory = "";
            SteamUsername = "";
        }
    }
}