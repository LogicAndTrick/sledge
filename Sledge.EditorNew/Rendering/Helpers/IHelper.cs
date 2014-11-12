using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Rendering.Helpers
{
    public interface IHelper
    {
        Document Document { get; set; }
        bool Is2DHelper { get; }
        bool Is3DHelper { get; }
        bool IsDocumentHelper { get; }
        HelperType HelperType { get; }
        bool IsValidFor(MapObject o);
        void BeforeRender2D(IViewport2D viewport);
        void Render2D(IViewport2D viewport, MapObject o);
        void AfterRender2D(IViewport2D viewport);
        void BeforeRender3D(IViewport3D viewport);
        void Render3D(IViewport3D viewport, MapObject o);
        void AfterRender3D(IViewport3D viewport);
        void RenderDocument(IMapViewport viewport, Document document);
        IEnumerable<MapObject> Order(IMapViewport viewport, IEnumerable<MapObject> mapObjects);
    }
}