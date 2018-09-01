using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Rendering.Components
{
    [Export(typeof(IStatusItem))]
    [AutoTranslate]
    [OrderHint("J")]
    public class ViewportZoomStatusItem : IStatusItem
    {
        public event EventHandler<string> TextChanged;

        public string ID => "Sledge.BspEditor.Rendering.Components.ViewportZoomStatusItem";
        public int Width => 100;
        public bool HasBorder => true;
        public string Text { get; set; } = "";

        public string Zoom { get; set; }
        
        public ViewportZoomStatusItem()
        {
            Oy.Subscribe<float>("MapDocument:ViewportZoomStatus:UpdateValue", UpdateValue);
        }

        private Task UpdateValue(float value)
        {
            var text = value <= 0 ? "" : $"{Zoom}: {value:#0.##}";
            Text = text;
            TextChanged?.Invoke(this, Text);
            return Task.CompletedTask;
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }
    }
}
