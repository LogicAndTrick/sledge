using System;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Rendering.Components
{
    [Export(typeof(IStatusItem))]
    [OrderHint("F")]
    public class ViewportMouseLocationStatusItem : IStatusItem
    {
        public event EventHandler<string> TextChanged;

        public string ID => "Sledge.BspEditor.Rendering.Components.ViewportMouseLocationStatusItem";
        public int Width => 100;
        public bool HasBorder => true;
        public string Text { get; set; } = "";
        
        public ViewportMouseLocationStatusItem()
        {
            Oy.Subscribe<Vector3?>("MapDocument:ViewportMouseLocationStatus:UpdateValue", UpdateValue);
        }

        private Task UpdateValue(Vector3? value)
        {
            var text = "";
            if (value.HasValue)
            {
                var v = value.Value;
                text = $"{v.X:#0} {v.Y:#0} {v.Z:#0}";
            }

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