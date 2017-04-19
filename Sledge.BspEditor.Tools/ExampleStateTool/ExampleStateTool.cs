using System.ComponentModel.Composition;
using System.Drawing;
using Sledge.BspEditor.Tools.State;
using Sledge.Common.Shell.Components;

namespace Sledge.BspEditor.Tools.ExampleStateTool
{
    [Export(typeof(ITool))]
    public class ExampleStateTool : StateTool
    {
        public override void ToolSelected()
        {
            CurrentState = new IdleState(this);
            base.ToolSelected();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Test;
        }

        public override string GetName()
        {
            return "Example State Tool";
        }
    }
}
