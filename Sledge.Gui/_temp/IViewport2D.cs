using Sledge.DataStructures.Geometric;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Interfaces.Viewports
{
    public interface IViewport2D : IViewport
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

        /*

        void FocusOn(Box box);
        void FocusOn(Coordinate coordinate);
        Matrix4 GetViewportMatrix();
        Matrix4 GetCameraMatrix();
        Matrix4 GetModelViewMatrix();
        Coordinate ScreenToWorld(Coordinate screen);
        Coordinate WorldToScreen(Coordinate world);
        
         */
    }
}