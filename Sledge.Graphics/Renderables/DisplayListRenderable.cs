using Sledge.Graphics.Helpers;

namespace Sledge.Graphics.Renderables
{
    public class DisplayListRenderable : IRenderable
    {
        public string ListName { get; set; }

        public DisplayListRenderable(string listName)
        {
            ListName = listName;
        }

        public void Render(object sender)
        {
            if (ListName != null)
            {
                DisplayList.Call(ListName);
            }
        }
    }
}
