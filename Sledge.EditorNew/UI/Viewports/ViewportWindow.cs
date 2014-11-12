using Sledge.Gui.Containers;
using Sledge.Gui.Shell;
using Sledge.Gui.Structures;

namespace Sledge.EditorNew.UI.Viewports
{
    public class ViewportWindow : Window
    {
        public ResizableTable TableControl { get; private set; }

        public ViewportWindow(ResizableTableConfiguration config)
        {
            TableControl = new ResizableTable();
            Container.Set(TableControl);
            TableControl.Configuration = config;
        }
    }
}