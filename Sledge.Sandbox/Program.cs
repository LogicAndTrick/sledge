using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using Sledge.Settings;
using Sledge.Settings.GameDetection;
using Sledge.Shell;

namespace Sledge.Sandbox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Startup.Run();
        }
    }
}
