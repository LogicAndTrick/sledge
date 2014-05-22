using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace Sledge.Editor.Elevate
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = args[0];
            var parameters = args.Skip(1).ToList();
            var t = typeof (string);
            var info = typeof (Program).GetMethod(method, BindingFlags.Static | BindingFlags.Public, null, parameters.Select(x => t).ToArray(), null);
            if (info == null)
            {
                Fail("method not found");
                return;
            }
            try
            {
                info.Invoke(null, parameters.OfType<object>().ToArray());
            }
            catch (Exception ex)
            {
                Fail(ex.Message, ex);
                return;
            }
            Environment.Exit(0);
        }

        private static void Fail(string message, Exception ex = null)
        {
            Console.WriteLine("Failed: " + message);
            if (ex != null) Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }

        public static void RegisterFileType(string programId, string execuatablePath, string extension)
        {
            var root = Registry.ClassesRoot;

            // Register application
            var progKey = root.CreateSubKey(programId);
            progKey.SetValue("FriendlyTypeName", "Sledge File");
            progKey.SetValue("CurVer", programId);
            progKey.SetValue("AppUserModelID", programId);
            var shell = progKey.CreateSubKey("shell");
            shell.SetValue(String.Empty, "Open");
            shell = shell.CreateSubKey("Open");
            shell = shell.CreateSubKey("Command");
            shell.SetValue(String.Empty, execuatablePath + " %1");
            shell.Close();
            progKey.Close();

            // Register extension
            if (!extension.StartsWith(".")) extension = "." + extension;
            var extKey = root.CreateSubKey(Path.Combine(extension, "OpenWithProgIds"));
            extKey.SetValue(programId, string.Empty);
            extKey.Close();

            root.Close();
        }
    }
}
