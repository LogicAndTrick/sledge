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
}