using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering.Scene
{
    /// <summary>
    /// The entry point for the rendering infrastructure.
    /// Handles when map documents are opened and closed, changed, and activated.
    /// </summary>
    [Export(typeof(IStartupHook))]
    public class SceneManager : IStartupHook
    {
        [Import] private Lazy<MapObjectConverter> _converter;
        [Import] private Lazy<EngineInterface> _engine;

        /// <inheritdoc />
        public Task OnStartup()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Opened", DocumentOpened);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);

            return Task.FromResult(0);
        }

        private readonly List<ConvertedScene> _convertedScenes;
        private ConvertedScene _activeScene;

        /// <summary>
        /// Construct a scene manager instance
        /// </summary>
        public SceneManager()
        {
            _convertedScenes = new List<ConvertedScene>();
        }

        // Document events

        private async Task DocumentChanged(Change change)
        {
            var sc = GetOrCreateScene(change.Document);
            if (sc != null) await sc.Update(change);
        }

        private async Task DocumentOpened(IDocument doc)
        {
            if (!(doc is MapDocument md)) return;
            GetOrCreateScene(md);
        }

        private async Task DocumentClosed(IDocument doc)
        {
            if (!(doc is MapDocument md)) return;
            DeleteScene(md);
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var md = doc as MapDocument;

            var scene = GetOrCreateScene(md);
            SetActiveScene(scene);

            Log.Debug("Bsp Renderer", "Scene activated");
        }

        // Scene handling

        private void SetActiveScene(ConvertedScene scene)
        {
            _activeScene?.SetActive(false);
            _activeScene = scene;
            _activeScene?.SetActive(true);
        }

        private ConvertedScene GetOrCreateScene(MapDocument doc)
        {
            lock (_convertedScenes)
            {
                if (doc == null) return null;

                var cs = _convertedScenes.FirstOrDefault(x => x.Document == doc);
                if (cs != null) return cs;

                cs = new ConvertedScene(_converter.Value, doc, _engine.Value);
                _convertedScenes.Add(cs);
                Log.Debug("Bsp Renderer", "Scene created");
                return cs;
            }
        }

        private void DeleteScene(MapDocument doc)
        {
            lock (_convertedScenes)
            {
                var scene = _convertedScenes.FirstOrDefault(x => x.Document == doc);
                if (scene == null) return;

                if (_activeScene == scene) SetActiveScene(null);

                scene.Dispose();
                _convertedScenes.Remove(scene);
                Log.Debug("Bsp Renderer", "Scene deleted");
            }
        }
    }
}
