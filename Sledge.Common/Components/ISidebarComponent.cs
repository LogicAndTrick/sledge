using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Common.Components
{
    public interface ISidebarComponent
    {
        string Title { get; }
        object Control { get; }
        bool IsInContext();
    }
}
