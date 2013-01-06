using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.History;
using Sledge.Editor.Properties;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Rendering;
using Sledge.Graphics.Helpers;
using Sledge.Settings;
using Sledge.UI;
using Sledge.Editor.Brushes;

namespace Sledge.Editor.Tools
{
    public class BrushTool : BaseBoxTool
    {
        private Box _lastBox;
        private bool _updatePreview;
        private List<Face> _preview;

        public override Image GetIcon()
        {
            return Resources.Tool_Brush;
        }

        public override string GetName()
        {
            return "Brush Tool";
        }

        protected override Color BoxColour
        {
            get { return Color.Turquoise; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(64, Color.Green); }
        }

        public override void ToolSelected()
        {
            BrushManager.ValuesChanged += ValuesChanged;
            var sel = Document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            if (sel.Any())
            {
                _lastBox = new Box(sel.Select(x => x.BoundingBox));
            }
            else if (_lastBox == null)
            {
                _lastBox = new Box(Coordinate.Zero, new Coordinate(Document.GridSpacing, Document.GridSpacing, Document.GridSpacing));
            }
            _updatePreview = true;
        }

        public override void ToolDeselected()
        {
            BrushManager.ValuesChanged -= ValuesChanged;
            _updatePreview = false;
        }

        private void ValuesChanged(IBrush brush)
        {
            if (BrushManager.CurrentBrush == brush) _updatePreview = true;
        }

        protected override void OnBoxChanged()
        {
            _updatePreview = true;
        }

        protected override void LeftMouseDownToDraw(Viewport2D viewport, System.Windows.Forms.MouseEventArgs e)
        {
            base.LeftMouseDownToDraw(viewport, e);
            if (_lastBox == null) return;
            State.BoxStart += viewport.GetUnusedCoordinate(_lastBox.Start);
            State.BoxEnd += viewport.GetUnusedCoordinate(_lastBox.End);
            _updatePreview = true;
        }

        private void CreateBrush(Box bounds)
        {
            var brush = GetBrush(bounds, Document.Map.IDGenerator);
            if (brush != null)
            {
                brush.Parent = Document.Map.WorldSpawn;
                Document.Map.WorldSpawn.Children.Add(brush);
                var hc = new HistoryCreate("Create " + BrushManager.CurrentBrush.Name.ToLower(), new[] {brush});
                Document.History.AddHistoryItem(hc);
            }
            Document.UpdateDisplayLists();
        }

        private MapObject GetBrush(Box bounds, IDGenerator idg)
        {
            var brush = BrushManager.CurrentBrush;
            var created = brush.Create(idg, bounds, TextureHelper.Get("AAATRIGGER")).ToList();
            if (created.Count > 1)
            {
                var g = new Group(idg.GetNextObjectID());
                g.Children.AddRange(created);
                g.UpdateBoundingBox(false);
                return g;
            }
            return created.FirstOrDefault();
        }

        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            var box = new Box(State.BoxStart, State.BoxEnd);
            if (box.Start.X != box.End.X && box.Start.Y != box.End.Y && box.Start.Z != box.End.Z)
            {
                CreateBrush(box);
                _lastBox = box;
            }
            _preview = null;
            base.BoxDrawnConfirm(viewport);
        }

        public override void BoxDrawnCancel(ViewportBase viewport)
        {
            _lastBox = new Box(State.BoxStart, State.BoxEnd);
            _preview = null;
            base.BoxDrawnCancel(viewport);
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            if (_updatePreview && ShouldDrawBox())
            {
                var box = new Box(State.BoxStart, State.BoxEnd);
                var brush = GetBrush(box, new IDGenerator());
                _preview = new List<Face>();
                CollectFaces(_preview, new[] { brush });
            }
            _updatePreview = false;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsPaste:
                    return HotkeyInterceptResult.SwitchToSelectTool;
            }
            return HotkeyInterceptResult.Continue;
        }

        private Color GetRenderColour()
        {
            var col = GetRenderBoxColour();
            return Color.FromArgb(128, col);
        }

        protected override void Render2D(Viewport2D viewport)
        {
            base.Render2D(viewport);
            if (ShouldDrawBox() && _preview != null)
            {
                GL.Color3(GetRenderColour());
                Graphics.Helpers.Matrix.Push();
                var matrix = DisplayListGroup.GetMatrixFor(viewport.Direction);
                GL.MultMatrix(ref matrix);
                DataStructures.Rendering.Rendering.DrawWireframe(_preview, true);
                Graphics.Helpers.Matrix.Pop();
            }
        }

        protected override void Render3D(Viewport3D viewport)
        {
            base.Render3D(viewport);
            if (ShouldDraw3DBox() && _preview != null)
            {
                DataStructures.Rendering.Rendering.DrawFilled(_preview, GetRenderColour());
            }
        }

        private static void CollectFaces(List<Face> faces, IEnumerable<MapObject> list)
        {
            foreach (var mo in list)
            {
                if (mo is Solid)
                {
                    faces.AddRange(((Solid)mo).Faces);
                }
                else if (mo is Entity || mo is Group)
                {
                    CollectFaces(faces, mo.Children);
                }
            }
        }
    }
}
