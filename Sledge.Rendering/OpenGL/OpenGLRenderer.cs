using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Arrays;
using Sledge.Rendering.OpenGL.Shaders;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL
{
    public class OpenGLRenderer : IRenderer
    {
        private readonly Dictionary<IViewport, ViewportData> _viewportData;
        private readonly TextureStorage _textureStorage;
        private readonly MaterialStorage _materialStorage;
        private readonly Octree<RenderableObject> _octree;
        private OctreeVertexArray _vertexArray;
        private bool _initialised;

        public Scene Scene { get; private set; }
        public ITextureStorage Textures { get { return _textureStorage; } }
        public IMaterialStorage Materials { get { return _materialStorage; } }

        public OpenGLRenderer()
        {
            _viewportData = new Dictionary<IViewport, ViewportData>();
            _textureStorage = new TextureStorage();
            _materialStorage = new MaterialStorage(this);
            _octree = new Octree<RenderableObject>();
            Scene = new Scene{TrackChanges = true};
            _initialised = false;
        }

        private void InitialiseRenderer()
        {
            if (_initialised) return;

            _vertexArray = new OctreeVertexArray(_octree);

            if (!_textureStorage.Exists("WhitePixel"))
            {
                _textureStorage.Create("WhitePixel", TextureStorage.WhitePixel, 1, 1, TextureFlags.None);
                _textureStorage.Create("DebugTexture", TextureStorage.DebugTexture, 100, 100, TextureFlags.None);
            }

            _initialised = true;
        }

        public IViewport CreateViewport()
        {
            var view = new OpenGLViewport();
            view.Render += RenderViewport;
            view.Update += UpdateViewport;
            return view;
        }

        private void InitialiseViewport(IViewport viewport, ViewportData data)
        {
            if (!data.Initialised)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Lequal);

                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Front);

                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                data.Initialised = true;
            }
            if (data.Width != viewport.Control.Width || data.Height != viewport.Control.Height)
            {
                data.Framebuffer.Size = new Size(viewport.Control.Width, viewport.Control.Height);
            }
        }

        private void ApplySceneChanges()
        {
            if (Scene.HasChanges)
            {
                _octree.Add(Scene.Objects.OfType<RenderableObject>());
                _vertexArray.Rebuild();
                Scene.ClearChanges();
            }
        }

        private void UpdateViewport(IViewport viewport, Frame frame)
        {
            Materials.Update(frame);
        }

        private void RenderViewport(IViewport viewport, Frame frame)
        {
            var data = GetViewportData(viewport);

            InitialiseRenderer();
            InitialiseViewport(viewport, data);
            ApplySceneChanges();

            var prog = new Passthrough();
            
            // Set up FBO
            data.Framebuffer.Bind();
            
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //((PerspectiveCamera)viewport.Camera).Position += new Coordinate(-0.002m, -0.002m, -0.002m);

            var vpMatrix = viewport.Camera.GetViewportMatrix(viewport.Control.Width, viewport.Control.Height);
            var camMatrix = viewport.Camera.GetCameraMatrix();

            // Render
            prog.Bind();
            prog.CameraMatrix = camMatrix;
            prog.ViewportMatrix = vpMatrix;
            _vertexArray.RenderTextured(this);
            prog.Unbind();

            // Blit FBO
            data.Framebuffer.Unbind();
            data.Framebuffer.Render();

            prog.Dispose();

        }

        private ViewportData GetViewportData(IViewport viewport)
        {
            if (!_viewportData.ContainsKey(viewport))
            {
                var data = new ViewportData(new Framebuffer(viewport.Control.Width, viewport.Control.Height));
                _viewportData.Add(viewport, data);
            }
            return _viewportData[viewport];
        }

        private class ViewportData
        {
            public Framebuffer Framebuffer { get; set; }
            public bool Initialised { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public ViewportData(Framebuffer framebuffer)
            {
                Framebuffer = framebuffer;
                Width = Height = 0;
                Initialised = false;
            }
        }
    }
}