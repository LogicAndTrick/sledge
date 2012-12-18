using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.Tools.DisplacementTools
{
    public abstract class DisplacementSubTool : BaseTool
    {
        public Control Control { get; set; }

        public override Image GetIcon()
        {
            throw new NotImplementedException();
        }
    }
}
