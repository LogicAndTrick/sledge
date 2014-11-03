using System;
using System.IO;
using System.Reflection;
using Sledge.EditorNew.Bootstrap;
using Sledge.Gui;
using Sledge.Gui.Resources;

namespace Sledge.EditorNew
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string preferredUiImplementation = null; // "WinForms"
            // preferredUiImplementation = Sledge.Settings.Something.PreferredUiImplementation
            UIManager.Initialise(preferredUiImplementation);
            UIManager.Manager.Shell.WindowLoaded += (sender, args) => Bootstrapper.Bootstrap();
            UIManager.Manager.Start();
        }
    }
}
