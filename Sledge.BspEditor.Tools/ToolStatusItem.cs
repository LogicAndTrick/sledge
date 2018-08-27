using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(IStatusItem))]
    [OrderHint("H")]
    public class ToolStatusItem : IStatusItem
    {
        public event EventHandler<string> TextChanged;

        public string ID => "Sledge.BspEditor.Tools.ToolStatusItem";
        public int Width => 130;
        public bool HasBorder => true;
        public string Text { get; set; }

        public ToolStatusItem()
        {
            Oy.Subscribe<string>("MapDocument:ToolStatus:UpdateText", UpdateText);
        }

        private async Task UpdateText(string text)
        {
            Text = text;
            TextChanged?.Invoke(this, Text);
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }
    }
}
