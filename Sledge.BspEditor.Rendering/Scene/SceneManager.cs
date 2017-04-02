using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.Common.Documents;
using Sledge.Common.Hooks;

namespace Sledge.BspEditor.Rendering.Scene
{
    [Export(typeof(IStartupHook))]
    public class SceneManager : IStartupHook
    {
        [Import] private MapObjectConverter _converter;

        public async Task OnStartup()
        {
            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<IDocument>("Document:Opened", DocumentOpened);
            Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
        }

        private readonly List<ConvertedScene> _convertedScenes;

        public SceneManager()
        {
            _convertedScenes = new List<ConvertedScene>();
        }

        private async Task DocumentOpened(IDocument doc)
        {
            var md = doc as MapDocument;
            if (md == null) return;
            var cs = new ConvertedScene(md, _converter);
            _convertedScenes.Add(cs);
            await cs.UpdateAll();
        }

        private async Task DocumentClosed(IDocument doc)
        {
            var scene = _convertedScenes.FirstOrDefault(x => x.Document == doc);
            if (scene != null)
            {
                scene.Dispose();
                _convertedScenes.Remove(scene);
            }
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var scene = _convertedScenes.FirstOrDefault(x => x.Document == doc)?.Scene;
            Renderer.Instance.Engine.Renderer.SetActiveScene(scene);
        }
    }
}
