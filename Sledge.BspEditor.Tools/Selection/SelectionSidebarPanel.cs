using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Selection
{
    [Export(typeof(ISidebarComponent))]
    [OrderHint("F")]
    public class SelectionSidebarPanel : ISidebarComponent
    {
        private Panel _panel;
        private Label _label;

        private WeakReference<MapDocument> _activeDocument;

        public string Title => "Selection";
        public object Control => _panel;

        public SelectionSidebarPanel()
        {
            _panel = new Panel();
            _label = new Label() { Dock = DockStyle.Fill};
            _panel.Controls.Add(_label);

            Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
            Oy.Subscribe<MapDocument>("MapDocument:SelectionChanged", SelectionChanged);
        }

        private async Task SelectionChanged(MapDocument doc)
        {
            if (_activeDocument != null && _activeDocument.TryGetTarget(out MapDocument d) && d == doc)
            {
                var sel = doc.Map.Data.Get<Primitives.MapData.Selection>().FirstOrDefault();
                var count = sel?.GetSelectedParents().Count() ?? 0;
                _label.Invoke(() =>
                {
                    _label.Text = $"{count} objects selected";
                });
            }
        }

        private async Task DocumentActivated(IDocument document)
        {
            var doc = document as MapDocument;
            _activeDocument = new WeakReference<MapDocument>(doc);
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }
    }
}
