using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;
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
            yield return new SettingKey("Renderer", typeof(string));
        }

        public void SetValues(ISettingsStore store)
        {
            switch (store.Get("Renderer", "OpenGLRenderer"))
            {
                // Uh... we only support one renderer for now
                // So why am I bothering with this? oh well...
                default:
                case "OpenGLRenderer":
                    _renderer = "OpenGLRenderer";
                    _engine = new Lazy<Engine>(() => MakeEngine(new OpenGLRenderer()));
                    break;
            }

            DisableTextureTransparency = store.Get("DisableTextureTransparency", DisableTextureTransparency);
            DisableTextureFiltering = store.Get("DisableTextureFiltering", DisableTextureFiltering);
            ForcePowerOfTwoTextureSizes = store.Get("ForcePowerOfTwoTextureSizes", ForcePowerOfTwoTextureSizes);
            ShowPerspectiveGrid = store.Get("ShowPerspectiveGrid", ShowPerspectiveGrid);
            PerspectiveGridSpacing = store.Get("PerspectiveGridSpacing", PerspectiveGridSpacing);
            PerspectiveBackgroundColour = store.Get("PerspectiveBackgroundColour", PerspectiveBackgroundColour);
            OrthographicBackgroundColour = store.Get("OrthographicBackgroundColour", OrthographicBackgroundColour);
            PointSize = store.Get("PointSize", PointSize);
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
            yield return new SettingValue("DisableTextureTransparency", settings?.DisableTextureTransparency ?? DisableTextureTransparency);
            yield return new SettingValue("DisableTextureFiltering", settings?.DisableTextureFiltering ?? DisableTextureFiltering);
            yield return new SettingValue("ForcePowerOfTwoTextureSizes", settings?.ForcePowerOfTwoTextureSizes ?? ForcePowerOfTwoTextureSizes);
            yield return new SettingValue("ShowPerspectiveGrid", settings?.ShowPerspectiveGrid ?? ShowPerspectiveGrid);
            yield return new SettingValue("PerspectiveBackgroundColour", (settings?.PerspectiveBackgroundColour ?? PerspectiveBackgroundColour).ToArgb());
            yield return new SettingValue("PerspectiveGridSpacing", settings?.PerspectiveGridSpacing ?? PerspectiveGridSpacing);
            yield return new SettingValue("OrthographicBackgroundColour", (settings?.OrthographicBackgroundColour ?? OrthographicBackgroundColour).ToArgb());
            yield return new SettingValue("PointSize", settings?.PointSize ?? PointSize);
        }
    }
}
