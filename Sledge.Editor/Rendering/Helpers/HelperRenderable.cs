using Sledge.Editor.Documents;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
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
            var vp = sender as ViewportBase;
            _document.HelperManager.Render(vp);
        }
    }
}