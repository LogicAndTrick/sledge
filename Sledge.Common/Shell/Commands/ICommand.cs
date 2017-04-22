using System.Threading.Tasks;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Commands
{
    public interface ICommand : IContextAware
    {
        string Name { get; }
        string Details { get; }
        
        Task Invoke(IContext context, CommandParameters parameters);
    }
}
