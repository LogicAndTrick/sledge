using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
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

        private RenderEngine _renderer = RenderEngine.OpenGLRenderer;
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
            yield return new SettingKey("Rendering", "Renderer", typeof(RenderEngine));
            yield return new SettingKey("Rendering", "DisableTextureTransparency", typeof(bool));
            yield return new SettingKey("Rendering", "DisableTextureFiltering", typeof(bool));
            yield return new SettingKey("Rendering", "ForcePowerOfTwoTextureSizes", typeof(bool));
            yield return new SettingKey("Rendering", "ShowPerspectiveGrid", typeof(bool));
            yield return new SettingKey("Rendering", "PerspectiveGridSpacing", typeof(decimal));
            yield return new SettingKey("Rendering", "PerspectiveBackgroundColour", typeof(Color));
            yield return new SettingKey("Rendering", "OrthographicBackgroundColour", typeof(Color));
            yield return new SettingKey("Rendering", "PointSize", typeof(decimal));
        }

        public void LoadValues(ISettingsStore store)
        {
            var render = Enum.TryParse(store.Get("Renderer", "OpenGLRenderer"), out RenderEngine r) ? r : RenderEngine.OpenGLRenderer;
            switch (render)
            {
                // Uh... we only support one renderer for now
                // So why am I bothering with this? oh well...
                default:
                case RenderEngine.OpenGLRenderer:
                    _renderer = RenderEngine.OpenGLRenderer;
                    _engine = new Lazy<Engine>(() => MakeEngine(new OpenGLRenderer()));
                    break;
            }
            store.LoadInstance(this);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.Set("Renderer", Convert.ToString(_renderer));
            store.StoreInstance(this);
        }

        public enum RenderEngine
        {
            OpenGLRenderer
        }
    }
}
