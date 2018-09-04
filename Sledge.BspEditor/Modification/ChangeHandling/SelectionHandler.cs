using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Modification.ChangeHandling
{
    [Export(typeof(IMapDocumentChangeHandler))]
    [Export(typeof(IStatusItem))]
    [AutoTranslate]
    [OrderHint("D")]
    public class SelectionHandler : IMapDocumentChangeHandler, IStatusItem
    {
        public event EventHandler<string> TextChanged;
        public string OrderHint => "W";

        public string ID => "Sledge.BspEditor.Modification.SelectionHandler";
        public int Width => 180;
        public bool HasBorder => true;
        public string Text { get; set; } = "";

        public string NoObjectsSelected { get; set; } = "";
        public string NumObjectsSelected { get; set; } = "{0}";

        public async Task Changed(Change change)
        {
            var sel = change.Document.Map.Data.Get<Selection>().FirstOrDefault();
            if (sel == null)
            {
                sel = new Selection();
                change.Document.Map.Data.Add(sel);
            }

            // Any hidden objects should be deselected
            HideDeselectedObjects(change);

            if (sel.Update(change))
            {
                await Oy.Publish("MapDocument:SelectionChanged", change.Document);
                await Oy.Publish("Menu:Update", String.Empty);
            }
            UpdateText(sel);
        }

        private void HideDeselectedObjects(Change change)
        {
            // Objects that are selected but hidden should be deselected
            var items = change.Added.Union(change.Updated).Where(x => x.IsSelected && x.IsHidden()).ToHashSet();

            // Parents should only be selected if all their children are selected, so recursively deselect those too
            while (items.Any())
            {
                var list = items.ToList();
                items.Clear();

                foreach (var o in list.Where(x => x.IsSelected))
                {
                    o.IsSelected = false;
                    change.Update(o);
                    items.Add(o.Hierarchy.Parent);
                }
            }
        }

        private void UpdateText(Selection selection)
        {
            string text;
            var parents = selection.GetSelectedParents().ToList();
            if (!parents.Any())
            {
                text = NoObjectsSelected;
            }
            else if (parents.Count == 1)
            {
                var sel = parents[0];
                if (sel is Entity e && !string.IsNullOrWhiteSpace(e.EntityData?.Name))
                {
                    var edn = e.EntityData.Name;
                    var targetname = e.EntityData.Get("targetname", "").Trim();
                    text = edn + (targetname.Length > 0 ? $" ({targetname})" : "");
                }
                else
                {
                    text = sel.GetType().Name;
                }
            }
            else
            {
                text = String.Format(NumObjectsSelected, parents.Count);
            }

            Text = text;
            TextChanged?.Invoke(this, text);
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }
    }
}