using Sledge.EditorNew.Documents;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics.Renderables;

namespace Sledge.EditorNew.Rendering.Helpers
{
    public class HelperRenderable : IRenderable
    {
        private readonly Document _document;

        public HelperRenderable(Document document)
        {
            _document = document;
        }

        public void Render(object sender)
        {
            var vp = sender as IMapViewport;
            _document.HelperManager.Render(vp);
        }
    }
}