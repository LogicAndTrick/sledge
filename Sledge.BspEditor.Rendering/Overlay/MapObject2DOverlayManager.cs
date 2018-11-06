using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using ImGuiNET;
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
        private readonly IEnumerable<Lazy<IMapObject2DOverlay>> _overlays;

        private readonly WeakReference<MapDocument> _document = new WeakReference<MapDocument>(null);

        [ImportingConstructor]
        public MapObject2DOverlayManager(
            [ImportMany] IEnumerable<Lazy<IMapObject2DOverlay>> overlays
        )
        {
            _overlays = overlays;
        }

        public void SetActiveDocument(MapDocument doc)
        {
            _document.SetTarget(doc);
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, ImDrawListPtr im)
        {
            if (!_overlays.Any()) return;
            if (!_document.TryGetTarget(out var doc)) return;

            // Determine which objects are visible
            var padding = Vector3.One * 100;
            var box = new Box(worldMin - padding, worldMax + padding);
            var objects = doc.Map.Root.Find(x => x.BoundingBox.IntersectsWith(box)).ToList();

            // Render the overlay for each object
            foreach (var overlay in _overlays)
            {
                overlay.Value.Render(viewport, objects, camera, worldMin, worldMax, im);
            }
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, ImDrawListPtr im)
        {
            // 2D only
        }
    }
}