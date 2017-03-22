using System;
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

        Task Invoke(CommandParameters parameters);
    }
}
