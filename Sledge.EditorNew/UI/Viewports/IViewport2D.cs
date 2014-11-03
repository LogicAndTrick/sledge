using Sledge.DataStructures.Geometric;

namespace Sledge.EditorNew.UI.Viewports
{
    public interface IViewport2D : IMapViewport
    {
        ViewDirection Direction { get; set; }
        Coordinate Position { get; set; }
        decimal Zoom { get; set; }
        Coordinate Flatten(Coordinate c);
        Coordinate Expand(Coordinate c);
        Coordinate GetUnusedCoordinate(Coordinate c);
        Coordinate ZeroUnusedCoordinate(Coordinate c);
        decimal UnitsToPixels(decimal units);
        decimal PixelsToUnits(decimal pixels);
    }
}