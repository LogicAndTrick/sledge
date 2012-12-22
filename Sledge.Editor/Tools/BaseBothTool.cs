using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Editor.Tools
{
    public abstract class BaseBothTool : BaseTool
    {
        protected BaseBothTool()
        {
            Usage = ToolUsage.Both;
        }
    }
}
