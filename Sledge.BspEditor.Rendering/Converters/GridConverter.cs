using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class GridConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLow;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Root;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            smo.SceneObjects.Add(new Holder(), new GridElement(document));
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            if (smo.SceneObjects.Keys.Any(x => x is Holder))
            {
                var ela = smo.SceneObjects.First(x => x.Key is Holder).Value as GridElement;
                if (ela != null)
                {
                    ela.Update(document);
                    return true;
                }
            }
            return false;
        }

        private class Holder { }

        public class GridElement : Element
        {
            private IGrid _grid;
            public override string ElementGroup => "Grid";

            public GridElement(MapDocument doc) : base(PositionType.World)
            {
                CameraFlags = CameraFlags.Orthographic;
                Update(doc);
            }

            public void Update(MapDocument doc)
            {
                ClearValue("Validated");
                _grid = doc.Map.Data.Get<GridData>().FirstOrDefault()?.Grid;
            }

            // todo !settings move grid colours to settings
            private static Color GridLines { get; set; } = Color.FromArgb(75, 75, 75);
            private static Color ZeroLines { get; set; } = Color.FromArgb(0, 100, 100);
            private static Color BoundaryLines { get; set; } = Color.Red;
            private static Color Highlight1 { get; set; } = Color.FromArgb(115, 115, 115);
            private static Color Highlight2 { get; set; } = Color.FromArgb(100, 46, 0);

            private Color GetColorForGridLineType(GridLineType type)
            {
                switch (type)
                {
                    case GridLineType.Standard:
                        return GridLines;
                    case GridLineType.Axis:
                        return ZeroLines;
                    case GridLineType.Primary:
                        return Highlight1;
                    case GridLineType.Secondary:
                        return Highlight2;
                    case GridLineType.Boundary:
                        return BoundaryLines;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
            
            public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
            {
                if (!GetValue<bool>(viewport, "Validated")) return true;
                if (Math.Abs(GetValue(viewport, "Scale", 1f) - viewport.Camera.Zoom) > 0.001) return true;

                var bounds = GetValue<RectangleF>(viewport, "Bounds");
                var newBounds = GetValidatedBounds(viewport, 0);
                if (!bounds.Contains(newBounds)) return true;

                return false;
            }

            public override void Validate(IViewport viewport, IRenderer renderer)
            {
                SetValue(viewport, "Validated", true);
                SetValue(viewport, "Scale", viewport.Camera.Zoom);
                SetValue(viewport, "Bounds", GetValidatedBounds(viewport, Padding));
            }

            private const int Padding = 50;

            private RectangleF GetValidatedBounds(IViewport viewport, int padding)
            {
                var vmin = viewport.Camera.Flatten(viewport.Camera.ScreenToWorld(new Vector3(-padding, viewport.Control.Height + padding, 0), viewport.Control.Width, viewport.Control.Height));
                var vmax = viewport.Camera.Flatten(viewport.Camera.ScreenToWorld(new Vector3(viewport.Control.Width + padding, -padding, 0), viewport.Control.Width, viewport.Control.Height));
                return new RectangleF(vmin.X, vmin.Y, vmax.X - vmin.X, vmax.Y - vmin.Y);
            }

            public override IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer)
            {
                var oc = viewport.Camera as OrthographicCamera;
                if (oc == null) yield break;

                if (_grid == null) yield break;

                var padding = Padding;
                var vmin = viewport.Camera.ScreenToWorld(new Vector3(-padding, viewport.Control.Height + padding, 0), viewport.Control.Width, viewport.Control.Height);
                var vmax = viewport.Camera.ScreenToWorld(new Vector3(viewport.Control.Width + padding, -padding, 0), viewport.Control.Width, viewport.Control.Height);
                var normal = Coordinate.One - viewport.Camera.Expand(new Vector3(1, 1, 0)).ToCoordinate();

                foreach (var line in _grid.GetLines(normal, (decimal) viewport.Camera.Zoom, vmin.ToCoordinate(), vmax.ToCoordinate()))
                {
                    var c = GetColorForGridLineType(line.Type);
                    yield return new LineElement(PositionType.World, c,
                        new List<Position>
                        {
                            new Position(line.Line.Start.ToVector3()),
                            new Position(line.Line.End.ToVector3())
                        })
                    {
                        Smooth = false,
                        ZIndex = -10,
                        DepthTested = true
                    };
                }
            }

            public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
            {
                yield break;
            }
        }
    }
}