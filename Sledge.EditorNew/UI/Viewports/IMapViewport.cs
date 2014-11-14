using System;
using OpenTK;
using OpenTK.Graphics;
using Sledge.DataStructures.Geometric;
using Sledge.Graphics;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.EditorNew.UI.Viewports
{
    public delegate void ListenerExceptionEventHandler(object sender, Exception exception);

    public interface IMapViewport : IViewport
    {
        event ListenerExceptionEventHandler ListenerException;
        RenderContext RenderContext { get; set; }
        bool Is3D { get; set; }
        bool Is2D { get; set; }
        int Width { get; }
        int Height { get; }
        void FocusOn(Box box);
        void FocusOn(Coordinate coordinate);
        Matrix4 GetViewportMatrix();
        Matrix4 GetCameraMatrix();
        Matrix4 GetModelViewMatrix();
        Coordinate ScreenToWorld(Coordinate screen);
        Coordinate WorldToScreen(Coordinate world);
    }

    public static class ViewportExtensions
    {
        public static Coordinate ScreenToWorld(this IMapViewport vp, decimal x, decimal y)
        {
            return vp.ScreenToWorld(new Coordinate(x, y, 0));
        }

        public static Coordinate WorldToScreen(this IMapViewport vp, decimal x, decimal y, decimal z)
        {
            return vp.WorldToScreen(new Coordinate(x, y, z));
        }
    }
}