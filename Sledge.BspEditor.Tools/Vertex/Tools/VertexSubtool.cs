using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Vertex.Selection;

namespace Sledge.BspEditor.Tools.Vertex.Tools
{
    public abstract class VertexSubtool : BaseDraggableTool
    {
        protected VertexSubtool()
        {
            UseValidation = true;
            Active = false;
        }

        public abstract string OrderHint { get; }
        public VertexSelection Selection { get; set; }
        [Import] public VertexTool Parent { get; set; }
        
        public override Image GetIcon() => null;
        public abstract Task SelectionChanged();
        public abstract Control Control { get; }

        protected override void Invalidate()
        {
            base.Invalidate();
            Parent.Invalidate();
        }
    }
}
