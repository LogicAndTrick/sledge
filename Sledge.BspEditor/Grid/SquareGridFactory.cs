using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Properties;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// The standard square grid
    /// </summary>
    [AutoTranslate]
    [Export]
    [Export(typeof(IGridFactory))]
    [Export(typeof(ISettingsContainer))]
    public class SquareGridFactory : IGridFactory, ISettingsContainer
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public virtual Image Icon => Resources.SquareGrid;

        [Setting] public static int GridHideSmallerThan { get; set; } = 4;
        [Setting] public static int GridHideFactor { get; set; } = 8;
        [Setting] public static int GridPrimaryHighlight { get; set; } = 8;
        [Setting] public static int GridSecondaryHighlight { get; set; } = 1024;

        public async Task<IGrid> Create(IEnvironment environment)
        {
            var gd = await environment.GetGameData();
            return new SquareGrid(gd.MapSizeHigh, gd.MapSizeLow, 16);
        }

        public bool IsInstance(IGrid grid)
        {
            return grid is SquareGrid;
        }

        string ISettingsContainer.Name => "Sledge.BspEditor.Grid.SquareGridFactory";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Rendering/Grid", "GridHideSmallerThan", typeof(int)) { EditorHint = "1,10"};
            yield return new SettingKey("Rendering/Grid", "GridHideFactor", typeof(int)) { EditorType = "Dropdown", EditorHint = "0,2,4,8,16,32,64" };
            yield return new SettingKey("Rendering/Grid", "GridPrimaryHighlight", typeof(int)) { EditorHint = "0,32" };
            yield return new SettingKey("Rendering/Grid", "GridSecondaryHighlight", typeof(int)) { EditorType = "Dropdown", EditorHint = "0,32,64,128,256,512,1024,2048,4096" };
        }

        public void LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }
    }
}