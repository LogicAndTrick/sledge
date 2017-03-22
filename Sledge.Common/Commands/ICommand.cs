using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Common.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Details { get; }

        bool IsInContext();
        Task Invoke(CommandParameters parameters);
    }
}
