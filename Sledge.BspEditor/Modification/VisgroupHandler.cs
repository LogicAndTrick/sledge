using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Modification.Operations;
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
            Oy.Subscribe<Change>("MapDocument:Changed", Changed);
        }

        private async Task Changed(Change change)
        {
            var changed = false;
            var visgroups = change.Document.Map.Data.Get<Visgroup>().ToDictionary(x => x.ID, x => x);
            var autoVisgroups = change.Document.Map.Data.Get<AutomaticVisgroup>().ToList();

            // If an object is removed from a visgroup we should force it visible
            var makeVisible = new HashSet<IMapObject>();

            foreach (var mo in change.Added.Union(change.Updated))
            {
                var bef = new HashSet<long>(visgroups.Values.Where(x => x.Objects.Contains(mo)).Select(x => x.ID));
                var now = new HashSet<long>(mo.Data.OfType<VisgroupID>().Select(x => x.ID));

                // Remove visgroups the object was in before but isn't now
                foreach (var id in bef.Except(now))
                {
                    changed = true;
                    if (visgroups.ContainsKey(id)) visgroups[id].Objects.Remove(mo);
                    makeVisible.UnionWith(mo.FindAll());
                }

                // Add visgroups the object is in now but wasn't before
                foreach (var id in now.Except(bef))
                {
                    changed = true;
                    if (visgroups.ContainsKey(id)) visgroups[id].Objects.Add(mo);
                }

                // Handle autovisgroups as well
                foreach (var av in autoVisgroups)
                {
                    var match = av.IsMatch(mo);
                    var contains = av.Objects.Contains(mo);

                    if (!match && contains)
                    {
                        av.Objects.Remove(mo);
                        changed = true;
                        makeVisible.UnionWith(mo.FindAll());
                    }
                    else if (match && !contains)
                    {
                        av.Objects.Add(mo);
                        changed = true;
                    }
                }
            }

            // Remove all deleted objects
            foreach (var vg in visgroups.Values)
            {
                var c = vg.Objects.Count;
                vg.Objects.ExceptWith(change.Removed);
                changed |= c != vg.Objects.Count;
            }

            foreach (var av in autoVisgroups)
            {
                var c = av.Objects.Count;
                av.Objects.ExceptWith(change.Removed);
                changed |= c != av.Objects.Count;
            }

            if (makeVisible.Any())
            {
                await MapDocumentOperation.Perform(change.Document, new TrivialOperation(
                    d => makeVisible.ToList().ForEach(x => x.Data.Remove(v => v is VisgroupHidden)),
                    c => c.UpdateRange(makeVisible)
                ));
            }

            // Fire event if changes were found
            if (changed)
            {
                await Oy.Publish("MapDocument:VisgroupsChanged", change.Document);
            }
        }
    }
}