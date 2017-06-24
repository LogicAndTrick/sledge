using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    [AutoTranslate]
    [Export(typeof(IObjectPropertyEditorTab))]
    public partial class VisgroupTab : UserControl, IObjectPropertyEditorTab
    {
        public string OrderHint => "Y";
        public Control Control => this;

        public string MemberOfGroup
        {
            get => lblMemberOfGroup.Text;
            set => this.Invoke(() => lblMemberOfGroup.Text = value);
        }

        public string EditVisgroups
        {
            get => btnEditVisgroups.Text;
            set => this.Invoke(() => btnEditVisgroups.Text = value);
        }

        public bool HasChanges
        {
            get { return false; }
        }

        public VisgroupTab()
        {
            InitializeComponent();
            
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public Task SetObjects(MapDocument document, List<IMapObject> objects)
        {
            visgroupPanel.Invoke(() =>
            {
                visgroupPanel.Update(document);
            });
            return Task.FromResult(0);
        }

        public IEnumerable<MapDocumentOperation> GetChanges(MapDocument document)
        {
            yield break;
        }
    }
}
