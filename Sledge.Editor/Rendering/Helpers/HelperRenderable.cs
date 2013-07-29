using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    public class HelperRenderable : IRenderable
    {
        public void Render(object sender)
        {
            var vp = sender as ViewportBase;
            HelperManager.Render(vp);
        }
    }
}