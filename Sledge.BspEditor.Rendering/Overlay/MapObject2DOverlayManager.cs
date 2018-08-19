using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    [Export(typeof(IMapDocumentOverlayRenderable))]
    public class MapObject2DOverlayManager : IMapDocumentOverlayRenderable
    {
        [ImportMany] private IMapObject2DOverlay[] _overlays;

        private readonly WeakReference<MapDocument> _document = new WeakReference<MapDocument>(null);

        public void SetActiveDocument(MapDocument doc)
        {
            _document.SetTarget(doc);
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            if (_overlays.Length == 0) return;
            if (!_document.TryGetTarget(out var doc)) return;

            // Determine which objects are visible
            var padding = Vector3.One * 100;
            var box = new Box(worldMin - padding, worldMax + padding);
            var objects = doc.Map.Root.Find(x => x.BoundingBox.IntersectsWith(box)).ToList();

            // Render the overlay for each object
            foreach (var overlay in _overlays)
            {
                overlay.Render(viewport, objects, camera, worldMin, worldMax, graphics);
            }
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            // 2D only
        }
    }
}