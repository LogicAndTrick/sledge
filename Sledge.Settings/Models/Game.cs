using System;
using System.Collections.Generic;
using System.Globalization;
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
        public bool SteamInstall { get; set; }
        public string WonGameDir { get; set; }
        public string SteamGameDir { get; set; }
        public string BaseDir { get; set; }
        public string ModDir { get; set; }
        public string MapDir { get; set; }
        public bool Autosave { get; set; }
        public bool UseCustomAutosaveDir { get; set; }
        public string AutosaveDir { get; set; }
        public int AutosaveTime { get; set; }
        public int AutosaveLimit { get; set; }
        public bool AutosaveOnlyOnChanged { get; set; }
        public string DefaultPointEntity { get; set; }
        public string DefaultBrushEntity { get; set; }
        public decimal DefaultTextureScale { get; set; }
        public decimal DefaultLightmapScale { get; set; }

        public bool OverrideMapSize { get; set; }
        public int OverrideMapSizeLow { get; set; }
        public int OverrideMapSizeHigh { get; set; }

        public List<Fgd> Fgds { get; set; }
        public List<Wad> Wads { get; set; }

        public Game()
        {
            Fgds = new List<Fgd>();
            Wads = new List<Wad>();
            AutosaveTime = 5;
            AutosaveLimit = 5;
            AutosaveOnlyOnChanged = true;
        }

        public void Read(GenericStructure gs)
        {
            ID = gs.PropertyInteger("ID");
            Name = gs["Name"];
            Engine = (Engine) Enum.Parse(typeof(Engine), gs["EngineID"]);
            BuildID = gs.PropertyInteger("BuildID");
            SteamInstall = gs.PropertyBoolean("SteamInstall");
            WonGameDir = gs["WonGameDir"];
            SteamGameDir = gs["SteamGameDir"];
            ModDir = gs["ModDir"];
            BaseDir = gs["BaseDir"];
            MapDir = gs["MapDir"];
            Autosave = gs.PropertyBoolean("Autosave");
            UseCustomAutosaveDir = gs.PropertyBoolean("UseCustomAutosaveDir");
            AutosaveDir = gs["AutosaveDir"];
            AutosaveTime = gs.PropertyInteger("AutosaveTime");
            AutosaveLimit = gs.PropertyInteger("AutosaveLimit");
            AutosaveOnlyOnChanged = gs.PropertyBoolean("AutosaveOnlyOnChanged");
            DefaultPointEntity = gs["DefaultPointEntity"];
            DefaultBrushEntity = gs["DefaultBrushEntity"];
            DefaultTextureScale = gs.PropertyDecimal("DefaultTextureScale");
            DefaultLightmapScale = gs.PropertyDecimal("DefaultLightmapScale");
            OverrideMapSize = gs.PropertyBoolean("OverrideMapSize");
            OverrideMapSizeLow = gs.PropertyInteger("OverrideMapSizeLow");
            OverrideMapSizeHigh = gs.PropertyInteger("OverrideMapSizeHigh");

            var wads = gs.Children.FirstOrDefault(x => x.Name == "Wads");
            if (wads != null)
            {
                foreach (var key in wads.GetPropertyKeys())
                {
                    Wads.Add(new Wad { Path = wads[key] });
                }
            }

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
            gs["SteamInstall"] = SteamInstall.ToString(CultureInfo.InvariantCulture);
            gs["WonGameDir"] = WonGameDir;
            gs["SteamGameDir"] = SteamGameDir;
            gs["ModDir"] = ModDir;
            gs["BaseDir"] = BaseDir;
            gs["MapDir"] = MapDir;
            gs["Autosave"] = Autosave.ToString(CultureInfo.InvariantCulture);
            gs["UseCustomAutosaveDir"] = UseCustomAutosaveDir.ToString(CultureInfo.InvariantCulture);
            gs["AutosaveDir"] = AutosaveDir;
            gs["AutosaveTime"] = AutosaveTime.ToString(CultureInfo.InvariantCulture);
            gs["AutosaveLimit"] = AutosaveLimit.ToString(CultureInfo.InvariantCulture);
            gs["AutosaveOnlyOnChanged"] = AutosaveOnlyOnChanged.ToString(CultureInfo.InvariantCulture);
            gs["DefaultPointEntity"] = DefaultPointEntity;
            gs["DefaultBrushEntity"] = DefaultBrushEntity;
            gs["DefaultTextureScale"] = DefaultTextureScale.ToString(CultureInfo.InvariantCulture);
            gs["DefaultLightmapScale"] = DefaultLightmapScale.ToString(CultureInfo.InvariantCulture);
            gs["OverrideMapSize"] = OverrideMapSize.ToString(CultureInfo.InvariantCulture);
            gs["OverrideMapSizeLow"] = OverrideMapSizeLow.ToString(CultureInfo.InvariantCulture);
            gs["OverrideMapSizeHigh"] = OverrideMapSizeHigh.ToString(CultureInfo.InvariantCulture);

            var wads = new GenericStructure("Wads");
            var i = 1;
            foreach (var wad in Wads)
            {
                wads.AddProperty(i.ToString(CultureInfo.InvariantCulture), wad.Path);
                i++;
            }
            gs.Children.Add(wads);

            var fgds = new GenericStructure("Fgds");
            i = 1;
            foreach (var fgd in Fgds)
            {
                fgds.AddProperty(i.ToString(CultureInfo.InvariantCulture), fgd.Path);
                i++;
            }
            gs.Children.Add(fgds);
        }
    }
}
