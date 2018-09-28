using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components.Properties.SmartEdit;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    /// <summary>
    /// An entity's flags are a series of boolean toggles that are 
    /// encoded into a single integer as a bit field.
    /// 
    /// The interface is represented as a series of checkboxes
    /// that the user can freely toggle.
    /// 
    /// This tab is only visible when the object context contains
    /// selected parents all of the same entity class.
    /// </summary>
    [AutoTranslate]
    [Export(typeof(IObjectPropertyEditorTab))]
    public sealed partial class FlagsTab : UserControl, IObjectPropertyEditorTab
    {
        [ImportMany] private IEnumerable<Lazy<SmartEditControl>> _smartEditControls;

        /// <inheritdoc />
        public string OrderHint => "H";

        /// <inheritdoc />
        public Control Control => this;

        /// <inheritdoc />
        public bool HasChanges => GetChangedValues().Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlagsTab()
        {
            InitializeComponent();
            CreateHandle();
        }

        /// <inheritdoc />
        public bool IsInContext(IContext context, List<IMapObject> objects)
        {
            return context.TryGet("ActiveDocument", out MapDocument _) &&
                   // All selected entities must be the same class
                   objects.GroupBy(x => x.Data.GetOne<EntityData>()?.Name).Count(x => x.Key != null) == 1;
        }

        /// <inheritdoc />
        public async Task SetObjects(MapDocument document, List<IMapObject> objects)
        {
            GameData gd = null;
            if (document != null) gd = await document.Environment.GetGameData();
            if (gd == null) gd = new GameData();
            if (objects == null) objects = new List<IMapObject>();
            this.InvokeLater(() =>
            {
                UpdateObjects(gd, objects);
            });
        }

        /// <inheritdoc />
        public IEnumerable<IOperation> GetChanges(MapDocument document, List<IMapObject> objects)
        {
            var cv = GetChangedValues();
            if (cv.Count == 0) yield break;

            foreach (var mo in objects)
            {
                var data = mo.Data.GetOne<EntityData>();
                if (data != null)
                {
                    var cf = data.Flags;
                    var nf = ApplyFlags(cf, cv);
                    yield return new EditEntityDataFlags(mo.ID, nf);
                }
            }
        }

        /// <summary>
        /// Apply a flags changeset to an original flags value
        /// </summary>
        private int ApplyFlags(int originalFlags, Dictionary<int, bool> changes)
        {
            foreach (var kv in changes)
            {
                if (kv.Value) originalFlags |= kv.Key;
                else originalFlags &= ~kv.Key;
            }
            return originalFlags;
        }

        /// <summary>
        /// Get a list of options that have been changed since the objects were set.
        /// Indeterminate checkboxes are never a change.
        /// </summary>
        private Dictionary<int, bool> GetChangedValues()
        {
            var d = new Dictionary<int, bool>();

            for (var i = 0; i < FlagsTable.Items.Count; i++)
            {
                var fh = (FlagHolder) FlagsTable.Items[i];
                var cs = FlagsTable.GetItemCheckState(i);
                if (cs == CheckState.Indeterminate || cs == fh.OriginalValue) continue;
                d.Add(fh.BitValue, cs == CheckState.Checked);
            }

            return d;
        }
        
        /// <summary>
        /// Update the checkbox list with the items in the given scope
        /// </summary>
        private void UpdateObjects(GameData gameData, List<IMapObject> objects)
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
        
        /// <summary>
        /// Populate the flags list using the given the source flags data of the context.
        /// </summary>
        /// <param name="cls">The game data object. If null, the list will remain blank.</param>
        /// <param name="flags">A list of flag values for the current context</param>
        private void PopulateFlags(GameDataObject cls, List<int> flags)
        {
            FlagsTable.Items.Clear();

            var flagsProp = cls?.Properties.FirstOrDefault(x => x.Name == "spawnflags");
            if (flagsProp == null) return;

            foreach (var option in flagsProp.Options.OrderBy(x => int.TryParse(x.Key, out int v) ? v : 0))
            {
                var key = int.Parse(option.Key);
                var numChecked = flags.Count(x => (x & key) > 0);
                var cs = numChecked == flags.Count ? CheckState.Checked : (numChecked == 0 ? CheckState.Unchecked : CheckState.Indeterminate);
                FlagsTable.Items.Add(new FlagHolder(option, cs), cs);
            }
        }

        /// <summary>
        /// Container for an option/flag to be used by the checkbox list.
        /// Also maintains the original state of each item to detect changes later.
        /// </summary>
        private class FlagHolder
        {
            public CheckState OriginalValue { get; }
            private Option Option { get; }
            public int BitValue => int.TryParse(Option.Key, out int v) ? v : 0;

            public FlagHolder(Option option, CheckState originalValue)
            {
                OriginalValue = originalValue;
                Option = option;
            }

            public override string ToString()
            {
                return Option.Description;
            }
        }

        private void FlagsTableChanged(object sender, ItemCheckEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChanges)));
        }
    }
}
