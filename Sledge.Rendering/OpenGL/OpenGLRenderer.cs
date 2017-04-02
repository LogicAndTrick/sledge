using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL.Arrays;
using Sledge.Rendering.OpenGL.Shaders;
using Sledge.Rendering.Scenes;

namespace Sledge.Rendering.OpenGL
{
    public class OpenGLRenderer : IRenderer
    {
        private readonly Dictionary<IViewport, ViewportData> _viewportData;
        private readonly Dictionary<Scene, SceneData> _sceneData;
        private readonly TextureStorage _textureStorage;
        private readonly MaterialStorage _materialStorage;
        private readonly ModelStorage _modelStorage;
        private Scene _activeScene;
        private bool _initialised;

        public Passthrough StandardShader { get; private set; }
        public ModelShader ModelShader { get; private set; }

        public ITextureStorage Textures { get { return _textureStorage; } }
        public IMaterialStorage Materials { get { return _materialStorage; } }
        public IModelStorage Models { get { return _modelStorage; } }
        public StringTextureManager StringTextureManager { get; private set; }
        public List<ITextureProvider> TextureProviders { get; private set; }
        public List<IModelProvider> ModelProviders { get; private set; }
        public IRendererSettings Settings { get; private set; }

        private readonly List<string> _requestedTextureQueue;
        private readonly List<string> _requestedModelQueue; 

        public Matrix4 SelectionTransform { get; set; }

        public OpenGLRenderer()
        {
            _viewportData = new Dictionary<IViewport, ViewportData>();
            _sceneData = new Dictionary<Scene, SceneData>();
            _textureStorage = new TextureStorage();
            _materialStorage = new MaterialStorage(this);
            _modelStorage = new ModelStorage();
            _initialised = false;

            SelectionTransform = Matrix4.Identity;
            StringTextureManager = new StringTextureManager(this);
            TextureProviders = new List<ITextureProvider>();
            ModelProviders = new List<IModelProvider>();
            Settings = new OpenGLRendererSettings(this);

            _requestedTextureQueue = new List<string>();
            _requestedModelQueue = new List<string>();
        }

        public void RequestTexture(string name)
        {
            _requestedTextureQueue.Add(name);
        }

        public void RequestModel(string name)
        {
            _requestedModelQueue.Add(name);
        }

        private void InitialiseRenderer()
        {
            if (_initialised) return;

            StandardShader = new Passthrough();
            ModelShader = new ModelShader();

            _materialStorage.Initialise();
            _textureStorage.Initialise();

            _initialised = true;
        }

        public IViewport CreateViewport()
        {
            var view = new OpenGLViewport();
            view.Render += RenderViewport;
            view.Update += UpdateViewport;
            return view;
        }

        public void DestroyViewport(IViewport viewport)
        {
            var data = _viewportData.ContainsKey(viewport) ? _viewportData[viewport] : null;
            if (data != null)
            {
                _viewportData.Remove(viewport);
            }

            var view = viewport as OpenGLViewport;
            if (view == null) return;

            view.Render -= RenderViewport;
            view.Update -= UpdateViewport;
            view.Dispose();
        }

        public Scene CreateScene()
        {
            var scene = new Scene { TrackChanges = true };
            _sceneData.Add(scene, null);
            return scene;
        }

        public void SetActiveScene(Scene scene)
        {
            if (scene == null || _sceneData.ContainsKey(scene)) _activeScene = scene;
        }

        public void RemoveScene(Scene scene)
        {
            if (_activeScene == scene) _activeScene = null;
            if (_sceneData.ContainsKey(scene))
            {
                var data = _sceneData[scene];
                if (data != null) data.Dispose();
            }
            _sceneData.Remove(scene);
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
                data.Width = viewport.Control.Width;
                data.Height = viewport.Control.Height;
            }
        }

        private void UpdateViewport(IViewport viewport, Frame frame)
        {
            Materials.Update(frame);
            Models.Update(frame);
            StringTextureManager.Update(frame);
        }

