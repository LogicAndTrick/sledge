using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sledge.Providers;

namespace Sledge.Settings.Models
{
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Engine Engine { get; set; }
        public int BuildID { get; set; }
        public string GameInstallDir { get; set; }
        public string BaseDir { get; set; }
        public string ModDir { get; set; }
        public bool UseHDModels { get; set; }
        public string Executable { get; set; }
        public string ExecutableParameters { get; set; }
        public string MapDir { get; set; }
        public bool Autosave { get; set; }
        public bool UseCustomAutosaveDir { get; set; }
        public string AutosaveDir { get; set; }
        public int AutosaveTime { get; set; }
        public int AutosaveLimit { get; set; }
        public bool AutosaveOnlyOnChanged { get; set; }
        public bool AutosaveTriggerFileSave { get; set; }
        public string DefaultPointEntity { get; set; }
        public string DefaultBrushEntity { get; set; }
        public decimal DefaultTextureScale { get; set; }
        public decimal DefaultLightmapScale { get; set; }

        public bool IncludeFgdDirectoriesInEnvironment { get; set; }
        public bool OverrideMapSize { get; set; }
        public int OverrideMapSizeLow { get; set; }
        public int OverrideMapSizeHigh { get; set; }

        public List<Fgd> Fgds { get; set; }
        public List<string> AdditionalPackages { get; set; }
        public string PackageBlacklist { get; set; }
        public string PackageWhitelist { get; set; }

        public Game()
        {
            Fgds = new List<Fgd>();
            AdditionalPackages = new List<string>();
            AutosaveTime = 5;
            AutosaveLimit = 5;
            AutosaveOnlyOnChanged = true;
            AutosaveTriggerFileSave = true;
        }

        public void Read(GenericStructure gs)
        {
            ID = gs.PropertyInteger("ID");
            Name = gs["Name"];
            Engine = (Engine) Enum.Parse(typeof(Engine), gs["EngineID"]);
            BuildID = gs.PropertyInteger("BuildID");
            GameInstallDir = gs["WonGameDir"] ?? gs["GameInstallDir"];
            var steamGameDir = gs["SteamGameDir"];
            var steamInstall = gs.PropertyBoolean("SteamInstall");
            if (steamInstall && steamGameDir != null)
            {
                GameInstallDir = Path.Combine(Steam.SteamDirectory, "steamapps", "common", steamGameDir);
            }
            ModDir = gs["ModDir"];
            UseHDModels = gs.PropertyBoolean("UseHDModels", true);
            BaseDir = gs["BaseDir"];
            Executable = gs["Executable"];
            ExecutableParameters = gs["ExecutableParameters"];
            MapDir = gs["MapDir"];
            Autosave = gs.PropertyBoolean("Autosave");
            UseCustomAutosaveDir = gs.PropertyBoolean("UseCustomAutosaveDir");
            AutosaveDir = gs["AutosaveDir"];
            AutosaveTime = gs.PropertyInteger("AutosaveTime");
            AutosaveLimit = gs.PropertyInteger("AutosaveLimit");
            AutosaveOnlyOnChanged = gs.PropertyBoolean("AutosaveOnlyOnChanged", true);
            AutosaveTriggerFileSave = gs.PropertyBoolean("AutosaveTriggerFileChange", true);
            DefaultPointEntity = gs["DefaultPointEntity"];
            DefaultBrushEntity = gs["DefaultBrushEntity"];
            DefaultTextureScale = gs.PropertyDecimal("DefaultTextureScale");
            DefaultLightmapScale = gs.PropertyDecimal("DefaultLightmapScale");
            IncludeFgdDirectoriesInEnvironment = gs.PropertyBoolean("IncludeFgdDirectoriesInEnvironment", true);
            OverrideMapSize = gs.PropertyBoolean("OverrideMapSize");
            OverrideMapSizeLow = gs.PropertyInteger("OverrideMapSizeLow");
            OverrideMapSizeHigh = gs.PropertyInteger("OverrideMapSizeHigh");

            var additional = gs.Children.FirstOrDefault(x => x.Name == "AdditionalPackages");
            if (additional != null)
            {
                foreach (var key in additional.GetPropertyKeys())
                {
                    AdditionalPackages.Add(additional[key]);
                }
            }

            PackageBlacklist = (gs["PackageBlacklist"] ?? "").Replace(";", "\r\n");
            PackageWhitelist = (gs["PackageWhitelist"] ?? "").Replace(";", "\r\n");

            var fgds = gs.Children.FirstOrDefault(x => x.Name == "Fgds");
            if (fgds != null)
            {
                foreach (var key in fgds.GetPropertyKeys())
                {
                    Fgds.Add(new Fgd { Path = fgds[key] });
                }
            }
        }

