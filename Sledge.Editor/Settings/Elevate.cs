using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Sledge.Editor.Settings
{
    public static class Elevation
    {
        public const string ProgramId = "SledgeEditor";

        public static void RegisterFileType(string extension)
        {
            if (!extension.StartsWith(".")) extension = "." + extension;
            var key = Registry.ClassesRoot.OpenSubKey(Path.Combine(extension, "OpenWithProgIds"));
            if (key == null) return; // TODO etc etc
            var registered = key.GetValue(ProgramId, null) != null;
            if (!registered)
            {
                //RegisterFileType(programId, execuatablePath, extension)
                Execute("RegisterFileType", ProgramId, '"' + Assembly.GetEntryAssembly().Location + '"', extension);
            }
        }

        private static void Execute(params string[] parameters)
        {
            var psi = new ProcessStartInfo("Sledge.Editor.Elevate.exe")
                          {
                              Arguments = String.Join(" ", parameters),
                              UseShellExecute = true,
                              Verb = "runas",
                              WindowStyle = ProcessWindowStyle.Hidden
                          };

            try
            {
                Process.Start(psi).WaitForExit();
                //TaskDialog.Show("File associations were " + (unregister ? "un" : "") + "registered");
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == 1223) // 1223: The operation was canceled by the user. 
                    TaskDialog.Show("The operation was canceled by the user.");
            }
        }
    }
}
