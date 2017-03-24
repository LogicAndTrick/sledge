using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Common.Hotkeys
{
    public interface IHotkey
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        string DefaultHotkey { get; }
        Task Invoke(); 
    }
}
