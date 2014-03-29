using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Providers;

namespace Sledge.Tests
{
    [TestClass]
    public class SerialisationTests
    {
        [TestMethod]
        public void TestMapObjectSerialisation()
        {
            // compare the *'s:
            // brush -> GS -> string* -> GS -> brush -> GS -> string*

            var brush = new BlockBrush();
            var b = brush.Create(new IDGenerator(), new Box(Coordinate.Zero, Coordinate.One * 100), null, 0);
            var serialised = GenericStructure.Serialise(b);
            var toString = serialised.ToString();
            var parsed = GenericStructure.Parse(new StringReader(toString));
            var deserialised = GenericStructure.Deserialise<List<MapObject>>(parsed.First());
            var reserialised = GenericStructure.Serialise(deserialised);

            Assert.AreEqual(serialised.ToString(), reserialised.ToString());
        }

        [TestMethod]
        public void TestPrimitiveSerialisation()
        {
            Assert.AreEqual(null, GenericStructure.Deserialise<object>(GenericStructure.Serialise(null)));
            Assert.AreEqual(new DateTime(2014, 01, 01), GenericStructure.Deserialise<DateTime>(GenericStructure.Serialise(new DateTime(2014, 01, 01))));
            Assert.AreEqual(10, GenericStructure.Deserialise<int>(GenericStructure.Serialise(10)));
            Assert.AreEqual(10m, GenericStructure.Deserialise<decimal>(GenericStructure.Serialise(10m)));
            Assert.AreEqual(false, GenericStructure.Deserialise<bool>(GenericStructure.Serialise(false)));
            Assert.AreEqual("12345", GenericStructure.Deserialise<string>(GenericStructure.Serialise("12345")));
            Assert.AreEqual(new Coordinate(123, 456, 789.0005m), GenericStructure.Deserialise<Coordinate>(GenericStructure.Serialise(new Coordinate(123, 456, 789.0005m))));
            var dsBox = GenericStructure.Deserialise<Box>(GenericStructure.Serialise(new Box(Coordinate.Zero, Coordinate.One)));
            Assert.AreEqual(Coordinate.Zero, dsBox.Start);
            Assert.AreEqual(Coordinate.One, dsBox.End);
            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), GenericStructure.Deserialise<Color>(GenericStructure.Serialise(Color.FromArgb(255, 255, 0, 0))));
            Assert.AreEqual(new Plane(Coordinate.UnitZ, 1), GenericStructure.Deserialise<Plane>(GenericStructure.Serialise(new Plane(Coordinate.UnitZ, 1))));
        }

        [TestMethod]
        public void TestObjectSerialisation()
        {
            var thing0 = new SerialisationThing(0);
            var thing1 = new SerialisationThing(1);
            var thing3 = new SerialisationThing(3);

            Assert.AreEqual(GenericStructure.Serialise(thing0).ToString(), GenericStructure.Serialise(GenericStructure.Deserialise<SerialisationThing>(GenericStructure.Serialise(thing0))).ToString());
            Assert.AreEqual(GenericStructure.Serialise(thing1).ToString(), GenericStructure.Serialise(GenericStructure.Deserialise<SerialisationThing>(GenericStructure.Serialise(thing1))).ToString());
            Assert.AreEqual(GenericStructure.Serialise(thing3).ToString(), GenericStructure.Serialise(GenericStructure.Deserialise<SerialisationThing>(GenericStructure.Serialise(thing3))).ToString());
        }

        private class SerialisationThing
        {
            public int Number { get; set; }
            public bool Bool { get; set; }
            public string String { get; set; }
            public List<int> Ints { get; set; }
            public DateTime DateTime { get; set; }
            public List<SerialisationThing> Children { get; set; }

            public SerialisationThing(int num)
            {
                Children = Enumerable.Range(0, num).Select(x => new SerialisationThing(num - 1)).ToList();
                Number = num;
                Bool = num % 2 == 0;
                String = num.ToString(CultureInfo.InvariantCulture);
                Ints = Enumerable.Range(num, num).ToList();
                DateTime = new DateTime(num);
            }
        }
    }
}
