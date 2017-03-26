using System.ComponentModel.Composition;
using System.Windows.Forms;
using Sledge.Common.Components;
using Sledge.Common.Context;
using Sledge.Common.Documents;

namespace Sledge.MinimalEditor.TextEditor
{
    [Export(typeof(ISidebarComponent))]
    public class SampleSidebarComponent : ISidebarComponent
    {
        private Control _control;
        public string Title => "Example";
        public object Control => _control;

        public SampleSidebarComponent()
        {
            _control = new Panel();
            _control.Controls.Add(new TextBox());
        }

        public bool IsInContext(IContext context)
        {
            IDocument doc;
            return context.TryGet("ActiveDocument", out doc) && doc is TextDocument;
        }
    }
}