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
        public static bool Camera2DPanRequiresMouseClick { get; set; } = false;
        public static bool Camera3DPanRequiresMouseClick { get; set; } = false;

        public static int ForwardSpeed { get; set; } = 1000;
        public static decimal TimeToTopSpeed { get; set; } = 0.5m;
        public static decimal MouseWheelMoveDistance { get; set; } = 500;
        public static bool InvertX { get; set; } = false;
        public static bool InvertY { get; set; } = false;

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

        public void SetValues(ISettingsStore store)
        {
            Camera2DPanRequiresMouseClick = store.Get("Camera2DPanRequiresMouseClick", Camera2DPanRequiresMouseClick);
            Camera3DPanRequiresMouseClick = store.Get("Camera3DPanRequiresMouseClick", Camera3DPanRequiresMouseClick);
            ForwardSpeed = store.Get("ForwardSpeed", ForwardSpeed);
            TimeToTopSpeed = store.Get("TimeToTopSpeed", TimeToTopSpeed);
            MouseWheelMoveDistance = store.Get("MouseWheelMoveDistance", MouseWheelMoveDistance);
            InvertX = store.Get("InvertX", InvertX);
            InvertY = store.Get("InvertY", InvertY);
        }

        public IEnumerable<SettingValue> GetValues()
        {
            yield return new SettingValue("Camera2DPanRequiresMouseClick", Camera2DPanRequiresMouseClick);
            yield return new SettingValue("Camera3DPanRequiresMouseClick", Camera3DPanRequiresMouseClick);
            yield return new SettingValue("InvertX", InvertX);
            yield return new SettingValue("InvertY", InvertY);
            yield return new SettingValue("ForwardSpeed", ForwardSpeed);
            yield return new SettingValue("TimeToTopSpeed", TimeToTopSpeed);
            yield return new SettingValue("MouseWheelMoveDistance", MouseWheelMoveDistance);
        }
    }
}