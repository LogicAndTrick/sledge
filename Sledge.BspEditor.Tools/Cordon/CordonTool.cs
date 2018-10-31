using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Cordon
{
    [Export(typeof(ITool))]
    [OrderHint("R")]
    [DefaultHotkey("Shift+K")]
    public class CordonTool : BaseDraggableTool
    {
        private readonly CordonBoxDraggableState _cordonBox;

        public CordonTool()
        {
            _cordonBox = new CordonBoxDraggableState(this);
            _cordonBox.BoxColour = Color.Red;
            _cordonBox.FillColour = Color.FromArgb(/*View.SelectionBoxBackgroundOpacity*/ 64, Color.LightGray);
            _cordonBox.State.Changed += CordonBoxChanged;
            States.Add(_cordonBox);

            Usage = ToolUsage.View2D;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Cordon;
        }

        public override string GetName()
        {
            return "CordonTool";
        }

        public override async Task ToolSelected()
        {
            _cordonBox.Update();
            await base.ToolSelected();
        }

        private void CordonBoxChanged(object sender, EventArgs e)
        {
            // Only commit changes after the resize has finished
            if (_cordonBox.State.Action != BoxAction.Drawn) return;

            var document = GetDocument();
            if (document == null) return;

            var bounds = new Box(_cordonBox.State.Start, _cordonBox.State.End);
            var cb = new CordonBounds
            {
                Box = bounds,
                Enabled = document.Map.Data.GetOne<CordonBounds>()?.Enabled == true
            };
            MapDocumentOperation.Perform(document, new TrivialOperation(x => x.Map.Data.Replace(cb), x => x.Update(cb)));
        }
    }
}
