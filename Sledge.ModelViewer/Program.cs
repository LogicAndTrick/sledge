using System;
using Sledge.Gui;
using Sledge.ModelViewer.Bootstrap;

namespace Sledge.ModelViewer
{
    static class Program
    {
        [STAThread]
		public static void Main ()
        {
            string preferredUiImplementation = null; // "WinForms"
            // preferredUiImplementation = Sledge.Settings.Something.PreferredUiImplementation
            UIManager.Initialise(preferredUiImplementation);
            UIManager.Manager.Shell.WindowLoaded += (sender, args) => Bootstrapper.Bootstrap();
            UIManager.Manager.Start();
		}
	}
}
