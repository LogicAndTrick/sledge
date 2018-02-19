using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Tools.Vertex.Selection
{
    public class VertexSelection : IEnumerable<VertexSolid>
    {
        private readonly object _lock = new object();
        private readonly HashSet<VertexSolid> _selectedSolids;

        public VertexSelection()
        {
            _selectedSolids = new HashSet<VertexSolid>();
        }

        public async Task Clear(MapDocument document)
        {
            var tran = new Transaction();

            lock (_lock)
            {
                if (_selectedSolids.Any())
                {
                    var toDeselect = _selectedSolids.ToList();
                    tran.Add(new TrivialOperation(
                        d => toDeselect.ForEach(x => x.Real.Data.Remove(o => o is VertexHidden)),
                        c => c.UpdateRange(toDeselect.Select(x => x.Real))
                    ));

                    _selectedSolids.Clear();
                }
            }

            if (!tran.IsEmpty) await MapDocumentOperation.Perform(document, tran);
        }

        public async Task Update(MapDocument document)
        {
            if (document == null) return;
            
            var tran = new Transaction();

            lock (_lock)
            {
                var selection = new HashSet<Solid>(document.Selection.OfType<Solid>());
                var toSelect = selection.Except(_selectedSolids.Select(x => x.Real)).ToList();
                var toDeselect = _selectedSolids.Select(x => x.Real).Except(selection).ToList();

                if (toSelect.Any())
                {
                    tran.Add(new TrivialOperation(
                        d => toSelect.ForEach(x => x.Data.Add(new VertexHidden())),
                        c => c.UpdateRange(toSelect)
                    ));
                }

                if (toDeselect.Any())
                {
                    tran.Add(new TrivialOperation(
                        d => toDeselect.ForEach(x => x.Data.Remove(o => o is VertexHidden)),
                        c => c.UpdateRange(toDeselect)
                    ));
                }

                _selectedSolids.RemoveWhere(s => toDeselect.Contains(s.Real));
                _selectedSolids.UnionWith(toSelect.Select(s => new VertexSolid(s)));
            }

            if (!tran.IsEmpty) await MapDocumentOperation.Perform(document, tran);
        }

        public async Task Commit(MapDocument document)
        {

        }

        public IEnumerator<VertexSolid> GetEnumerator()
        {
            lock (_lock)
            {
                return _selectedSolids.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
