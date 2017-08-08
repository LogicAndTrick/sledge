using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components.Properties.SmartEdit;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    [AutoTranslate]
    [Export(typeof(IObjectPropertyEditorTab))]
    public partial class FlagsTab : UserControl, IObjectPropertyEditorTab
    {
        [ImportMany] private IEnumerable<Lazy<SmartEditControl>> _smartEditControls;

        public string OrderHint => "H";
        public Control Control => this;

        public bool HasChanges
        {
            get { return false; }
        }

        public FlagsTab()
        {
            InitializeComponent();
            CreateHandle();

        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument doc) &&
                   // All selected entities must be the same class
                   doc.Selection.GetSelectedParents().GroupBy(x => x.Data.GetOne<EntityData>()?.Name).Count(x => x.Key != null) == 1;
        }

        public async Task SetObjects(MapDocument document, List<IMapObject> objects)
        {
            GameData gd = null;
            if (document != null) gd = await document.Environment.GetGameData();
            this.Invoke(() =>
            {
                UpdateObjects(gd ?? new GameData(), document, objects);
            });
        }

        public IEnumerable<MapDocumentOperation> GetChanges(MapDocument document)
        {
            yield break;
        }

        private void UpdateObjects(GameData gameData, MapDocument document, List<IMapObject> objects)
        {
            SuspendLayout();

            var datas = objects
                .Select(x => new {Object = x, EntityData = x.Data.GetOne<EntityData>()})
                .Where(x => x.EntityData != null)
                .ToList();

            var classes = datas
                .Select(x => x.EntityData.Name)
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            if (classes.Count == 1)
            {
                var gdo = gameData.Classes.FirstOrDefault(x => x.ClassType != ClassType.Base && String.Equals(x.Name, classes[0], StringComparison.InvariantCultureIgnoreCase));
                PopulateFlags(gdo, datas.Select(x => x.EntityData.Flags).ToList());
            }

            ResumeLayout();
        }
        
        private void PopulateFlags(GameDataObject cls, List<int> flags)
        {
            FlagsTable.Items.Clear();

            var flagsProp = cls?.Properties.FirstOrDefault(x => x.Name == "spawnflags");
            if (flagsProp == null) return;

            foreach (var option in flagsProp.Options.OrderBy(x => int.Parse(x.Key)))
            {
                var key = int.Parse(option.Key);
                var numChecked = flags.Count(x => (x & key) > 0);
                FlagsTable.Items.Add(option.Description, numChecked == flags.Count ? CheckState.Checked : (numChecked == 0 ? CheckState.Unchecked : CheckState.Indeterminate));
            }
        }
    }
}
