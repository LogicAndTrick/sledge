using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(ISettingsContainer))]
    public class CameraNavigataionViewportSettings : ISettingsContainer
    {
        [Setting] public static bool Camera2DPanRequiresMouseClick { get; set; } = false;
        [Setting] public static bool Camera3DPanRequiresMouseClick { get; set; } = false;

        [Setting] public static int ForwardSpeed { get; set; } = 1000;
        [Setting] public static decimal TimeToTopSpeed { get; set; } = 0.5m;
        [Setting] public static decimal MouseWheelMoveDistance { get; set; } = 500;
        [Setting] public static bool InvertX { get; set; } = false;
        [Setting] public static bool InvertY { get; set; } = false;

        public string Name => "Sledge.BspEditor.Rendering.ViewportCameraNavigataionSettings";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Camera2DPanRequiresMouseClick", typeof(bool));
            yield return new SettingKey("Camera3DPanRequiresMouseClick", typeof(bool));
            yield return new SettingKey("ForwardSpeed", typeof(int));
            yield return new SettingKey("TimeToTopSpeed", typeof(decimal));
            yield return new SettingKey("MouseWheelMoveDistance", typeof(decimal));
            yield return new SettingKey("InvertX", typeof(bool));
            yield return new SettingKey("InvertY", typeof(bool));
        }

        public void LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }
    }
}