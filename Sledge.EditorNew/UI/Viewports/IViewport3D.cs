using Sledge.DataStructures.Geometric;
using Sledge.Graphics;

namespace Sledge.EditorNew.UI.Viewports
{
    public interface IViewport3D : IMapViewport
    {
        ViewType Type { get; set; }
        Camera Camera { get; }
        void FocusOn(Coordinate coordinate, Coordinate distance);
        Line CastRayFromScreen(int x, int y);
    }
}