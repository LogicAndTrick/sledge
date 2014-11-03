using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.EditorNew.UI.Viewports
{
    public interface IMapViewport
    {
        void FocusOn(Box box);
        void FocusOn(Coordinate coordinate);
        Matrix4 GetViewportMatrix();
        Matrix4 GetCameraMatrix();
        Matrix4 GetModelViewMatrix();
        Coordinate ScreenToWorld(Coordinate screen);
        Coordinate WorldToScreen(Coordinate world);
    }
}