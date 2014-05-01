using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Operations.EditOperations;
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

            var fi = new FileInfo(@"verc_18.rmf");
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

        private void Compare(bool checkDifferent)
        {
            if (File.Exists("after.vmf")) File.Delete("after.vmf");
            MapProvider.SaveMapToFile("after.vmf", _document.Map);

            var before = File.ReadAllLines("before.vmf");
            var after = File.ReadAllLines("after.vmf");

            if (checkDifferent)
            {
                if (before.Length == after.Length)
                {
                    for (var i = 0; i < before.Length; i++)
                    {
                        if (before[i] != after[i]) return;
                    }
                    Assert.Fail("The two files are the same when they should differ.");
                }
            }
            else
            {
                Assert.AreEqual(before.Length, after.Length);
                for (var i = 0; i < before.Length; i++)
                {
                    Assert.AreEqual(before[i], after[i]);
                }
            }
        }

        private void TestAction(IAction action)
        {
            action.Perform(_document);
            Compare(true);
            action.Reverse(_document);
            Compare(false);
            action.Perform(_document);
            Compare(true);
            action.Reverse(_document);
            Compare(false);
            action.Perform(_document);
            Compare(true);
            action.Reverse(_document);
            Compare(false);
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
            TestAction(new Create(_document.Map.WorldSpawn.ID, random));
        }

        [TestMethod]
        public void TestClip()
        {
            var all = _document.Map.WorldSpawn.FindAll().OfType<Solid>().ToList();
            var plane = new Plane(Coordinate.UnitZ, Coordinate.Zero);
            TestAction(new Clip(all, plane, true, true));
        }

        [TestMethod]
        public void TestEdit()
        {
            var before = GetRandomObjects(_document, 200).OfType<Solid>().ToList();
            var after = before.Select(x => x.Clone()).ToList();
            var rot = new UnitRotate(40, new Line(new Coordinate(1, 0, -1), new Coordinate(2, -3, 7)));
            after.ForEach(x => x.Transform(rot, TransformFlags.None));
            TestAction(new Edit(before, after));
        }

        [TestMethod]
        public void TestEditAction()
        {
            var before = GetRandomObjects(_document, 200).OfType<Solid>().ToList();
            var rot = new UnitRotate(40, new Line(new Coordinate(1, 0, -1), new Coordinate(2, -3, 7)));
            TestAction(new Edit(before, new TransformEditOperation(rot, TransformFlags.None)));
        }

        [TestMethod]
        public void TestReparent()
        {
            var parent = _document.Map.WorldSpawn;
            var objects = _document.Map.WorldSpawn.GetChildren().SelectMany(x => x.FindAll());
            TestAction(new Reparent(parent.ID, objects));
        }
    }
}
