using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;

namespace Sledge.BspEditor.Environment.Goldsource
{
    [Export(typeof(IEnvironmentFactory))]
    public class GoldsourceEnvironmentFactory : IEnvironmentFactory
    {
        public Type Type => typeof(GoldsourceEnvironment);
        public string TypeName => "GoldsourceEnvironment";
        public string Description { get; set; } = "Goldsource";

        private T GetVal<T>(Dictionary<string, string> dictionary, string key, T def = default(T))
        {
            if (dictionary.TryGetValue(key, out string val))
            {
                try
                {
                    return (T) Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
                }
                catch
                {
                    
                }
            }
            return def;
        }

        public IEnvironment Deserialise(SerialisedEnvironment environment)
        {
            var gse = new GoldsourceEnvironment
            {
                ID = environment.ID,
                Name = environment.Name,
                BaseDirectory = GetVal(environment.Properties, "BaseDirectory", ""),
                GameDirectory = GetVal(environment.Properties, "GameDirectory", ""),
                ModDirectory = GetVal(environment.Properties, "ModDirectory", ""),
                GameExe = GetVal(environment.Properties, "GameExe", ""),
                LoadHdModels = GetVal(environment.Properties, "LoadHdModels", true),

                FgdFiles = GetVal(environment.Properties, "FgdFiles", "").Split(';').ToList(),
                DefaultPointEntity = GetVal(environment.Properties, "DefaultPointEntity", ""),
                DefaultBrushEntity = GetVal(environment.Properties, "DefaultBrushEntity", ""),
                OverrideMapSize = GetVal(environment.Properties, "OverrideMapSize", false),
                MapSizeLow = GetVal(environment.Properties, "MapSizeLow", 0m),
                MapSizeHigh = GetVal(environment.Properties, "MapSizeHigh", 0m),
                IncludeFgdDirectoriesInEnvironment = GetVal(environment.Properties, "IncludeFgdDirectoriesInEnvironment", true),

                ToolsDirectory = GetVal(environment.Properties, "ToolsDirectory", ""),
                IncludeToolsDirectoryInEnvironment = GetVal(environment.Properties, "IncludeToolsDirectoryInEnvironment", true),
                BspExe = GetVal(environment.Properties, "BspExe", ""),
                CsgExe = GetVal(environment.Properties, "CsgExe", ""),
                VisExe = GetVal(environment.Properties, "VisExe", ""),
                RadExe = GetVal(environment.Properties, "RadExe", ""),

                DefaultTextureScale = GetVal(environment.Properties, "DefaultTextureScale", 0m)
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

                    { "DefaultTextureScale", Convert.ToString(env.DefaultTextureScale, CultureInfo.InvariantCulture) },
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