using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Context;

namespace Sledge.Common.Hotkeys
{
    public interface IHotkey
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        string DefaultHotkey { get; }

        bool IsInContext(IContext context);
        Task Invoke(); 
    }
}
