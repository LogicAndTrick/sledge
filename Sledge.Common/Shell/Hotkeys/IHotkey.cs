using System.Threading.Tasks;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Hotkeys
{
    public interface IHotkey : IContextAware
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        string DefaultHotkey { get; }
        
        Task Invoke(); 
    }
}
