using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Win32;

namespace Sledge.Editor.Settings
{
    public static class FileTypeRegistration
    {
        public const string ProgramId = "SledgeEditor";
        public const string ProgramIdVer = "1";

        public static FileType[] GetSupportedExtensions()
        {
            return new[]
            {
                new FileType(".vmf", "Valve Map File", true),
                new FileType(".rmf", "Worldcraft RMF", true),
                new FileType(".map", "Quake MAP Format", true),
                new FileType(".obj", "Wavefront Model Format", true),

                new FileType(".rmx", "Worldcraft RMF (Hammer Backup)", false),
                new FileType(".max", "Quake MAP Format (Hammer Backup)", false),
                new FileType(".vmx", "Valve Map File (Hammer Backup)", false),
            };
        }

        private static string ExecutableLocation()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        private static void AddExtensionHandler(string extension, string description)
        {
            using (var root = Registry.CurrentUser.OpenSubKey("Software\\Classes", true))
            {
                if (root == null) return;

                using (var progId = root.CreateSubKey(ProgramId + extension + "." + ProgramIdVer))
                {
                    if (progId == null) return;
                    progId.SetValue("", description);

                    using (var di = progId.CreateSubKey("DefaultIcon"))
                    {
                        if (di != null) di.SetValue("", ExecutableLocation() + ",-40001");
                    }

                    using (var comm = progId.CreateSubKey("shell\\open\\command"))
                    {
                        if (comm != null) comm.SetValue("", "\"" + ExecutableLocation() + "\" \"%1\"");
                    }

                    progId.SetValue("AppUserModelID", ProgramId);
                }
            }
        }

        private static void AssociateExtensionHandler(string extension)
        {
            using (var root = Registry.CurrentUser.OpenSubKey("Software\\Classes", true))
            {
                if (root == null) return;

                using (var ext = root.CreateSubKey(extension))
                {
                    if (ext == null) return;
                    ext.SetValue("", ProgramId + extension + "." + ProgramIdVer);
                    ext.SetValue("PerceivedType", "Document");

                    using (var openWith = ext.CreateSubKey("OpenWithProgIds"))
                    {
                        if (openWith != null) openWith.SetValue(ProgramId + extension + "." + ProgramIdVer, string.Empty);
                    }
                }
            }
        }

        public static IEnumerable<string> GetRegisteredDefaultFileTypes()
        {
            using (var root = Registry.CurrentUser.OpenSubKey("Software\\Classes"))
            {
                if (root == null) yield break;

                foreach (var ft in GetSupportedExtensions())
                {
                    using (var ext = root.OpenSubKey(ft.Extension))
                    {
                        if (ext == null) continue;
                        if (Convert.ToString(ext.GetValue("")) == ProgramId + ft.Extension + "." + ProgramIdVer)
                        {
                            yield return ft.Extension;
                        }
                    }
                }
            }
        }

        public static void RegisterDefaultFileTypes(IEnumerable<string> extensions)
        {
            #if DEBUG
                return;
            #endif

            foreach (var e in extensions)
            {
                var extension = e;
                if (!extension.StartsWith(".")) extension = "." + extension;
                AssociateExtensionHandler(extension);
            }
        }

        public static void RegisterFileTypes()
        {
            #if DEBUG
                return;
            #endif

            try
            {
                foreach (var ft in GetSupportedExtensions())
                {
                    AddExtensionHandler(ft.Extension, ft.Description);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // security exception or some such
            }


            /*
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
             */
        }
    }
}
