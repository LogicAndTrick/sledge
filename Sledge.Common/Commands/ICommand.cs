using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Context;

namespace Sledge.Common.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Details { get; }

        bool IsInContext(IContext context);
        Task Invoke(CommandParameters parameters);
    }
}
