using System.Reflection;
using Microsoft.Win32;

namespace Sledge.Editor.Settings
{
    public static class FileTypeRegistration
    {
        public const string ProgramId = "SledgeEditor";
        public const string ProgramIdVer = "1";

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
                        if (comm != null) comm.SetValue("", "\"" + ExecutableLocation() + "\" /doc \"%1\"");
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

        public static void RegisterDefaultFileType(string extension)
        {
            #if DEBUG
                return;
            #endif

            if (!extension.StartsWith(".")) extension = "." + extension;

            AssociateExtensionHandler(extension);
        }

        public static void RegisterFileTypes()
        {
            #if DEBUG
                return;
            #endif

            AddExtensionHandler(".rmf", "Goldsource RMF File");
            AddExtensionHandler(".map", "Quake MAP File");
            AddExtensionHandler(".vmf", "Sledge VMF File");


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
            shell.SetValue(String.Empty, execuatablePath + " /doc %1");
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
