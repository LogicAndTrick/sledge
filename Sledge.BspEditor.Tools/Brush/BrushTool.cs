using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Tools.Brush
{
    [Export(typeof(ITool))]
    [Export(typeof(ISettingsContainer))]
    [Export]
    [OrderHint("H")]
    [AutoTranslate]
    [DefaultHotkey("Shift+B")]
    public class BrushTool : BaseDraggableTool, ISettingsContainer
    {
        [Import] private EngineInterface _engine;

        private bool _updatePreview;
        private List<IMapObject> _preview;
        private BoxDraggableState box;
        private IBrush _activeBrush;

        public bool RoundVertices { get; set; } = true;

        public string CreateObject { get; set; } = "Create Object";
        
        // Settings

        [Setting("SelectionBoxBackgroundOpacity")] private int _selectionBoxBackgroundOpacity = 64;
        [Setting("SelectCreatedBrush")] private bool _selectCreatedBrush = true;
        [Setting("SwitchToSelectAfterBrushCreation")] private bool _switchToSelectAfterCreation = false;
        [Setting("ResetBrushTypeOnCreation")] private bool _resetBrushTypeOnCreation = false;

        string ISettingsContainer.Name => "Sledge.BspEditor.Tools.BrushTool";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Tools/Brush", "SelectionBoxBackgroundOpacity", typeof(int));
            yield return new SettingKey("Tools/Brush", "SelectCreatedBrush", typeof(bool));
            yield return new SettingKey("Tools/Brush", "SwitchToSelectAfterBrushCreation", typeof(bool));
            yield return new SettingKey("Tools/Brush", "ResetBrushTypeOnCreation", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        public BrushTool()
        {
            box = new BoxDraggableState(this);
            box.BoxColour = Color.Turquoise;
            box.FillColour = Color.FromArgb(_selectionBoxBackgroundOpacity, Color.Green);
            box.State.Changed += BoxChanged;
            States.Add(box);

            UseValidation = true;
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<Change>("MapDocument:Changed", x =>
            {
                if (x.Document == Document)
                {
                    TextureSelected();
                }
                return Task.FromResult(0);
            });
            yield return Oy.Subscribe<object>("BrushTool:ValuesChanged", x =>
            {
                _updatePreview = true;
                Invalidate();
            });
            yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", x =>
            {
                x.Clear();
                x.AddCallback(String.Format(CreateObject, _activeBrush?.Name), () => Confirm(x.Viewport));
            });
        }

        protected override void ContextChanged(IContext context)
        {
            _activeBrush = context.Get<IBrush>("BrushTool:ActiveBrush");
            _updatePreview = true;
            Invalidate();

            base.ContextChanged(context);
        }

        public override async Task ToolSelected()
        {
            var sel = Document.Selection.OfType<Solid>().ToList();
            if (sel.Any())
            {
                box.RememberedDimensions = new Box(sel.Select(x => x.BoundingBox));
            }
            else if (box.RememberedDimensions == null)
            {
                var gs = Document.Map.Data.GetOne<GridData>()?.Grid;
                var start = Vector3.Zero;
                var next = gs?.AddStep(Vector3.Zero, Vector3.One) ?? Vector3.One * 64;
                box.RememberedDimensions = new Box(start, next);
            }

            _updatePreview = true;
            await base.ToolSelected();
        }

        public override async Task ToolDeselected()
        {
            _updatePreview = false;
            await base.ToolDeselected();
        }

        private void TextureSelected()
        {
            _updatePreview = true;
            Invalidate();
        }

        private void BoxChanged(object sender, EventArgs e)
        {
            _updatePreview = true;
            Invalidate();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Brush;
        }

        public override string GetName()
        {
            return "BrushTool";
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirm(viewport);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel(viewport);
            }
            base.KeyDown(viewport, e);
        }

        private void CreateBrush(Box bounds)
        {
            var brush = GetBrush(bounds, Document.Map.NumberGenerator);
            if (brush == null) return;
            
            var transaction = new Transaction();

            transaction.Add(new Attach(Document.Map.Root.ID, brush));

            if (_selectCreatedBrush)
            {
                transaction.Add(new Deselect(Document.Selection));
                transaction.Add(new Select(brush.FindAll()));
            }

            MapDocumentOperation.Perform(Document, transaction);
        }

        private IMapObject GetBrush(Box bounds, UniqueNumberGenerator idg)
        {
            var brush = _activeBrush;
            if (brush == null) return null;

            // Don't round if the box is rather small
            var rounding = RoundVertices ? 0 : 2;
            if (bounds.SmallestDimension < 10) rounding = 2;

            var ti = Document.Map.Data.GetOne<ActiveTexture>()?.Name ?? "aaatrigger";
            var created = brush.Create(idg, bounds, ti, rounding).ToList();

            // Align all textures to the face and set the texture scale
            foreach (var f in created.SelectMany(x => x.Data.OfType<Face>()))
            {
                f.Texture.XScale = f.Texture.YScale = (float) Document.Environment.DefaultTextureScale;
                f.Texture.AlignToNormal(f.Plane.Normal);
            }

            // If there's more than one object in the result, group them up
            if (created.Count > 1)
            {
                var g = new Group(idg.Next("MapObject"));
                created.ForEach(x => x.Hierarchy.Parent = g);
                g.DescendantsChanged();
                return g;
            }

            return created.FirstOrDefault();
        }

        private void Confirm(MapViewport viewport)
        {
            if (box.State.Action != BoxAction.Drawn) return;
            var bbox = new Box(box.State.Start, box.State.End);
            if (!bbox.IsEmpty())
            {
                CreateBrush(bbox);
                box.RememberedDimensions = bbox;
            }
            _preview = null;
            box.State.Action = BoxAction.Idle;
            if (_switchToSelectAfterCreation)
            {
                Oy.Publish("ActivateTool", "SelectTool");
            }
            if (_resetBrushTypeOnCreation)
            {
                Oy.Publish("BrushTool:ResetBrushType", this);
            }
        }

        private void Cancel(MapViewport viewport)
        {
            box.RememberedDimensions = new Box(box.State.Start, box.State.End);
            _preview = null;
            box.State.Action = BoxAction.Idle;
        }

        private List<IMapObject> GetPreview()
        {
            if (_updatePreview)
            {
                var bbox = new Box(box.State.Start, box.State.End);
                var brush = GetBrush(bbox, new UniqueNumberGenerator()).FindAll();
                _preview = brush;
            }

            _updatePreview = false;
            return _preview ?? new List<IMapObject>();
        }

        public override void Render(BufferBuilder builder)
        {
            if (box.State.Action != BoxAction.Idle)
            {
                // Force this work to happen on a new thread so waiting on it won't block the context
                Task.Run(async () =>
                {
                    foreach (var obj in GetPreview().OfType<Solid>())
                    {
                        await Convert(builder, Document, obj);
                    }
                }).Wait();
            }

            base.Render(builder);
        }

        private async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj)
        {
            var solid = (Solid)obj;

            // Pack the vertices like this [ f1v1 ... f1vn ] ... [ fnv1 ... fnvn ]
            var numVertices = (uint)solid.Faces.Sum(x => x.Vertices.Count);

            // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
            var numSolidIndices = (uint)solid.Faces.Sum(x => (x.Vertices.Count - 2) * 3);
            var numWireframeIndices = numVertices * 2;

            var points = new VertexStandard[numVertices];
            var indices = new uint[numSolidIndices + numWireframeIndices];

            var c = Color.Turquoise;
            var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            c = Color.FromArgb(192, Color.Turquoise);
            var tint = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            var tc = await document.Environment.GetTextureCollection();

            var flags = solid.IsSelected ? VertexFlags.SelectiveTransformed : VertexFlags.None;

            var vi = 0u;
            var si = 0u;
            var wi = numSolidIndices;
            foreach (var face in solid.Faces)
            {
                var t = await tc.GetTextureItem(face.Texture.Name);
                var w = t?.Width ?? 0;
                var h = t?.Height ?? 0;

                var offs = vi;
                var numFaceVerts = (uint)face.Vertices.Count;

                var textureCoords = face.GetTextureCoordinates(w, h).ToList();

                var normal = face.Plane.Normal;
                for (var i = 0; i < face.Vertices.Count; i++)
                {
                    var v = face.Vertices[i];
                    points[vi++] = new VertexStandard
                    {
                        Position = v,
                        Colour = colour,
                        Normal = normal,
                        Texture = new Vector2(textureCoords[i].Item2, textureCoords[i].Item3),
                        Tint = tint,
                        Flags = flags
                    };
                }

                // Triangles - [0 1 2]  ... [0 n-1 n]
                for (uint i = 2; i < numFaceVerts; i++)
                {
                    indices[si++] = offs;
                    indices[si++] = offs + i - 1;
                    indices[si++] = offs + i;
                }

                // Lines - [0 1] ... [n-1 n] [n 0]
                for (uint i = 0; i < numFaceVerts; i++)
                {
                    indices[wi++] = offs + i;
                    indices[wi++] = offs + (i == numFaceVerts - 1 ? 0 : i + 1);
                }
            }

            var groups = new List<BufferGroup>();

            uint texOffset = 0;
            foreach (var f in solid.Faces)
            {
                var texInd = (uint)(f.Vertices.Count - 2) * 3;

                var opacity = tc.GetOpacity(f.Texture.Name);
                var t = await tc.GetTextureItem(f.Texture.Name);
                var transparent = opacity < 0.95f || t?.Flags.HasFlag(TextureFlags.Transparent) == true;

                var texture = $"{document.Environment.ID}::{f.Texture.Name}";
                groups.Add(new BufferGroup(t == null ? PipelineType.FlatColourGeneric : PipelineType.TexturedGeneric, CameraType.Perspective, transparent, f.Origin, texture, texOffset, texInd));
                texOffset += texInd;

                if (t != null)
                {
                    _engine.UploadTexture(texture, () => new EnvironmentTextureSource(document.Environment, t));
                }
            }

            // groups.Add(new BufferGroup(PipelineType.FlatColourGeneric, 0, numSolidIndices));
            groups.Add(new BufferGroup(PipelineType.WireframeGeneric, solid.IsSelected ? CameraType.Both : CameraType.Orthographic, false, solid.BoundingBox.Center, numSolidIndices, numWireframeIndices));

            builder.Append(points, indices, groups);
        }
    }
}