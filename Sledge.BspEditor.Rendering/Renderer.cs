using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Hooks;
using Sledge.Common.Settings;
using Sledge.Rendering;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL;

namespace Sledge.BspEditor.Rendering
{
    /// <summary>
    /// Bootstraps a renderer instance
    /// </summary>
    [Export(typeof(ISettingsContainer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Renderer : ISettingsContainer, IInitialiseHook
    {
        public async Task OnInitialise()
        {
            // Just here to make sure this gets initialised
        }
        
        public static Renderer Instance { get; private set; }

        private string _renderer = "OpenGLRenderer";
        private Lazy<Engine> _engine;

        public Engine Engine => _engine.Value;

        // Renderer settings
        private bool DisableTextureTransparency { get; set; } = false;
        private bool DisableTextureFiltering { get; set; } = false;
        private bool ForcePowerOfTwoTextureSizes { get; set; } = false;
        private Color PerspectiveBackgroundColour { get; set; } = Color.Black;
        private Color OrthographicBackgroundColour { get; set; } = Color.Black;
        private float PerspectiveGridSpacing { get; set; } = 64;
        private bool ShowPerspectiveGrid { get; set; } = false;
        private float PointSize { get; set; } = 4;

        public Renderer()
        {
            Instance = this;
        }

        // Settings container

        public string Name => "Sledge.Rendering.Renderer";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Renderer", "The 3D renderer", typeof(string));
        }

        public void SetValues(IEnumerable<SettingValue> values)
        {
            var d = values.ToDictionary(x => x.Name, x => x.Value);
            if (!d.ContainsKey("Renderer")) d["Renderer"] = "OpenGLRenderer";

            switch (d["Renderer"])
            {
                // Uh... we only support one renderer for now
                // So why am I bothering with this? oh well...
                default:
                case "OpenGLRenderer":
                    _renderer = "OpenGLRenderer";
                    _engine = new Lazy<Engine>(() => MakeEngine(new OpenGLRenderer()));
                    break;
            }
            
            bool b;
            int i;
            float f;
            if (d.ContainsKey("DisableTextureTransparency") && bool.TryParse(d["DisableTextureTransparency"], out b))
                DisableTextureTransparency = b;
            if (d.ContainsKey("DisableTextureFiltering") && bool.TryParse(d["DisableTextureFiltering"], out b))
                DisableTextureFiltering = b;
            if (d.ContainsKey("ForcePowerOfTwoTextureSizes") && bool.TryParse(d["ForcePowerOfTwoTextureSizes"], out b))
                ForcePowerOfTwoTextureSizes = b;
            if (d.ContainsKey("ShowPerspectiveGrid") && bool.TryParse(d["ShowPerspectiveGrid"], out b))
                ShowPerspectiveGrid = b;
            if (d.ContainsKey("PerspectiveGridSpacing") && float.TryParse(d["PerspectiveGridSpacing"], out f))
                PerspectiveGridSpacing = f;
            if (d.ContainsKey("PerspectiveBackgroundColour") && int.TryParse(d["PerspectiveBackgroundColour"], out i))
                PerspectiveBackgroundColour = Color.FromArgb(i);
            if (d.ContainsKey("OrthographicBackgroundColour") && int.TryParse(d["PerspectiveBackgroundColour"], out i))
                OrthographicBackgroundColour = Color.FromArgb(i);
            if (d.ContainsKey("PointSize") && int.TryParse(d["PerspectiveBackgroundColour"], out i))
                PointSize = i;
        }

        private Engine MakeEngine(IRenderer renderer)
        {
            var e = new Engine(renderer);

            var settings = e.Renderer.Settings;
            settings.DisableTextureTransparency = DisableTextureTransparency;
            settings.DisableTextureFiltering = DisableTextureFiltering;
            settings.ForcePowerOfTwoTextureSizes = ForcePowerOfTwoTextureSizes;
            settings.ShowPerspectiveGrid = ShowPerspectiveGrid;
            settings.PerspectiveGridSpacing = PerspectiveGridSpacing;
            settings.PerspectiveBackgroundColour = PerspectiveBackgroundColour;
            settings.OrthographicBackgroundColour = OrthographicBackgroundColour;
            settings.PointSize = PointSize;

            return e;
        }

        public IEnumerable<SettingValue> GetValues()
        {
            yield return new SettingValue("Renderer", _renderer);
            var settings = _engine.IsValueCreated ? _engine.Value.Renderer.Settings : null;
            yield return new SettingValue("DisableTextureTransparency", Convert.ToString(settings?.DisableTextureTransparency ?? DisableTextureTransparency, CultureInfo.InvariantCulture));
            yield return new SettingValue("DisableTextureFiltering", Convert.ToString(settings?.DisableTextureFiltering ?? DisableTextureFiltering, CultureInfo.InvariantCulture));
            yield return new SettingValue("ForcePowerOfTwoTextureSizes", Convert.ToString(settings?.ForcePowerOfTwoTextureSizes ?? ForcePowerOfTwoTextureSizes, CultureInfo.InvariantCulture));
            yield return new SettingValue("ShowPerspectiveGrid", Convert.ToString(settings?.ShowPerspectiveGrid ?? ShowPerspectiveGrid, CultureInfo.InvariantCulture));
            yield return new SettingValue("PerspectiveBackgroundColour", Convert.ToString((settings?.PerspectiveBackgroundColour ?? PerspectiveBackgroundColour).ToArgb(), CultureInfo.InvariantCulture));
            yield return new SettingValue("PerspectiveGridSpacing", Convert.ToString(settings?.PerspectiveGridSpacing ?? PerspectiveGridSpacing, CultureInfo.InvariantCulture));
            yield return new SettingValue("OrthographicBackgroundColour", Convert.ToString((settings?.OrthographicBackgroundColour ?? OrthographicBackgroundColour).ToArgb(), CultureInfo.InvariantCulture));
            yield return new SettingValue("PointSize", Convert.ToString(settings?.PointSize ?? PointSize, CultureInfo.InvariantCulture));
        }
    }
}
