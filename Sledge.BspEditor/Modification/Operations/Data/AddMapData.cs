using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;

namespace Sledge.BspEditor.Modification.Operations.Data
{
    public class AddMapData : IOperation
    {
        private List<IMapData> _dataToAdd;

        public AddMapData(params IMapData[] dataToAdd)
        {
            _dataToAdd = dataToAdd.ToList();
        }

        public AddMapData(IEnumerable<IMapData> dataToAdd)
        {
            _dataToAdd = dataToAdd.ToList();
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);
            
            foreach (var d in _dataToAdd)
            {
                document.Map.Data.Add(d);
            }
            ch.UpdateDocument();

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            foreach (var d in _dataToAdd)
            {
                document.Map.Data.Remove(d);
            }
            ch.UpdateDocument();

            return ch;
        }
    }
}