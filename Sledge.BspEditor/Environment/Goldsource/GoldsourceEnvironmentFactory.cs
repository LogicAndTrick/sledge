using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Environment.Goldsource
{
    [Export(typeof(IEnvironmentFactory))]
    [AutoTranslate]
    public class GoldsourceEnvironmentFactory : IEnvironmentFactory
    {
        public Type Type => typeof(GoldsourceEnvironment);
        public string TypeName => "GoldsourceEnvironment";
        public string Description { get; set; } = "Goldsource";

        private T GetVal<T>(Dictionary<string, string> dictionary, string key, T def = default(T))
        {
            if (dictionary.TryGetValue(key, out var val))
            {
                try
                {
                    return (T) Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
                }
                catch
                {
                    //
                }
            }
            return def;
        }

        public IEnvironment Deserialise(SerialisedEnvironment environment)
        {
            var gse = new GoldsourceEnvironment()
            {
                ID = environment.ID,
                Name = environment.Name,
                BaseDirectory = GetVal(environment.Properties, "BaseDirectory", ""),
                GameDirectory = GetVal(environment.Properties, "GameDirectory", ""),
                ModDirectory = GetVal(environment.Properties, "ModDirectory", ""),
                GameExe = GetVal(environment.Properties, "GameExe", ""),
                LoadHdModels = GetVal(environment.Properties, "LoadHdModels", true),

                FgdFiles = GetVal(environment.Properties, "FgdFiles", "").Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
                DefaultPointEntity = GetVal(environment.Properties, "DefaultPointEntity", ""),
                DefaultBrushEntity = GetVal(environment.Properties, "DefaultBrushEntity", ""),
                OverrideMapSize = GetVal(environment.Properties, "OverrideMapSize", false),
                MapSizeLow = GetVal(environment.Properties, "MapSizeLow", -4096m),
                MapSizeHigh = GetVal(environment.Properties, "MapSizeHigh", 4096m),
                IncludeFgdDirectoriesInEnvironment = GetVal(environment.Properties, "IncludeFgdDirectoriesInEnvironment", true),

                ToolsDirectory = GetVal(environment.Properties, "ToolsDirectory", ""),
                IncludeToolsDirectoryInEnvironment = GetVal(environment.Properties, "IncludeToolsDirectoryInEnvironment", true),
                BspExe = GetVal(environment.Properties, "BspExe", ""),
                CsgExe = GetVal(environment.Properties, "CsgExe", ""),
                VisExe = GetVal(environment.Properties, "VisExe", ""),
                RadExe = GetVal(environment.Properties, "RadExe", ""),

                GameCopyBsp = GetVal(environment.Properties, "GameCopyBsp", true),
                GameRun = GetVal(environment.Properties, "GameRun", true),
                GameAsk = GetVal(environment.Properties, "GameAsk", true),

                MapCopyBsp = GetVal(environment.Properties, "MapCopyBsp", false),
                MapCopyMap = GetVal(environment.Properties, "MapCopyMap", false),
                MapCopyLog = GetVal(environment.Properties, "MapCopyLog", false),
                MapCopyErr = GetVal(environment.Properties, "MapCopyErr", false),
                MapCopyRes = GetVal(environment.Properties, "MapCopyRes", false),

                DefaultTextureScale = GetVal(environment.Properties, "DefaultTextureScale", 1m),
                ExcludedWads = GetVal(environment.Properties, "ExcludedWads", "").Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
                AdditionalTextureFiles = GetVal(environment.Properties, "AdditionalTextureFiles", "").Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).ToList(),
            };
            return gse;
        }

        public SerialisedEnvironment Serialise(IEnvironment environment)
        {
            var env = (GoldsourceEnvironment) environment;
            var se = new SerialisedEnvironment
            {
                ID = environment.ID,
                Name = environment.Name,
                Type = TypeName,
                Properties =
                {
                    { "BaseDirectory", env.BaseDirectory },
                    { "GameDirectory", env.GameDirectory },
                    { "ModDirectory", env.ModDirectory },
                    { "GameExe", env.GameExe },
                    { "LoadHdModels", Convert.ToString(env.LoadHdModels, CultureInfo.InvariantCulture) },

                    { "FgdFiles", String.Join(";", env.FgdFiles) },
                    { "DefaultPointEntity", env.DefaultPointEntity },
                    { "DefaultBrushEntity", env.DefaultBrushEntity },
                    { "OverrideMapSize", Convert.ToString(env.OverrideMapSize, CultureInfo.InvariantCulture) },
                    { "MapSizeLow", Convert.ToString(env.MapSizeLow, CultureInfo.InvariantCulture) },
                    { "MapSizeHigh", Convert.ToString(env.MapSizeHigh, CultureInfo.InvariantCulture) },
                    { "IncludeFgdDirectoriesInEnvironment", Convert.ToString(env.IncludeFgdDirectoriesInEnvironment, CultureInfo.InvariantCulture) },

                    { "ToolsDirectory", env.ToolsDirectory },
                    { "IncludeToolsDirectoryInEnvironment", Convert.ToString(env.IncludeToolsDirectoryInEnvironment, CultureInfo.InvariantCulture) },
                    { "BspExe", env.BspExe },
                    { "CsgExe", env.CsgExe },
                    { "VisExe", env.VisExe },
                    { "RadExe", env.RadExe },

                    { "GameCopyBsp", Convert.ToString(env.GameCopyBsp, CultureInfo.InvariantCulture) },
                    { "GameRun", Convert.ToString(env.GameRun, CultureInfo.InvariantCulture) },
                    { "GameAsk", Convert.ToString(env.GameAsk, CultureInfo.InvariantCulture) },

                    { "MapCopyBsp", Convert.ToString(env.MapCopyBsp, CultureInfo.InvariantCulture) },
                    { "MapCopyMap", Convert.ToString(env.MapCopyMap, CultureInfo.InvariantCulture) },
                    { "MapCopyLog", Convert.ToString(env.MapCopyLog, CultureInfo.InvariantCulture) },
                    { "MapCopyErr", Convert.ToString(env.MapCopyErr, CultureInfo.InvariantCulture) },
                    { "MapCopyRes", Convert.ToString(env.MapCopyRes, CultureInfo.InvariantCulture) },

                    { "DefaultTextureScale", Convert.ToString(env.DefaultTextureScale, CultureInfo.InvariantCulture) },

                    { "ExcludedWads", String.Join(";", env.ExcludedWads) },
                    { "AdditionalTextureFiles", String.Join(";", env.AdditionalTextureFiles) }
                }
            };
            return se;
        }

        public IEnvironment CreateEnvironment()
        {
            return new GoldsourceEnvironment();
        }

        public IEnvironmentEditor CreateEditor()
        {
            return new GoldsourceEnvironmentEditor();
        }
    }
}