using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Settings;

namespace Sledge.Editor.Rendering.Converters
{
    public class GridConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return obj is World;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            smo.SceneObjects.Add(new Holder(), new GridElement(document));
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
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
            public int Low { get; private set; }
            public int High { get; private set; }
            public float Step { get; private set; }
            public bool ShowGrid { get; set; }
            public override string ElementGroup { get { return "Grid"; } }

            public GridElement(Document doc) : base(PositionType.World)
            {
                CameraFlags = CameraFlags.Orthographic;
                Update(doc);
            }

            public void Update(Document doc)
            {
                Low = doc.GameData.MapSizeLow;
                High = doc.GameData.MapSizeHigh;
                Step = (float) doc.Map.GridSpacing;
                ShowGrid = doc.Map.Show2DGrid;
                ClearValue("Validated");
            }

            private float GetActualStep(IViewport viewport)
            {
                var step = Step;
                var actualDist = step * viewport.Camera.Zoom;
                if (Grid.HideSmallerOn)
                {
                    while (actualDist < Grid.HideSmallerThan)
                    {
                        step *= Grid.HideFactor;
                        actualDist *= Grid.HideFactor;
                    }
                }
                return step;
            }

            public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
            {
                if (!GetValue<bool>(viewport, "Validated")) return true;
                if (Math.Abs(GetValue(viewport, "ActualStep", 0f) - GetActualStep(viewport)) > 0.001) return true;
                if (GetValue<bool>(viewport, "ShowGrid") != ShowGrid) return true;

                var bounds = GetValue<RectangleF>(viewport, "Bounds");
                var newBounds = GetValidatedBounds(viewport, 0);
                if (!bounds.Contains(newBounds)) return true;

                return false;
            }

            public override void Validate(IViewport viewport, IRenderer renderer)
            {
                SetValue(viewport, "Validated", true);
                SetValue(viewport, "ActualStep", GetActualStep(viewport));
                SetValue(viewport, "ShowGrid", ShowGrid);
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
                if (!ShowGrid) yield break;

                var oc = viewport.Camera as OrthographicCamera;
                if (oc == null) yield break;

                var lower = Low;
                var upper = High;
                var step = GetActualStep(viewport);

                var bounds = GetValidatedBounds(viewport, Padding);

                var unused = Vector3.One - viewport.Camera.Expand(new Vector3(1, 1, 0));
                var bottom = unused * Low;

                for (float f = lower; f <= upper; f += step)
                {
                    if ((f < bounds.Left || f > bounds.Right) && (f < bounds.Top || f > bounds.Bottom)) continue;
                    var i = (int) f;
                    var c = Grid.GridLines;
                    if (i == 0) c = Grid.ZeroLines;
                    else if (i % Grid.Highlight2UnitNum == 0 && Grid.Highlight2On) c = Grid.Highlight2;
                    else if (i % (int) (step * Grid.Highlight1LineNum) == 0 && Grid.Highlight1On) c = Grid.Highlight1;

                    yield return new LineElement(PositionType.World, c, new List<Position>
                    {
                        new Position(viewport.Camera.Expand(new Vector3(lower, f, 0)) + bottom),
                        new Position(viewport.Camera.Expand(new Vector3(upper, f, 0)) + bottom)
                    }) { Smooth = false, ZIndex = -10, DepthTested = true };

                    yield return new LineElement(PositionType.World, c, new List<Position>
                    {
                        new Position(viewport.Camera.Expand(new Vector3(f, lower, 0)) + bottom),
                        new Position(viewport.Camera.Expand(new Vector3(f, upper, 0)) + bottom)
                    }) { Smooth = false, ZIndex = -10, DepthTested = true };
                }
            }

            public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
            {
                yield break;
            }
        }
    }
}