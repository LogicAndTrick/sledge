using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Shell;

namespace Sledge.MinimalEditor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Startup.Run();
        }
    }
}
