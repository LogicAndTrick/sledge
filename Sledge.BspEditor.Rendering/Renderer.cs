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
        [Setting] public static Color PerspectiveBackgroundColour { get; set; } = Color.Black;
        [Setting] public static Color OrthographicBackgroundColour { get; set; } = Color.Black;

        [Setting] public static Color FractionalGridLineColour { get; set; } = Color.FromArgb(32, 32, 32);
        [Setting] public static Color StandardGridLineColour { get; set; } = Color.FromArgb(75, 75, 75);
        [Setting] public static Color PrimaryGridLineColour { get; set; } = Color.FromArgb(115, 115, 115);
        [Setting] public static Color SecondaryGridLineColour { get; set; } = Color.FromArgb(100, 46, 0);
        [Setting] public static Color AxisGridLineColour { get; set; } = Color.FromArgb(0, 100, 100);
        [Setting] public static Color BoundaryGridLineColour { get; set; } = Color.Red;

        // Settings container

        public string Name => "Sledge.BspEditor.Rendering.Renderer";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Rendering", "PerspectiveBackgroundColour", typeof(Color));
            yield return new SettingKey("Rendering", "OrthographicBackgroundColour", typeof(Color));

            yield return new SettingKey("Rendering/Grid", "FractionalGridLineColour", typeof(Color));
            yield return new SettingKey("Rendering/Grid", "StandardGridLineColour", typeof(Color));
            yield return new SettingKey("Rendering/Grid", "PrimaryGridLineColour", typeof(Color));
            yield return new SettingKey("Rendering/Grid", "SecondaryGridLineColour", typeof(Color));
            yield return new SettingKey("Rendering/Grid", "AxisGridLineColour", typeof(Color));
            yield return new SettingKey("Rendering/Grid", "BoundaryGridLineColour", typeof(Color));
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
