using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Common.Hooks
{
    /// <summary>
    /// A hook that runs at startup
    /// </summary>
    public interface IStartupHook
    {
        /// <summary>
        /// Runs on startup
        /// </summary>
        /// <returns></returns>
        Task OnStartup();
    }
}
