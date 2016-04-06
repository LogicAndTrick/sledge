using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Operations.EditOperations;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Editor.Tools.SelectTool.TransformationHandles;
using Sledge.Editor.Tools.Widgets;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Editor.Tools.SelectTool
{
    public class SelectionBoxDraggableState : BoxDraggableState
    {
        private List<IDraggable>[] _handles;
        private int _currentIndex;
        private RotationOrigin _rotationOrigin;

        public List<Widget> Widgets { get; private set; }
        private readonly RotationWidget _rotationWidget;

        public SelectionBoxDraggableState(BaseDraggableTool tool) : base(tool)
        {
            Widgets = new List<Widget>
            {
                (_rotationWidget = new RotationWidget(tool.Document) { Active = false })
            };
            BindWidgets();
        }

        private void BindWidgets()
        {
            foreach (var w in Widgets)
            {
                w.Transforming += WidgetTransforming;
                w.Transformed += WidgetTransformed;
            }
        }

        private void WidgetTransforming(object sender, Matrix4? transformation)
        {
            if (transformation.HasValue)
            {
                Tool.Document.SetSelectListTransform(transformation.Value);
            }
        }

        private void WidgetTransformed(object sender, Matrix4? transformation)
        {
            if (transformation.HasValue)
            {
                var cad = new CreateEditDelete();
                cad.Edit(Tool.Document.Selection.GetSelectedParents().ToList(), new TransformEditOperation(new UnitMatrixMult(transformation.Value), Tool.Document.Map.GetTransformFlags()));
                Tool.Document.PerformAction("Transform selection", cad);
            }
            Tool.Document.EndSelectionTransform();
        }

        public void Update()
        {
            _rotationWidget.Active = State.Action != BoxAction.Idle && _currentIndex == 1;
            _rotationWidget.SetPivotPoint(_rotationOrigin.Position);
        }

        protected override void CreateBoxHandles()
        {
            var resize = new List<IDraggable>
            {
                new ResizeTransformHandle(this, ResizeHandle.TopLeft),
                new ResizeTransformHandle(this, ResizeHandle.TopRight),
                new ResizeTransformHandle(this, ResizeHandle.BottomLeft),
                new ResizeTransformHandle(this, ResizeHandle.BottomRight),

                new ResizeTransformHandle(this, ResizeHandle.Top),
                new ResizeTransformHandle(this, ResizeHandle.Right),
                new ResizeTransformHandle(this, ResizeHandle.Bottom),
                new ResizeTransformHandle(this, ResizeHandle.Left),

                new ResizeTransformHandle(this, ResizeHandle.Center), 
            };
            _rotationOrigin = new RotationOrigin();
            _rotationOrigin.DragMoved += (sender, args) => Update();
            var rotate = new List<IDraggable>
            {
                _rotationOrigin,

                new RotateTransformHandle(this, ResizeHandle.TopLeft, _rotationOrigin),
                new RotateTransformHandle(this, ResizeHandle.TopRight, _rotationOrigin),
                new RotateTransformHandle(this, ResizeHandle.BottomLeft, _rotationOrigin),
                new RotateTransformHandle(this, ResizeHandle.BottomRight, _rotationOrigin),

                new ResizeTransformHandle(this, ResizeHandle.Center), 
            };
            var skew = new List<IDraggable>
            {
                new SkewTransformHandle(this, ResizeHandle.Top),
                new SkewTransformHandle(this, ResizeHandle.Right),
                new SkewTransformHandle(this, ResizeHandle.Bottom),
                new SkewTransformHandle(this, ResizeHandle.Left),

                new ResizeTransformHandle(this, ResizeHandle.Center), 
            };

            _handles = new [] { resize, rotate, skew };
        }

        public override IEnumerable<IDraggable> GetDraggables()
        {
            if (State.Action == BoxAction.Idle || State.Action == BoxAction.Drawing) return new IDraggable[0];
            return _handles[_currentIndex];
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            return false;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            var list = new List<Element>();
            var tf = GetTransformationMatrix(viewport, camera, Tool.Document);
            if (State.Action == BoxAction.Resizing && tf.HasValue)
            {
                // todo this looks pretty silly when the box doesn't perfectly match the transformed selection
                var box = new Box(State.OrigStart, State.OrigEnd);
                box = box.Transform(new UnitMatrixMult(tf.Value));
                if (ShouldDrawBox())
                {
                    foreach (var face in box.GetBoxFaces())
                    {
                        var verts = face.Select(x => new PositionVertex(new Position(x.ToVector3()), 0, 0)).ToList();
                        var rc = GetRenderBoxColour();
                        var fe = new FaceElement(PositionType.World, Material.Flat(Color.FromArgb(rc.A / 8, rc)), verts)
                        {
                            RenderFlags = RenderFlags.Wireframe,
                            CameraFlags = CameraFlags.Orthographic,
                            AccentColor = GetRenderBoxColour()
                        };
                        list.Add(fe);
                    }
                }
                if (ShouldDrawBoxText())
                {
                    list.AddRange(GetBoxTextElements(viewport, box.Start.ToVector3(), box.End.ToVector3()));
                }
            }
            else
            {
                list.AddRange(base.GetViewportElements(viewport, camera));
            }
            return list;
        }

        public Matrix4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, Document document)
        {
            if (State.Action != BoxAction.Resizing) return null;
            var tt = Tool.CurrentDraggable as ITransformationHandle;
            return tt != null ? tt.GetTransformationMatrix(viewport, camera, State, document) : null;
        }

        public void Cycle()
        {
            _currentIndex = (_currentIndex + 1) % _handles.Length;
            if (State.Start != null) _rotationOrigin.Position = new Box(State.Start, State.End).Center;
            else _rotationOrigin.Position = Coordinate.Zero;

            //_scaleWidget.Active = _currentIndex == 0;
            _rotationWidget.Active = _currentIndex == 1;
            //_skewWidget.Active = _currentIndex == 2;
        }
    }
}