using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Rendering.Scene
{
    [Export(typeof(IStartupHook))]
    public class SceneManager : IStartupHook
    {
        [ImportMany] private IEnumerable<Lazy<ISceneObjectProviderFactory>> _providers;

        public async Task OnStartup()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Opened", DocumentOpened);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
        }

        private readonly List<ConvertedScene> _convertedScenes;
        private WeakReference<MapDocument> _activeDocument;

        public SceneManager()
        {
            _convertedScenes = new List<ConvertedScene>();
            _activeDocument = new WeakReference<MapDocument>(null);
        }

        private async Task DocumentOpened(IDocument doc)
        {
            var md = doc as MapDocument;
            if (md == null) return;
            GetOrCreateScene(md); // Prepare the scene

            var e = md.Environment.GetData<EnvironmentTextureProvider>().FirstOrDefault();
            if (e == null)
            {
                e = new EnvironmentTextureProvider(md.Environment);
                md.Environment.AddData(e);
                await e.Init();
                Renderer.Instance.Engine.Renderer.TextureProviders.Add(e);
            }
        }

        private async Task DocumentClosed(IDocument doc)
        {
            lock (_convertedScenes)
            {
                var scene = _convertedScenes.FirstOrDefault(x => x.Document == doc);
                if (scene != null)
                {
                    scene.Dispose();
                    _convertedScenes.Remove(scene);
                }
            }
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;
            _activeDocument = new WeakReference<MapDocument>(md);

            var scene = GetOrCreateScene(md)?.Scene;
            Renderer.Instance.Engine.Renderer.SetActiveScene(scene);

            var mat = md?.Map.Data.GetOne<SelectionTransform>()?.Transform ?? Matrix.Identity;
            Renderer.Instance.Engine.Renderer.SelectionTransform = mat.ToOpenTKMatrix4();

            Log.Debug("Bsp Renderer", "Scene activated");
        }
        private async Task DocumentChanged(Change change)
        {
            if (!change.HasDataChanges) return;
            if (!_activeDocument.TryGetTarget(out MapDocument act) || act != change.Document) return;
            var mat = change.Document.Map.Data.GetOne<SelectionTransform>()?.Transform ?? Matrix.Identity;
            Renderer.Instance.Engine.Renderer.SelectionTransform = mat.ToOpenTKMatrix4();
        }

        private ConvertedScene GetOrCreateScene(MapDocument doc)
        {
            lock (_convertedScenes)
            {
                if (doc == null) return null;
                var cs = _convertedScenes.FirstOrDefault(x => x.Document == doc);
                if (cs == null)
                {
                    Log.Debug("Bsp Renderer", "Creating scene...");
                    cs = new ConvertedScene(doc);
                    foreach (var p in _providers) cs.AddProvider(p.Value);
                    _convertedScenes.Add(cs);
                }
                return cs;
            }
        }
    }
}
