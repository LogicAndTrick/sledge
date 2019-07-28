using System.Collections.Generic;
using System.ComponentModel.Composition;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(ISettingsContainer))]
    public class CameraNavigationViewportSettings : ISettingsContainer
    {
        [Setting] public static bool Camera2DPanRequiresMouseClick { get; set; } = false;
        [Setting] public static bool Camera3DPanRequiresMouseClick { get; set; } = false;

        [Setting] public static int ForwardSpeed { get; set; } = 1000;
        [Setting] public static decimal TimeToTopSpeed { get; set; } = 0.5m;
        [Setting] public static decimal MouseWheelMoveDistance { get; set; } = 500;
        [Setting] public static decimal MouseWheelZoomMultiplier { get; set; } = 1.2m;
        [Setting] public static int FOV { get; set; } = 60;
        [Setting] public static bool InvertX { get; set; } = false;
        [Setting] public static bool InvertY { get; set; } = false;
        [Setting] public static decimal Sensitivity { get; set; } = 5;

        public string Name => "Sledge.BspEditor.Rendering.CameraNavigationViewportSettings";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Navigation/2D", "Camera2DPanRequiresMouseClick", typeof(bool));
            yield return new SettingKey("Navigation/3D", "Camera3DPanRequiresMouseClick", typeof(bool));
            yield return new SettingKey("Navigation/3D", "FOV", typeof(int)) { EditorType = "Slider", EditorHint = "20,100,5,10,0.5" };
            yield return new SettingKey("Navigation/3D", "ForwardSpeed", typeof(int)) { EditorType = "Slider", EditorHint = "100,500,10,50,0.1" };
            yield return new SettingKey("Navigation/3D", "TimeToTopSpeed", typeof(decimal)) { EditorType = "Slider", EditorHint = "0,5,0.1,1,10" };
            yield return new SettingKey("Navigation/3D", "MouseWheelMoveDistance", typeof(decimal)) { EditorType = "Slider", EditorHint = "1,200,1,10,0.1" };
            yield return new SettingKey("Navigation/2D", "MouseWheelZoomMultiplier", typeof(decimal));
            yield return new SettingKey("Navigation/3D", "InvertX", typeof(bool));
            yield return new SettingKey("Navigation/3D", "InvertY", typeof(bool));
            yield return new SettingKey("Navigation/3D", "Sensitivity", typeof(decimal));

        }

        public void LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
            Oy.Publish("MapDocument:Viewport:SetFOV", FOV);
        }
    }
}