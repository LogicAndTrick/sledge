using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Data
{
    public class RemoveMapObjectData : IOperation
    {
        private long _id;
        private List<IMapObjectData> _dataToRemove;
        public bool Trivial => false;

        public RemoveMapObjectData(long id, params IMapObjectData[] dataToRemove)
        {
            _id = id;
            _dataToRemove = dataToRemove.ToList();
        }

        public RemoveMapObjectData(long id, IEnumerable<IMapObjectData> dataToRemove)
        {
            _id = id;
            _dataToRemove = dataToRemove.ToList();
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            if (obj != null)
            {
                foreach (var d in _dataToRemove)
                {
                    obj.Data.Remove(d);
                }
                obj.DescendantsChanged();
                ch.Update(obj);
            }

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            if (obj != null)
            {
                foreach (var d in _dataToRemove)
                {
                    obj.Data.Add(d);
                }
                obj.DescendantsChanged();
                ch.Update(obj);
            }

            return ch;
        }
    }
}