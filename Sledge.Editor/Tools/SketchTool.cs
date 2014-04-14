using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Brushes;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering.Immediate;
using Sledge.Settings;
using Sledge.UI;
using Select = Sledge.Settings.Select;

namespace Sledge.Editor.Tools
{
    public class SketchTool : BaseTool
    {
        public enum SketchState
        {
            None,
            Ready,
            DrawingBase,
            DrawingVolume
        }

        private SketchState _state;
        private Face _currentFace;
        private Face _cloneFace;
        private Coordinate _intersection;
        private Box _drawing;
        private Plane _volumePlane;

        public SketchTool()
        {
            Usage = ToolUsage.View3D;
        }

        public override void ToolSelected(bool preventHistory)
        {
            _state = SketchState.None;
            _currentFace = _cloneFace = null;
            _intersection = null;
            _drawing = null;
            _volumePlane = null;
        }

        public override void ToolDeselected(bool preventHistory)
        {
            _state = SketchState.None;
            _currentFace = _cloneFace = null;
            _intersection = null;
            _drawing = null;
            _volumePlane = null;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Translate;
        }

        public override string GetName()
        {
            return "Sketch Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Sketch;
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            // 
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            //
            switch (_state)
            {
                case SketchState.None:
                    // nothin
                    break;
                case SketchState.Ready:
                    if (e.Button != MouseButtons.Left) break;
                    _drawing = new Box(_intersection, _intersection);
                    _state = SketchState.DrawingBase;
                    break;
                case SketchState.DrawingBase:
                    if (e.Button == MouseButtons.Right)
                    {
                        // Cancel
                        _state = SketchState.None;
                        _drawing = null;
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        _drawing = new Box(_drawing.Start, _intersection);
                        _volumePlane = new Plane(new Coordinate(_drawing.End.X, _drawing.Start.Y, _drawing.Start.Z), _drawing.End, _drawing.End + _currentFace.Plane.Normal);
                        _state = SketchState.DrawingVolume;
                    }
                    break;
                case SketchState.DrawingVolume:
                    if (e.Button == MouseButtons.Right)
                    {
                        _state = SketchState.DrawingBase;
                        _volumePlane = null;
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        CreateBrush(new Box(new[] {_drawing.Start, _drawing.End}));
                        _drawing = null;
                        _volumePlane = null;
                        _state = SketchState.None;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CreateBrush(Box bounds)
        {
            var brush = GetBrush(bounds, Document.Map.IDGenerator);
            if (brush == null) return;
            IAction action = new Create(brush);
            if (Select.SelectCreatedBrush)
            {
                brush.IsSelected = true;
                if (Select.DeselectOthersWhenSelectingCreation)
                {
                    action = new ActionCollection(new ChangeSelection(new MapObject[0], Document.Selection.GetSelectedObjects()), action);
                }
            }
            Document.PerformAction("Create " + BrushManager.CurrentBrush.Name.ToLower(), action);
        }

        private MapObject GetBrush(Box bounds, IDGenerator idg)
        {
            var brush = new BlockBrush();
            var ti = Document.TextureCollection.SelectedTexture;
            var texture = ti != null ? ti.GetTexture() : null;
            var created = brush.Create(idg, bounds, texture, 0).ToList();
            if (created.Count > 1)
            {
                var g = new Group(idg.GetNextObjectID());
                created.ForEach(x => x.SetParent(g));
                g.UpdateBoundingBox();
                return g;
            }
            return created.FirstOrDefault();
        }

        public override void MouseClick(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            switch (_state)
            {
                case SketchState.None:
                case SketchState.Ready:
                    // nothin
                    break;
                case SketchState.DrawingBase:
                    // IF dragging base
                    // left: go to volume mode
                    break;
                case SketchState.DrawingVolume:
                    // nothin
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            var vp = viewport as Viewport3D;
            if (vp == null) return;

            UpdateCurrentFace(vp, e);

            switch (_state)
            {
                case SketchState.None:
                case SketchState.Ready:
                    // face detect
                    break;
                case SketchState.DrawingBase:
                    _drawing = new Box(_drawing.Start, _intersection);
                    break;
                case SketchState.DrawingVolume:
                    _drawing = new Box(_drawing.Start, _intersection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateCurrentFace(Viewport3D viewport, ViewportEvent e)
        {
            var ray = viewport.CastRayFromScreen(e.X, e.Y);

            // The face doesn't change when drawing, just update the intersection
            if (_state == SketchState.DrawingBase || _state == SketchState.DrawingVolume)
            {
                _intersection = (_state == SketchState.DrawingBase ? _currentFace.Plane : _volumePlane).GetIntersectionPoint(ray, true, true);
                return;
            }

            var isect = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray)
                .OfType<Solid>()
                .SelectMany(x => x.Faces)
                .Select(x => new { Item = x, Intersection = x.GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .FirstOrDefault();

            if (isect != null)
            {
                if (_currentFace != isect.Item)
                {
                    _cloneFace = isect.Item.Clone();
                    _cloneFace.Transform(new UnitTranslate(isect.Item.Plane.Normal * 0.1m), TransformFlags.None);
                }

                _currentFace = isect.Item;
                _intersection = isect.Intersection;
                _state = SketchState.Ready;
            }
            else
            {
                _cloneFace = null;
                _currentFace = null;
                _intersection = null;
                _state = SketchState.None;
            }
        }


        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {
            switch (_state)
            {
                case SketchState.None:
                case SketchState.Ready:
                    // nothin
                    break;
                case SketchState.DrawingBase:
                case SketchState.DrawingVolume:
                    // esc: cancel
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame)
        {
            //
        }

        public override void Render(ViewportBase viewport)
        {
            // Render
            if (_drawing != null)
            {
                var faces = _drawing.GetBoxFaces().Select(x =>
                {
                    var f = new Face(0) { Plane = new Plane(x[0], x[1], x[2])};
                    f.Vertices.AddRange(x.Select(v => new Vertex(v + f.Plane.Normal * 0.1m, f)));
                    return f;
                });
                MapObjectRenderer.DrawFilled(faces, Color.FromArgb(64, Color.DodgerBlue), false, false);
            }
            else if (_cloneFace != null)
            {
                MapObjectRenderer.DrawFilled(new[] { _cloneFace }, Color.FromArgb(64, Color.Orange), false, false);
            }
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsPaste:
                    return HotkeyInterceptResult.SwitchToSelectTool;
            }
            return HotkeyInterceptResult.Continue;
        }
    }
}
