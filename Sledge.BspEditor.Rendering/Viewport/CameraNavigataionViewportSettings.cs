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
            yield return new SettingKey("Camera2DPanRequiresMouseClick", "", typeof(bool));
            yield return new SettingKey("Camera3DPanRequiresMouseClick", "", typeof(bool));
            yield return new SettingKey("ForwardSpeed", "", typeof(int));
            yield return new SettingKey("TimeToTopSpeed", "", typeof(decimal));
            yield return new SettingKey("MouseWheelMoveDistance", "", typeof(decimal));
            yield return new SettingKey("InvertX", "", typeof(bool));
            yield return new SettingKey("InvertY", "", typeof(bool));
        }

        public void SetValues(IEnumerable<SettingValue> values)
        {
            var d = values.ToDictionary(x => x.Name, x => x.Value);
            bool b;
            int i;
            decimal m;
            if (d.ContainsKey("Camera2DPanRequiresMouseClick") && bool.TryParse(d["Camera2DPanRequiresMouseClick"], out b)) Camera2DPanRequiresMouseClick = b;
            if (d.ContainsKey("Camera3DPanRequiresMouseClick") && bool.TryParse(d["Camera3DPanRequiresMouseClick"], out b)) Camera3DPanRequiresMouseClick = b;
            if (d.ContainsKey("InvertX") && bool.TryParse(d["InvertX"], out b)) InvertX = b;
            if (d.ContainsKey("InvertY") && bool.TryParse(d["InvertY"], out b)) InvertY = b;
            if (d.ContainsKey("ForwardSpeed") && int.TryParse(d["ForwardSpeed"], out i)) ForwardSpeed = i;
            if (d.ContainsKey("TimeToTopSpeed") && decimal.TryParse(d["TimeToTopSpeed"], out m)) TimeToTopSpeed = m;
            if (d.ContainsKey("MouseWheelMoveDistance") && decimal.TryParse(d["MouseWheelMoveDistance"], out m)) MouseWheelMoveDistance = m;
        }

        public IEnumerable<SettingValue> GetValues()
        {
            yield return new SettingValue("Camera2DPanRequiresMouseClick", Convert.ToString(Camera2DPanRequiresMouseClick, CultureInfo.InvariantCulture));
            yield return new SettingValue("Camera3DPanRequiresMouseClick", Convert.ToString(Camera3DPanRequiresMouseClick, CultureInfo.InvariantCulture));
            yield return new SettingValue("InvertX", Convert.ToString(InvertX, CultureInfo.InvariantCulture));
            yield return new SettingValue("InvertY", Convert.ToString(InvertY, CultureInfo.InvariantCulture));
            yield return new SettingValue("ForwardSpeed", Convert.ToString(ForwardSpeed, CultureInfo.InvariantCulture));
            yield return new SettingValue("TimeToTopSpeed", Convert.ToString(TimeToTopSpeed, CultureInfo.InvariantCulture));
            yield return new SettingValue("MouseWheelMoveDistance", Convert.ToString(MouseWheelMoveDistance, CultureInfo.InvariantCulture));
        }
    }
}