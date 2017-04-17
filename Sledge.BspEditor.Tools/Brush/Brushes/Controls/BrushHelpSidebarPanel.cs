using System.ComponentModel.Composition;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Shell.Controls;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    [Export(typeof(ISidebarComponent))]
    public class BrushHelpSidebarPanel : TextSidebarPanel
    {
        public override string Title => "Brush Tool";
        public override string Text =>
            "Draw a box in the 2D view to define the size of the brush.\n" +
            "Select the type of the brush to create in the sidebar.\n" +
            "Press *enter* in the 2D view to create the brush.";

        public override bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out BrushTool _);
        }
    }
}