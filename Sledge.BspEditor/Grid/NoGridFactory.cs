using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// A grid that does nothing
    /// </summary>
    [AutoTranslate]
    [Export(typeof(IGridFactory))]
    public class NoGridFactory : IGridFactory
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Image Icon => null;

        public async Task<IGrid> Create(MapDocument document)
        {
            return new NoGrid();
        }
    }
}