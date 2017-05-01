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
        [Setting] private bool DisableTextureTransparency { get; set; } = false;
        [Setting] private bool DisableTextureFiltering { get; set; } = false;
        [Setting] private bool ForcePowerOfTwoTextureSizes { get; set; } = false;
        [Setting] private Color PerspectiveBackgroundColour { get; set; } = Color.Black;
        [Setting] private Color OrthographicBackgroundColour { get; set; } = Color.Black;
        [Setting] private float PerspectiveGridSpacing { get; set; } = 64;
        [Setting] private bool ShowPerspectiveGrid { get; set; } = false;
        [Setting] private float PointSize { get; set; } = 4;

        public Renderer()
        {
            Instance = this;
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

        // Settings container

        public string Name => "Sledge.Rendering.Renderer";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Renderer", typeof(string));
        }

        public void LoadValues(ISettingsStore store)
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
            store.LoadInstance(this);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.Set("Renderer", _renderer);
            store.StoreInstance(this);
        }
    }
}