        private void ProcessTextureQueue()
        {
            if (_requestedTextureQueue.Any())
            {
                foreach (var tp in TextureProviders)
                {
                    var list = _requestedTextureQueue.Where(x => tp.Exists(x)).ToList();
                    _requestedTextureQueue.RemoveAll(list.Contains);
                    tp.Request(list);
                }
            }

            foreach (var tp in TextureProviders)
            {
                foreach (var td in tp.PopRequestedTextures(5))
                {
                    Textures.Create(td.Name, td.Bitmap, td.Width, td.Height, td.Flags);
                }
            }
        }

        private void ProcessModelQueue()
        {
            if (_requestedModelQueue.Any())
            {
                foreach (var mp in ModelProviders)
                {
                    var list = _requestedModelQueue.Where(x => mp.Exists(x)).ToList();
                    _requestedModelQueue.RemoveAll(list.Contains);
                    mp.Request(list);
                }
            }

            foreach (var mp in ModelProviders)
            {
                foreach (var md in mp.PopRequestedModels(1))
                {
                    Models.Add(md.Name, md.Model);
                    foreach (var material in md.Model.Meshes.Select(x => x.Material).Where(x => x != null))
                    {
                        Materials.Add(material);
                    }
                    foreach (var td in md.Textures)
                    {
                        Textures.Create(td.Name, td.Bitmap, td.Width, td.Height, td.Flags);
                    }
                }
            }
        }

        private void RenderViewport(IViewport viewport, Frame frame)
        {
            if (_activeScene == null) return;

            var vpData = GetViewportData(viewport);
            var scData = GetSceneData(_activeScene);

            InitialiseRenderer();
            InitialiseViewport(viewport, vpData);

            scData.Array.ApplyChanges();
            vpData.ElementArray.Update(scData.Array.Elements);

            ProcessTextureQueue();
            ProcessModelQueue();

            // todo: some sort of garbage collection?

            GL.PointSize(Settings.PointSize);

            GL.ClearColor(viewport.Camera is PerspectiveCamera ? Settings.PerspectiveBackgroundColour : Settings.OrthographicBackgroundColour);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            StandardShader.Bind();
            StandardShader.GridSpacing = Settings.PerspectiveGridSpacing;
            StandardShader.ShowGrid = Settings.ShowPerspectiveGrid;
            StandardShader.Unbind();

            scData.Array.Render(this, viewport);
            vpData.ElementArray.Render(this, viewport);
        }

        private ViewportData GetViewportData(IViewport viewport)
        {
            if (!_viewportData.ContainsKey(viewport))
            {
                var data = new ViewportData(this, viewport);
                _viewportData.Add(viewport, data);
            }
            return _viewportData[viewport];
        }

        private SceneData GetSceneData(Scene scene)
        {
            if (!_sceneData.ContainsKey(scene) || _sceneData[scene] == null)
            {
                var data = new SceneData(this, scene);
                _sceneData[scene] = data;
            }
            return _sceneData[scene];
        }

        private class ViewportData : IDisposable
        {
            public ElementArrayCollection ElementArray { get; private set; }
            public bool Initialised { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public ViewportData(IRenderer renderer, IViewport viewport)
            {
                Width = viewport.Control.Width;
                Height = viewport.Control.Height;
                ElementArray = new ElementArrayCollection(renderer, viewport);
                Initialised = false;
            }

            public void Dispose()
            {
                ElementArray.Dispose();
            }
        }

        private class SceneData : IDisposable
        {
            public Scene Scene { get; set; }
            public OctreeVertexArray Array { get; private set; }

            public SceneData(OpenGLRenderer renderer, Scene scene)
            {
                Scene = scene;
                Array = new OctreeVertexArray(renderer, scene, Math.Min(-float.MinValue, float.MaxValue));
            }

            public void Dispose()
            {
                Array.Dispose();
            }
        }
    }
}