        public void Write(GenericStructure gs)
        {
            gs["ID"] = ID.ToString(CultureInfo.InvariantCulture);
            gs["Name"] = Name;
            gs["EngineID"] = Engine.ToString();
            gs["BuildID"] = BuildID.ToString(CultureInfo.InvariantCulture);
            gs["GameInstallDir"] = GameInstallDir;
            gs["ModDir"] = ModDir;
            gs["UseHDModels"] = UseHDModels.ToString(CultureInfo.InvariantCulture);
            gs["BaseDir"] = BaseDir;
            gs["Executable"] = Executable;
            gs["ExecutableParameters"] = ExecutableParameters;
            gs["MapDir"] = MapDir;
            gs["Autosave"] = Autosave.ToString(CultureInfo.InvariantCulture);
            gs["UseCustomAutosaveDir"] = UseCustomAutosaveDir.ToString(CultureInfo.InvariantCulture);
            gs["AutosaveDir"] = AutosaveDir;
            gs["AutosaveTime"] = AutosaveTime.ToString(CultureInfo.InvariantCulture);
            gs["AutosaveLimit"] = AutosaveLimit.ToString(CultureInfo.InvariantCulture);
            gs["AutosaveOnlyOnChanged"] = AutosaveOnlyOnChanged.ToString(CultureInfo.InvariantCulture);
            gs["AutosaveTriggerFileChange"] = AutosaveTriggerFileSave.ToString(CultureInfo.InvariantCulture);
            gs["DefaultPointEntity"] = DefaultPointEntity;
            gs["DefaultBrushEntity"] = DefaultBrushEntity;
            gs["DefaultTextureScale"] = DefaultTextureScale.ToString(CultureInfo.InvariantCulture);
            gs["DefaultLightmapScale"] = DefaultLightmapScale.ToString(CultureInfo.InvariantCulture);
            gs["IncludeFgdDirectoriesInEnvironment"] = IncludeFgdDirectoriesInEnvironment.ToString(CultureInfo.InvariantCulture);
            gs["OverrideMapSize"] = OverrideMapSize.ToString(CultureInfo.InvariantCulture);
            gs["OverrideMapSizeLow"] = OverrideMapSizeLow.ToString(CultureInfo.InvariantCulture);
            gs["OverrideMapSizeHigh"] = OverrideMapSizeHigh.ToString(CultureInfo.InvariantCulture);

            var additional = new GenericStructure("AdditionalPackages");
            var i = 1;
            foreach (var add in AdditionalPackages)
            {
                additional.AddProperty(i.ToString(CultureInfo.InvariantCulture), add);
                i++;
            }
            gs.Children.Add(additional);

            gs["PackageBlacklist"] = (PackageBlacklist ?? "").Replace("\r", "").Replace('\n', ';');
            gs["PackageWhitelist"] = (PackageWhitelist ?? "").Replace("\r", "").Replace('\n', ';');

            var fgds = new GenericStructure("Fgds");
            i = 1;
            foreach (var fgd in Fgds)
            {
                fgds.AddProperty(i.ToString(CultureInfo.InvariantCulture), fgd.Path);
                i++;
            }
            gs.Children.Add(fgds);
        }

        public string GetMapDirectory()
        {
            return Path.Combine(GetModDirectory(), "maps");
        }

        public string GetModDirectory()
        {
            return Path.Combine(GameInstallDir, ModDir);
        }

        public string GetBaseDirectory()
        {
            return Path.Combine(GameInstallDir, BaseDir);
        }

        public IEnumerable<string> GetFgdDirectories()
        {
            return Fgds.Select(x => Path.GetDirectoryName(Path.GetFullPath(x.Path))).Distinct();
        }

        public string GetExecutable()
        {
            return Path.Combine(GameInstallDir, Executable);
        }

        public string GetGameLaunchArgument()
        {
            var mod = (ModDir ?? "").ToLowerInvariant();
            if (mod != "valve") return "-game " + mod;
            return "";
        }

        public IEnumerable<string> GetTextureBlacklist()
        {
            var bl = new List<string>();
            if (Engine == Engine.Goldsource)
            {
                bl.Add("cached");
                bl.Add("gfx");
            }
            bl.AddRange((PackageBlacklist ?? "").Trim().Split('\n').Select(x => x.Trim()).Where(x => !String.IsNullOrWhiteSpace(x)));
            return bl;
        }

        public IEnumerable<string> GetTextureWhitelist()
        {
            return (PackageWhitelist ?? "").Trim().Split('\n').Select(x => x.Trim()).Where(x => !String.IsNullOrWhiteSpace(x));
        }

        private int GetSteamAppId()
        {
            if (Engine == Engine.Goldsource)
            {
                switch ((ModDir ?? "").ToLowerInvariant())
                {
                    case "bshift":
                        return 130;
                    case "czero":
                        return 80;
                    case "czeror":
                        return 100;
                    case "cstrike":
                        return 10;
                    case "dod":
                        return 30;
                    case "dmc":
                        return 40;
                    case "ricochet":
                        return 60;
                    case "gearbox":
                        return 50;
                    case "tfc":
                        return 20;
                    case "valve":
                    default:
                        return 70;
                }
            }
            // todo source
            return 0;
        }
    }
}
