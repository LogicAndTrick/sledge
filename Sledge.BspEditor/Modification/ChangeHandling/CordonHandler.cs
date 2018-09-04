using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Modification.ChangeHandling
{
    /// <summary>
    /// Handles changes to the cordon bounds and toggles visibility of objects not in the bounds.
    /// </summary>
    [Export(typeof(IMapDocumentChangeHandler))]
    public class CordonHandler : IMapDocumentChangeHandler
    {
        public string OrderHint => "M";

        public Task Changed(Change change)
        {
            var bounds = change.Document.Map.Data.OfType<CordonBounds>().FirstOrDefault();

            // Trigger if cordon is on, or if it was just turned off in this change
            if (bounds != null && (bounds.Enabled || change.AffectedData.Contains(bounds)))
            {
                if (bounds.Enabled) SetObjectVisibilities(change, bounds.Box);
                else ShowAllObjects(change);
            }

            return Task.CompletedTask;
        }

        private void ShowAllObjects(Change change)
        {
            var hidden = change.Document.Map.Root.Find(x => x.Data.OfType<CordonHidden>().Any()).ToList();
            foreach (var o in hidden)
            {
                o.Data.Remove(x => x is CordonHidden);
                change.Add(o);
            }
        }

        private void SetObjectVisibilities(Change change, Box bounds)
        {
            // Hide objects that are currently visible but shouldn't be
            // Show objects that are not currently visible but should be
            foreach (var o in change.Document.Map.Root.FindAll())
            {
                var shouldBeVisible = o.BoundingBox != null && o.BoundingBox.IntersectsWith(bounds);
                var isCurrentlyVisible = !o.Data.OfType<CordonHidden>().Any();
                if (shouldBeVisible && !isCurrentlyVisible)
                {
                    o.Data.Remove(x => x is CordonHidden);
                    change.Add(o);
                }
                else if (!shouldBeVisible && isCurrentlyVisible)
                {
                    o.Data.Add(new CordonHidden());
                    change.Add(o);
                }
            }
        }
    }
}
