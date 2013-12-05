using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Sledge.Providers;
using Sledge.Settings.Models;

namespace Sledge.Settings
{
    public static class SettingsManager
    {
        public static List<Build> Builds { get; set; }
        public static List<Game> Games { get; set; }
        public static List<RecentFile> RecentFiles { get; set; }
        public static List<Setting> Settings { get; set; }

        public static string SettingsFile { get; set; }

        static SettingsManager()
        {
            Builds = new List<Build>();
            Games = new List<Game>();
            RecentFiles = new List<RecentFile>();
            Settings = new List<Setting>();
            SpecialTextureOpacities = new Dictionary<string, float>
                                          {
                                              {"null", 0},
                                              {"tools/toolsnodraw", 0},
                                              {"aaatrigger", 0.5f},
                                              {"bevel", 0.5f},
                                              {"clip", 0.5f},
                                              {"hint", 0.5f},
                                              {"origin", 0.5f},
                                              {"skip", 0.5f},
                                              {"tools/toolsareaportal", 0.5f},
                                              {"tools/toolsblock_los", 0.5f},
                                              {"tools/toolsblockbullets", 0.5f},
                                              {"tools/toolsblocklight", 0.5f},
                                              {"tools/toolsclip", 0.5f},
                                              {"tools/toolscontrolclip", 0.5f},
                                              {"tools/toolsfog", 0.5f},
                                              {"tools/toolshint", 0.5f},
                                              {"tools/toolsinvisible", 0.5f},
                                              {"tools/toolsinvisibledisplacement", 0.5f},
                                              {"tools/toolsinvisibleladder", 0.5f},
                                              {"tools/toolsnpcclip", 0.5f},
                                              {"tools/toolsoccluder", 0.5f},
                                              {"tools/toolsorigin", 0.5f},
                                              {"tools/toolsplayerclip", 0.5f},
                                              {"tools/toolsskip", 0.5f},
                                              {"tools/toolstrigger", 0.5f}
                                          };

            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var sledge = Path.Combine(appdata, "Sledge");
            if (!Directory.Exists(sledge)) Directory.CreateDirectory(sledge);
            SettingsFile = Path.Combine(sledge, "Settings.vdf");
        }
        
        public static float GetSpecialTextureOpacity(string name)
        {
            name = name.ToLowerInvariant();
            var val = SpecialTextureOpacities.ContainsKey(name) ? SpecialTextureOpacities[name] : 1;
            if (View.DisableToolTextureTransparency || View.GloballyDisableTransparency)
            {
                return val < 0.1 ? 0 : 1;
            }
            return val;
        }

        private static readonly IDictionary<string, float> SpecialTextureOpacities;

        private static GenericStructure ReadSettingsFile()
        {
            if (File.Exists(SettingsFile)) return GenericStructure.Parse(SettingsFile).FirstOrDefault();

            var exec = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(exec, "Settings.vdf");
            if (File.Exists(path)) return GenericStructure.Parse(path).FirstOrDefault();

            return null;
        }

        public static void Read()
        {
            Builds.Clear();
            Games.Clear();
            RecentFiles.Clear();
            Settings.Clear();

            var root = ReadSettingsFile();

            if (root == null) return;

            var settings = root.Children.FirstOrDefault(x => x.Name == "Settings");
            if (settings != null)
            {
                foreach (var key in settings.GetPropertyKeys())
                {
                    Settings.Add(new Setting {Key = key, Value = settings[key]});
                }
            }
            var recents = root.Children.FirstOrDefault(x => x.Name == "RecentFiles");
            if (recents != null)
            {
                foreach (var key in recents.GetPropertyKeys())
                {
                    int i;
                    if (int.TryParse(key, out i))
                    {
                        RecentFiles.Add(new RecentFile {Location = recents[key], Order = i});
                    }
                }
            }
            var games = root.Children.Where(x => x.Name == "Game");
            foreach (var game in games)
            {
                var g = new Game();
                g.Read(game);
                Games.Add(g);
            }
            var builds = root.Children.Where(x => x.Name == "Build");
            foreach (var build in builds)
            {
                var b = new Build();
                b.Read(build);
                Builds.Add(b);
            }
            Serialise.DeserialiseSettings(Settings.ToDictionary(x => x.Key, x => x.Value));

            if (!File.Exists(SettingsFile))
            {
                Write();
            }
        }

        public static void Write()
        {
            var newSettings = Serialise.SerialiseSettings().Select(s => new Setting { Key = s.Key, Value = s.Value });
            Settings.Clear();
            Settings.AddRange(newSettings);

            var root = new GenericStructure("Sledge");

            // Settings
            var settings = new GenericStructure("Settings");
            foreach (var setting in Settings)
            {
                settings.AddProperty(setting.Key, setting.Value);
            }
            root.Children.Add(settings);

            // Recent Files
            var recents = new GenericStructure("RecentFiles");
            var i = 1;
            foreach (var file in RecentFiles.OrderBy(x => x.Order).Select(x => x.Location).Where(File.Exists))
            {
                recents.AddProperty(i.ToString(CultureInfo.InvariantCulture), file);
                i++;
            }
            root.Children.Add(recents);

            // Games > Fgds/Wads
            foreach (var game in Games)
            {
                var g = new GenericStructure("Game");
                game.Write(g);
                root.Children.Add(g);
            }

            // Builds
            foreach (var build in Builds)
            {
                var b = new GenericStructure("Build");
                build.Write(b);
                root.Children.Add(b);
            }

            File.WriteAllText(SettingsFile, root.ToString());
        }

        private static string GetSessionFile()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var sledge = Path.Combine(appdata, "Sledge");
            if (!Directory.Exists(sledge)) Directory.CreateDirectory(sledge);
            return Path.Combine(sledge, "session");
        }

        public static void SaveSession(IEnumerable<Tuple<string, Game>> files)
        {
            File.WriteAllLines(GetSessionFile(), files.Select(x => x.Item1 + ":" + x.Item2.ID));
        }

        public static IEnumerable<Tuple<string, Game>> LoadSession()
        {
            var sf = GetSessionFile();
            if (!File.Exists(sf)) return new List<Tuple<string, Game>>();
            return File.ReadAllLines(sf)
                .Select(x =>
                            {
                                var i = x.LastIndexOf(":", StringComparison.Ordinal);
                                var file = x.Substring(0, i);
                                var num = x.Substring(i + 1);
                                int id;
                                int.TryParse(num, out id);
                                return Tuple.Create(file, Games.FirstOrDefault(g => g.ID == id));
                            })
                .Where(x => File.Exists(x.Item1) && x.Item2 != null);
        }
    }
}
