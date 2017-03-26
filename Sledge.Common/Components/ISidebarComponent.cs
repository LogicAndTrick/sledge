using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Context;

namespace Sledge.Common.Components
{
    public interface ISidebarComponent
    {
        string Title { get; }
        object Control { get; }
        bool IsInContext(IContext context);
    }
}
