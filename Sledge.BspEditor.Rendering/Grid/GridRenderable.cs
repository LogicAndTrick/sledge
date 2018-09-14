using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;
using Buffer = Sledge.Rendering.Resources.Buffer;

namespace Sledge.BspEditor.Rendering.Grid
{
    /// <summary>
    /// Renders the grid for a single viewport.
    /// </summary>
    public class GridRenderable : IRenderable
    {
        public float Order => -100;

        private readonly IViewport _viewport;
        private IGrid _grid;

        private bool _validated;
        private OrthographicCamera.OrthographicType _currentType;
        private float _currentZoom;
        private RectangleF _currentBounds;

        private readonly Buffer _buffer;
        private uint _indexCount;

        /// <summary>
        /// Create a grid renderable for a specific viewport.
        /// </summary>
        /// <param name="viewport">The viewport that will render this grid</param>
        /// <param name="engine">The engine interface, to create a buffer</param>
        public GridRenderable(IViewport viewport, EngineInterface engine)
        {
            _viewport = viewport;
            _validated = false;
            _buffer = engine.CreateBuffer();
        }

        /// <summary>
        /// Set the current grid that is being rendered by extracting it from a document.
        /// </summary>
        /// <param name="doc">The document to extract the grid data from</param>
        public void SetGrid(MapDocument doc)
        {
            _grid = doc?.Map.Data.GetOne<GridData>()?.Grid;
            _validated = false;
        }

        public bool ShouldRender(IPipeline pipeline, IViewport viewport)
        {
            return pipeline.Type == PipelineType.Wireframe && viewport == _viewport && _grid != null && viewport.Camera.Type == CameraType.Orthographic;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            if (UpdateRequired()) Update();

            _buffer.Bind(cl, 0);
            cl.DrawIndexed(_indexCount, 1, 0, 0, 0);
        }

        public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
        {
            yield break;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl, ILocation locationObject)
        {
            //
        }

        private bool UpdateRequired()
        {
            if (!(_viewport.Camera is OrthographicCamera oc)) return false;
            if (_grid == null) return false;

            if (!_validated) return true;
            if (oc.ViewType != _currentType) return true;
            if (Math.Abs(_currentZoom - oc.Zoom) > 0.001f) return true;

            var newBounds = GetValidatedBounds(oc, 0);
            if (!_currentBounds.Contains(newBounds)) return true;

            return false;
        }

        private void Update()
        {
            if (!(_viewport.Camera is OrthographicCamera oc)) return;
            if (_grid == null) return;

            var newBounds = GetValidatedBounds(oc, 50);
            var min = oc.Expand(new Vector3(newBounds.Left, newBounds.Top, 0));
            var max = oc.Expand(new Vector3(newBounds.Right, newBounds.Bottom, 0));

            var normal = Vector3.One - oc.Expand(new Vector3(1, 1, 0));
            
            var points = new List<VertexStandard>();
            var indices = new List<uint>();

            uint idx = 0;
            foreach (var line in _grid.GetLines(normal, oc.Zoom, min, max).OrderBy(x => (int) x.Type))
            {
                var c = GetColorForGridLineType(line.Type);
                var col = new Vector4(c.R, c.G, c.B, c.A) / 255f;
                points.Add(new VertexStandard
                {
                    Position = line.Line.Start,
                    Normal = normal,
                    Colour = col,
                    Texture = Vector2.Zero,
                    Tint = Vector4.One
                });
                points.Add(new VertexStandard
                {
                    Position = line.Line.End,
                    Normal = normal,
                    Colour = col,
                    Texture = Vector2.Zero,
                    Tint = Vector4.One
                });
                indices.Add(idx++);
                indices.Add(idx++);
            }

            _buffer.Update(points, indices);
            _indexCount = idx;

            _validated = true;
            _currentType = oc.ViewType;
            _currentZoom = oc.Zoom;
            _currentBounds = newBounds;
        }

        private RectangleF GetValidatedBounds(OrthographicCamera camera, int padding)
        {
            var vmin = camera.Flatten(camera.ScreenToWorld(new Vector3(-padding, camera.Height + padding, 0)));
            var vmax = camera.Flatten(camera.ScreenToWorld(new Vector3(camera.Width + padding, -padding, 0)));
            return new RectangleF(vmin.X, vmin.Y, vmax.X - vmin.X, vmax.Y - vmin.Y);
        }

        public void Dispose()
        {
            _buffer?.Dispose();
        }
        
        private Color GetColorForGridLineType(GridLineType type)
        {
            switch (type)
            {
                case GridLineType.Fractional:
                    return Renderer.FractionalGridLineColour;
                case GridLineType.Standard:
                    return Renderer.StandardGridLineColour;
                case GridLineType.Axis:
                    return Renderer.AxisGridLineColour;
                case GridLineType.Primary:
                    return Renderer.PrimaryGridLineColour;
                case GridLineType.Secondary:
                    return Renderer.SecondaryGridLineColour;
                case GridLineType.Boundary:
                    return Renderer.BoundaryGridLineColour;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}