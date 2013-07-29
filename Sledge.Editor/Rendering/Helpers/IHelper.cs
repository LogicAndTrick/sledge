using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    public interface IHelper
    {
        bool Is2DHelper { get; }
        bool Is3DHelper { get; }
        bool IsDocumentHelper { get; }
        HelperType HelperType { get; }
        bool IsValidFor(MapObject o);
        void Render2D(Viewport2D viewport, MapObject o);
        void Render3D(Viewport3D viewport, MapObject o);
        void RenderDocument(ViewportBase viewport, Document document);
    }
}