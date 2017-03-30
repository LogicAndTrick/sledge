using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Hooks;
using Sledge.Common.Settings;
using Sledge.Rendering;
using Sledge.Rendering.OpenGL;

namespace Sledge.BspEditor.Components
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
                    _engine = new Lazy<Engine>(() => new Engine(new OpenGLRenderer()));
                    break;
            }
        }

        public IEnumerable<SettingValue> GetValues()
        {
            yield return new SettingValue("Renderer", _renderer);
        }
    }
}
