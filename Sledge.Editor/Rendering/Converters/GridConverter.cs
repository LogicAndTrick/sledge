using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            smo.SceneObjects.Add(new Holder(), new GridElement(document));
            return true;
        }

        public bool Update(SceneMapObject smo, Document document, MapObject obj)
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
                return !GetValue<bool>(viewport, "Validated") || Math.Abs(GetValue(viewport, "ActualStep", 0f) - GetActualStep(viewport)) > 0.001;
            }

            public override void Validate(IViewport viewport, IRenderer renderer)
            {
                SetValue(viewport, "Validated", true);
                SetValue(viewport, "ActualStep", GetActualStep(viewport));
            }

            public override IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer)
            {
                var oc = viewport.Camera as OrthographicCamera;
                if (oc == null) yield break;

                var lower = Low;
                var upper = High;
                var step = GetActualStep(viewport);

                var vmin = viewport.Camera.Flatten(viewport.Camera.ScreenToWorld(new Vector3(0, viewport.Control.Height, 0), viewport.Control.Width, viewport.Control.Height));
                var vmax = viewport.Camera.Flatten(viewport.Camera.ScreenToWorld(new Vector3(viewport.Control.Width, 0, 0), viewport.Control.Width, viewport.Control.Height));

                var unused = Vector3.One - viewport.Camera.Expand(new Vector3(1, 1, 0));
                var bottom = unused * Low;

                for (float f = lower; f <= upper; f += step)
                {
                    //if ((f < vmin.X || f > vmax.X) && (f < vmin.Y || f > vmax.Y)) continue;
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