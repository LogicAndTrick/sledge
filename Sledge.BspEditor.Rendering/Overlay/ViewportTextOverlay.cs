using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Sledge.BspEditor.Documents;
using Sledge.Common;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    [Export(typeof(IMapDocumentOverlayRenderable))]
    public class ViewportTextOverlay : IMapDocumentOverlayRenderable
    {
        public void SetActiveDocument(MapDocument doc)
        {
            //
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, ImDrawListPtr im)
        {
            var str = $"2D {camera.ViewType}";
            var size = ImGui.CalcTextSize(str);
            im.AddText(new Vector2(2, 2), Color.White.ToImGuiColor(), str);
            im.AddRectFilled(Vector2.Zero, size + new Vector2(4, 4), Color.FromArgb(128, Color.Pink).ToImGuiColor());
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, ImDrawListPtr im)
        {
            var str = $"3D View";
            var size = ImGui.CalcTextSize(str);
            im.AddText(new Vector2(2, 2), Color.White.ToImGuiColor(), str);
            im.AddRectFilled(Vector2.Zero, size + new Vector2(4, 4), Color.FromArgb(128, Color.Pink).ToImGuiColor());
        }
    }
}
