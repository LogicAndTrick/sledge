using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    public interface IHelper
    {
        Document Document { get; set; }
        bool Is2DHelper { get; }
        bool Is3DHelper { get; }
        bool IsDocumentHelper { get; }
        HelperType HelperType { get; }
        bool IsValidFor(MapObject o);
        void BeforeRender2D(Viewport2D viewport);
        void Render2D(Viewport2D viewport, MapObject o);
        void AfterRender2D(Viewport2D viewport);
        void BeforeRender3D(Viewport3D viewport);
        void Render3D(Viewport3D viewport, MapObject o);
        void AfterRender3D(Viewport3D viewport);
        void RenderDocument(ViewportBase viewport, Document document);
        IEnumerable<MapObject> Order(ViewportBase viewport, IEnumerable<MapObject> mapObjects);
    }
}