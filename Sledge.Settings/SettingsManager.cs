using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static List<Hotkey> Hotkeys { get; set; }
        private static readonly Dictionary<string, GenericStructure> AdditionalSettings;
        public static List<FavouriteTextureFolder> FavouriteTextureFolders { get; set; }

        public static string SettingsFile { get; set; }

        static SettingsManager()
        {
            Builds = new List<Build>();
            Games = new List<Game>();
            RecentFiles = new List<RecentFile>();
            Settings = new List<Setting>();
            Hotkeys = new List<Hotkey>();
            SpecialTextureOpacities = new Dictionary<string, float>
                                          {
                                              {"null", 0},
                                              {"bevel", 0},
                                              {"tools/toolsnodraw", 0},
                                              {"aaatrigger", 0.5f},
                                              {"clip", 0.5f},
                                              {"hint", 0.5f},
                                              {"origin", 0.5f},
                                              {"skip", 0.5f},
                                          };
            AdditionalSettings = new Dictionary<string, GenericStructure>();
            FavouriteTextureFolders = new List<FavouriteTextureFolder>();

            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var sledge = Path.Combine(appdata, "Sledge");
            if (!Directory.Exists(sledge)) Directory.CreateDirectory(sledge);
            SettingsFile = Path.Combine(sledge, "Settings.vdf");
        }

        public static string GetTextureCachePath()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var sledge = Path.Combine(appdata, "Sledge");
            if (!Directory.Exists(sledge)) Directory.CreateDirectory(sledge);
            var cache = Path.Combine(sledge, "TextureCache");
            if (!Directory.Exists(cache)) Directory.CreateDirectory(cache);
            return cache;
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
            Hotkeys.Clear();
            AdditionalSettings.Clear();
            FavouriteTextureFolders.Clear();

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
            var hotkeys = root.Children.FirstOrDefault(x => x.Name == "Hotkeys");
            if (hotkeys != null)
            {
                foreach (var key in hotkeys.GetPropertyKeys())
                {
                    var spl = key.Split(':');
                    Hotkeys.Add(new Hotkey {ID = spl[0], HotkeyString = hotkeys[key]});
                }
            }

            Serialise.DeserialiseSettings(Settings.ToDictionary(x => x.Key, x => x.Value));
            Sledge.Settings.Hotkeys.SetupHotkeys(Hotkeys);

            var additionalSettings = root.Children.FirstOrDefault(x => x.Name == "AdditionalSettings");
            if (additionalSettings != null)
            {
                foreach (var child in additionalSettings.Children)
                {
                    if (child.Children.Count > 0) AdditionalSettings.Add(child.Name, child.Children[0]);
                }
            }

            var favTextures = root.Children.FirstOrDefault(x => x.Name == "FavouriteTextures");
            if (favTextures != null && favTextures.Children.Any())
            {
                try
                {
                    var ft = GenericStructure.Deserialise<List<FavouriteTextureFolder>>(favTextures.Children[0]);
                    if (ft != null) FavouriteTextureFolders.AddRange(ft);
                    FixFavouriteNames(FavouriteTextureFolders);
                }
                catch
                {
                    // Nope
                }
            }

            if (!File.Exists(SettingsFile))
            {
                Write();
            }
        }

        private static void FixFavouriteNames(IEnumerable<FavouriteTextureFolder> folders)
        {
            foreach (var f in folders)
            {
                FixFavouriteNames(f.Children);
                f.Items = f.Items.Select(x =>
                {
                    var i = x.IndexOf(':');
                    if (i >= 0) x = x.Substring(i + 1);
                    return x;
                }).ToList();
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

            // Hotkeys
            Hotkeys = Sledge.Settings.Hotkeys.GetHotkeys().ToList();
            var hotkeys = new GenericStructure("Hotkeys");
            foreach (var g in Hotkeys.GroupBy(x => x.ID))
            {
                var count = 0;
                foreach (var hotkey in g)
                {
                    hotkeys.AddProperty(hotkey.ID + ":" + count, hotkey.HotkeyString);
                    count++;
                }
            }
            root.Children.Add(hotkeys);

            // Additional
            var additional = new GenericStructure("AdditionalSettings");
            foreach (var kv in AdditionalSettings)
            {
                var child = new GenericStructure(kv.Key);
                child.Children.Add(kv.Value);
                additional.Children.Add(child);
            }
            root.Children.Add(additional);

            // Favourite textures
            var favTextures = new GenericStructure("FavouriteTextures");
            favTextures.Children.Add(GenericStructure.Serialise(FavouriteTextureFolders));
            root.Children.Add(favTextures);

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

        public static T GetAdditionalData<T>(string key)
        {
            if (!AdditionalSettings.ContainsKey(key)) return default(T);
            var additional = AdditionalSettings[key];
            try
            {
                return GenericStructure.Deserialise<T>(additional);
            }
            catch
            {
                return default(T); // Deserialisation failure
            }
        }

        public static void SetAdditionalData<T>(string key, T obj)
        {
            if (AdditionalSettings.ContainsKey(key)) AdditionalSettings.Remove(key);
            AdditionalSettings.Add(key, GenericStructure.Serialise(obj));
        }
    }
}
