using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Hooks;

namespace Sledge.BspEditor.Modification
{
    /// <summary>
    /// Handles changes on visgroups and objects to maintain the object list and visibilities.
    /// </summary>
    [Export(typeof(IInitialiseHook))]
    public class VisgroupHandler : IInitialiseHook
    {
        public async Task OnInitialise()
        {
            Oy.Subscribe<MapDocument>("Document:Opened", Opened);
            Oy.Subscribe<Change>("MapDocument:Changed", Changed);
        }

        private async Task Opened(MapDocument doc)
        {
            var visgroups = doc.Map.Data.Get<Visgroup>().ToDictionary(x => x.ID, x => x);
            foreach (var obj in doc.Map.Root.FindAll())
            {
                var ids = obj.Data.Get<VisgroupID>().ToList();
                var visible = true;
                foreach (var id in ids)
                {
                    if (!visgroups.ContainsKey(id.ID)) continue;

                    var vis = visgroups[id.ID];
                    vis.Objects.Add(obj);
                    visible = vis.Visible;
                }

                obj.Data.Replace(new VisgroupHidden(!visible));
            }
        }

        private async Task Changed(Change change)
        {
            var changed = false;
            var visgroups = change.Document.Map.Data.Get<Visgroup>().ToDictionary(x => x.ID, x => x);

            foreach (var mo in change.Added.Union(change.Updated))
            {
                var bef = new HashSet<long>(visgroups.Values.Where(x => x.Objects.Contains(mo)).Select(x => x.ID));
                var now = new HashSet<long>(mo.Data.OfType<VisgroupID>().Select(x => x.ID));

                // Remove visgroups the object was in before but isn't now
                foreach (var id in bef.Except(now))
                {
                    changed = true;
                    if (visgroups.ContainsKey(id)) visgroups[id].Objects.Remove(mo);
                }

                // Add visgroups the object is in now bue wasn't before
                foreach (var id in now.Except(bef))
                {
                    changed = true;
                    if (visgroups.ContainsKey(id)) visgroups[id].Objects.Add(mo);
                }
            }

            // Remove all deleted objects
            foreach (var vg in visgroups.Values)
            {
                var c = vg.Objects.Count;
                vg.Objects.ExceptWith(change.Removed);
                changed |= c != vg.Objects.Count;
            }

            // Fire event if changes were found
            if (changed)
            {
                await Oy.Publish("MapDocument:VisgroupsChanged", change.Document);
            }
        }
    }
}