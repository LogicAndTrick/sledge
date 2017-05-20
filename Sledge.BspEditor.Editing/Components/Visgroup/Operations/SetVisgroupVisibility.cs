using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Editing.Components.Visgroup.Operations
{
    public class SetVisgroupVisibility : IOperation
    {
        public bool Trivial => true;

        private readonly long _visgroupId;
        private readonly bool _hidden;

        public SetVisgroupVisibility(long visgroupId, bool hidden)
        {
            _visgroupId = visgroupId;
            _hidden = hidden;
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var vis = document.Map.Data.Get<Primitives.MapData.Visgroup>().FirstOrDefault(x => x.ID == _visgroupId);
            if (vis != null)
            {
                var items = vis.Objects.SelectMany(x => x.FindAll());
                foreach (var obj in items)
                {
                    obj.Data.Replace(new VisgroupHidden(_hidden));
                    ch.Update(obj);
                }
            }

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            throw new NotSupportedException();
        }
    }
}
