using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Properties;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// The standard square grid
    /// </summary>
    [AutoTranslate]
    [Export]
    [Export(typeof(IGridFactory))]
    public class SquareGridFactory : IGridFactory
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public virtual Image Icon => Resources.SquareGrid;

        // todo !square grid settings
        public int HideSmallerThan { get; } = 4;
        public int HideFactor { get; } = 8;
        public int Highlight1LineNum { get; } = 8;
        public int Highlight2UnitNum { get; } = 1024;

        public async Task<IGrid> Create(MapDocument document)
        {
            var gd = await document.Environment.GetGameData();
            return new SquareGrid(gd.MapSizeHigh, gd.MapSizeLow, 16);
        }
    }
}