using SharpDX.Direct3D;

namespace Sledge.Rendering.Engine
{
    public static class Features
    {
        public static FeatureLevel FeatureLevel { get; set; }

        public static bool IndirectBuffers => DirectX11OrHigher;

        public static bool DirectX10 => FeatureLevel >= FeatureLevel.Level_10_0 && FeatureLevel < FeatureLevel.Level_11_0;
        public static bool DirectX11 => FeatureLevel >= FeatureLevel.Level_11_0 && FeatureLevel < FeatureLevel.Level_12_0;
        public static bool DirectX10OrHigher => FeatureLevel >= FeatureLevel.Level_10_0;
        public static bool DirectX11OrHigher => FeatureLevel >= FeatureLevel.Level_11_0;
    }
}
