using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Rendering;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering.Renderers;
using Sledge.Editor.UI;
using Sledge.Extensions;
using Sledge.Graphics.Shaders;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Rendering
{
    public class RenderManager : IDisposable
    {
        private readonly Document _document;
        private readonly IRenderer _renderer;

        public RenderManager(Document document)
        {
            _document = document;
            switch (View.Renderer)
            {
                case RenderMode.OpenGL3:
                    _renderer = new ArrayRendererGL3(_document);
                    break;
                case RenderMode.OpenGL1DisplayLists:
                    _renderer = new DisplayListRendererGL1(_document);
                    break;
                case RenderMode.OpenGL1Immediate:
                    _renderer = new ImmediateRendererGL1(_document);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("This isn't a valid render mode.");
            }
        }

        public void Dispose()
        {
            _renderer.Dispose();
        }

        public void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D)
        {
            _renderer.UpdateGrid(gridSpacing, showIn2D, showIn3D);
        }

        public void SetSelectionTransform(Matrix4 selectionTransform)
        {
            _renderer.SetSelectionTransform(selectionTransform);
        }

        public void Draw2D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            _renderer.Draw2D(context, viewport, camera, modelView);
        }

        public void Draw3D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            _renderer.Draw3D(context, viewport, camera, modelView);
        }

        public void Update()
        {
            _renderer.Update();
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _renderer.UpdatePartial(objects);
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _renderer.UpdatePartial(faces);
        }

        public void Register(IEnumerable<ViewportBase> viewports)
        {
            foreach (var vp in viewports)
            {
                vp.RenderContext.Add(new RenderManagerRenderable(vp, this));
            }
        }
    }
}