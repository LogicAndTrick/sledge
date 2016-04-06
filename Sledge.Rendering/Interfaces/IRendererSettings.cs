using System.Drawing;

namespace Sledge.Rendering.Interfaces
{
    public interface IRendererSettings
    {
        bool DisableTextureTransparency { get; set; }
        bool DisableTextureFiltering { get; set; }
        bool ForcePowerOfTwoTextureSizes { get; set; }
        Color PerspectiveBackgroundColour { get; set; }
        Color OrthographicBackgroundColour { get; set; }
        float PerspectiveGridSpacing { get; set; }
        bool ShowPerspectiveGrid { get; set; }
        float PointSize { get; set; }
    }
}