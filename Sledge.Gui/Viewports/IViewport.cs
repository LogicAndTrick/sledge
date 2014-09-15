using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.Gui.Viewports
{
    public interface IViewport
    {
        bool Is3D { get; }
        void FocusOn(Box box);
        void FocusOn(Coordinate coordinate);
        Matrix4 GetViewportMatrix();
        Matrix4 GetCameraMatrix();
        Matrix4 GetModelViewMatrix();
        Coordinate ScreenToWorld(Coordinate screen);
        Coordinate WorldToScreen(Coordinate world);
    }
}