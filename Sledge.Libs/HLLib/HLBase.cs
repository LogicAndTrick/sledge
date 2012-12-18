using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Sledge.Libs.HLLib
{
    public static class HLBase
    {
        private static bool _initialised;
        private static bool _open;

        public static void Initialise()
        {
            if (!_initialised)
            {
                HLLib.Initialize();
                _initialised = true;
            }
        }

        public static void ShutDown()
        {
            if (_initialised)
            {
                HLLib.Shutdown();
                _initialised = false;
            }
        }

        public static HLLib.PackageType GetPackageType(string package)
        {
            var ptype = HLLib.Package.GetTypeFromName(package);
            // If it cannot find the package type from the file name,
            // I will need to read [HLLib.DefaultPackageTestBufferSize] bytes from
            // the file and pass it through to hlGetPackageTypeFromMemory.
            return ptype;
        }

        public static uint OpenPackage(HLLib.PackageType type, string package)
        {
            if (_open)
            {
                throw new Exception("Another package is already open.");
            }
            uint id;
            if (!HLLib.Package.Create(type, out id))
            {
                throw new Exception();
            }
            HLLib.Package.Bind(id);
            if (!HLLib.Package.OpenFile(package, (uint) HLLib.FileMode.ReadVolatile))
            {
                HLLib.Package.Delete(id);
                throw new Exception();
            }
            _open = true;
            return id;
        }

        public static void ClosePackage(uint id)
        {
            HLLib.Package.Close();
            HLLib.Package.Delete(id);
            _open = false;
        }

        public static IntPtr GetPackageRoot()
        {
            return HLLib.Package.GetRoot();
        }

        public static MemoryStream GetItemStreamFromPackage(IntPtr item)
        {
            MemoryStream ret = null;
            IntPtr strm;
            if (HLLib.File.CreateStream(item, out strm))
            {
                if (HLLib.Stream.Open(strm, (uint)HLLib.FileMode.ReadVolatile))
                {
                    var size = HLLib.Stream.GetStreamSize(strm);
                    var buffer = new byte[size];
                    var allocation = Marshal.AllocHGlobal((int) size);
                    HLLib.Stream.Read(strm, allocation, size);
                    Marshal.Copy(allocation, buffer, 0, (int) size);
                    Marshal.FreeHGlobal(allocation);
                    HLLib.Stream.Close(strm);
                    ret = new MemoryStream(buffer);
                }
                HLLib.File.ReleaseStream(item, strm);
            }
            return ret;
        }
    }
}
