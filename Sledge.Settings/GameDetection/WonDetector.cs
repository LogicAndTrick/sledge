using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Sledge.Settings.Models;

namespace Sledge.Settings.GameDetection
{
    public static class GoldsourceDirectoryScanner
    {
        public static IEnumerable<Game> Scan(string dir)
        {
            foreach (var d in Directory.GetDirectories(dir))
            {
                var di = new DirectoryInfo(d);
                var gam = Directory.GetFiles(d, "liblist.gam");
                if (gam.Any())
                {
                    var lines = File.ReadAllLines(gam[0]).Select(x => x.Trim());
                    var dict = GetGamKeyVals(lines);
                    if (!dict.ContainsKey("game")) continue;
                    var game = new Game();
                    game.Engine = Engine.Goldsource;
                    // game.Fgds lookup
                    // game.BaseDir
                    game.ModDir = di.Name;
                    game.Name = dict["game"];
                    game.GameInstallDir = dir;
                    yield return game;
                }
            }
        }

        private static Dictionary<string, string> GetGamKeyVals(IEnumerable<string> lines)
        {
            var dict = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var str = line;
                if (str.Contains("//")) str = line.Substring(0, str.IndexOf("//", System.StringComparison.Ordinal));
                str = Regex.Replace(str, @"\s+", " ").Trim();
                if (String.IsNullOrWhiteSpace(str)) continue;
                var split = str.Split(' ');
                if (split.Length != 2) continue;
                var key = split[0].ToLower();
                var value = split[1].Trim('"');
                dict.Add(key, value);
            }
            return dict;
        }
    }

    public class WonDetector : IGameDetector
    {
        public string Name
        {
            get { return "WON Installed Games"; }
        }

        public IEnumerable<Game> Detect()
        {
            var directoriesToScan = new List<string>();
            var software = Registry.LocalMachine.OpenSubKey("Software");
            if (software == null) yield break;
            var sierra = software.OpenSubKey("Sierra OnLine");
            if (sierra != null)
            {
                var setup = sierra.OpenSubKey("Setup");
                if (setup != null)
                {
                    foreach (var key in setup.GetSubKeyNames().Select(x => x.ToLower()))
                    {
                        var found = false;
                        switch (key)
                        {
                            case "halflife":
                            case "cstrike":
                            case "opforeng":
                            case "bshift":
                                found = true;
                                break;
                        }
                        if (found)
                        {
                            var gamekey = setup.OpenSubKey(key);
                            if (gamekey != null)
                            {
                                var dirkey = gamekey.GetValue("Directory");
                                if (dirkey != null)
                                {
                                    // Add
                                    directoriesToScan.Add(dirkey.ToString());
                                }
                            }
                        }
                    }
                }
            }
            var valve = software.OpenSubKey("Valve");
            if (valve != null)
            {
                var halflife = valve.OpenSubKey("Half-Life");
                if (halflife != null)
                {
                    var dirkey = halflife.GetValue("InstallPath");
                    if (dirkey != null)
                    {
                        // Add
                        directoriesToScan.Add(dirkey.ToString());
                    }
                }
            }
            foreach (var directory in directoriesToScan.Select(x => x.ToLower()).Distinct())
            {
                foreach (var game in GoldsourceDirectoryScanner.Scan(directory))
                {
                    yield return game;
                }
            }
        }
    }
}
