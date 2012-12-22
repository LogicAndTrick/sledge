using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Editor.Tools
{
    public abstract class Base3DTool : BaseTool
    {
        protected Base3DTool()
        {
            Usage = ToolUsage.View3D;
        }
    }
}
