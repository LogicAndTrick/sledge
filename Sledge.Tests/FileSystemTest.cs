using Sledge.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sledge.Providers.Model;

namespace Sledge.Tests
{
    [TestClass()]
    public class FileSystemTest
    {
        internal virtual IFile CreateGcfContainer()
        {
            const string gcfFolder = @"F:\Steam\steamapps\half-life.gcf";
            return new GcfFile(gcfFolder);
        }

        internal virtual IFile CreateGcfSubfolder()
        {
            const string gcfFolder = @"F:\Steam\steamapps\half-life.gcf";
            return new GcfFile(gcfFolder, "valve");
        }

        internal virtual IFile CreateGcfFile()
        {
            const string gcfFolder = @"F:\Steam\steamapps\half-life.gcf";
            return new GcfFile(gcfFolder, @"valve\halflife.wad");
        }

        internal virtual IFile CreateWadFolderInsideGcfFile()
        {
            const string gcfFolder = @"F:\Steam\steamapps\half-life.gcf";
            const string wadFile = @"valve\halflife.wad";
            return new WadInsideGcfFile(gcfFolder, wadFile);
        }

        internal virtual IFile CreateWadFileInsideGcfFile()
        {
            const string gcfFolder = @"F:\Steam\steamapps\half-life.gcf";
            const string wadFile = @"valve\halflife.wad";
            return new WadInsideGcfFile(gcfFolder, wadFile, "AAATRIGGER.bmp");
        }

        [TestMethod]
        public void TestOpenGcf()
        {
            var f = CreateGcfContainer();
            var children = f.GetChildren();
            //children.ToList().ForEach(x => Console.WriteLine(x.FullPathName));
            Assert.IsTrue(children.Any(x => x.Name == "valve"));
        }

        [TestMethod]
        public void TestSubfolderGcf()
        {
            var f = CreateGcfSubfolder();
            var files = f.GetFiles();
            //files.ToList().ForEach(x => Console.WriteLine(x.FullPathName));
            Assert.IsTrue(files.Any(x => x.Name == "halflife.wad"));
        }

        [TestMethod]
        public void TestFileGcf()
        {
            var f = CreateGcfFile();
            Assert.IsTrue(f.Size > 0);
        }

        [TestMethod]
        public void TestWadFolderInsideGcf()
        {
            var f = CreateWadFolderInsideGcfFile();
            var files = f.GetFiles();
            //files.ToList().ForEach(x => Console.WriteLine(x.FullPathName));
            Assert.IsTrue(files.Any(x => x.Name.ToLower() == "aaatrigger.bmp"));
        }

        [TestMethod]
        public void TestWadFileInsideGcf()
        {
            var f = CreateWadFileInsideGcfFile();
            var data = f.ReadAll();
            //files.ToList().ForEach(x => Console.WriteLine(x.FullPathName));
            Assert.IsTrue(data.Length > 0);
        }
    }
}
