using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using Sledge.Common.Shell.Settings;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering
{
    /// <summary>
    /// Bootstraps a renderer instance
    /// </summary>
    [Export(typeof(ISettingsContainer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Renderer : ISettingsContainer
    {
        [Import] private Lazy<EngineInterface> _engine;

        // Renderer settings
        [Setting] private Color PerspectiveBackgroundColour { get; set; } = Color.Black;
        [Setting] private Color OrthographicBackgroundColour { get; set; } = Color.Black;

        // Settings container

        public string Name => "Sledge.BspEditor.Rendering.Renderer";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Rendering", "PerspectiveBackgroundColour", typeof(Color));
            yield return new SettingKey("Rendering", "OrthographicBackgroundColour", typeof(Color));
        }

        public void LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
            _engine.Value.SetClearColour(CameraType.Perspective, PerspectiveBackgroundColour);
            _engine.Value.SetClearColour(CameraType.Orthographic, OrthographicBackgroundColour);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }
    }
}
