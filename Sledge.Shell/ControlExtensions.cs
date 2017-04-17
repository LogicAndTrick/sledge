using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.Shell
{
    public static class ControlExtensions
    {
        public static void Invoke(this Control control, Action action)
        {
            control.Invoke((MethodInvoker) delegate { action(); });
        }
    }
}
