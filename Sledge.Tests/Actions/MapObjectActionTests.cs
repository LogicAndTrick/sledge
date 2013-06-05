using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.MapObjects;
using Sledge.Database.Models;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Documents;
using Sledge.Providers.Map;

namespace Sledge.Tests.Actions
{
    [TestClass]
    public class MapObjectActionTests
    {
        private Document _document;

        // Guess what! These tests are all random. Why? Because shut up, that's why!
        private static IEnumerable<MapObject> GetRandomObjects(Document doc, int count)
        {
            var random = new Random();
            var flat = doc.Map.WorldSpawn.FindAll();
            for (var i = 0; i < count; i++)
            {
                var r = random.Next(0, flat.Count);
                yield return flat[r];
            }
        }

        [TestInitialize]
        public void Initialize()
        {

            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new VmfProvider());

            var fi = new FileInfo("verc_18.rmf");
            var ctor = typeof (Document).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            _document = (Document) ctor.Invoke(null);
            _document.Map = MapProvider.GetMapFromFile(fi.FullName);

            SaveBefore();
        }

        private void SaveBefore()
        {
            if (File.Exists("before.vmf")) File.Delete("before.vmf");
            MapProvider.SaveMapToFile("before.vmf", _document.Map);
        }

        private void Compare()
        {
            if (File.Exists("after.vmf")) File.Delete("after.vmf");
            MapProvider.SaveMapToFile("after.vmf", _document.Map);

            var before = File.ReadAllLines("before.vmf");
            var after = File.ReadAllLines("after.vmf");
            Assert.AreEqual(before.Length, after.Length);

            for (var i = 0; i < before.Length; i++)
            {
                Assert.AreEqual(before[i], after[i]);
            }
        }

        private void TestAction(IAction action)
        {
            action.Perform(_document);
            action.Reverse(_document);
            Compare();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists("before.vmf")) File.Delete("before.vmf");
            if (File.Exists("after.vmf")) File.Delete("after.vmf");
            _document = null;
            MapProvider.DeregisterAll();
        }

        [TestMethod]
        public void TestDelete()
        {
            var random = GetRandomObjects(_document, 50);
            TestAction(new Delete(random.Select(x => x.ID).ToList()));
        }

        [TestMethod]
        public void TestCreate()
        {
            // Remove some random objects from the map to create
            var random = GetRandomObjects(_document, 50).ToList();
            random.ForEach(x => x.SetParent(null));
            SaveBefore();
            TestAction(new Create(random));
        }
    }
}
