using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;

namespace Sledge.BspEditor.Modification.Operations.Data
{
    public class RemoveMapData : IOperation
    {
        private List<IMapData> _dataToRemove;

        public RemoveMapData(params IMapData[] dataToRemove)
        {
            _dataToRemove = dataToRemove.ToList();
        }

        public RemoveMapData(IEnumerable<IMapData> dataToRemove)
        {
            _dataToRemove = dataToRemove.ToList();
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);
            
            foreach (var d in _dataToRemove)
            {
                document.Map.Data.Remove(d);
            }
            ch.UpdateDocument();

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            foreach (var d in _dataToRemove)
            {
                document.Map.Data.Add(d);
            }
            ch.UpdateDocument();

            return ch;
        }
    }
}