using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Providers.Map;

namespace Sledge.Tests
{
    /*
     * INVALID SOLIDS TEST
     * -------------------
     * Hammer cannot be automated, so these tests are rather manual.
     * Generate the invalid solids and save them to disk somewhere,
     * then open them in Hammer and check for problems to see if the
     * solid is invalid or not.
     * 
     * RESULTS - HAMMER 3.5
     * --------------------
     * 
     * Box Start Location | Cube Size      | Hammer Tolerance (first invalid result)
     * -------------------|----------------|----------------------------------------
     * 0 units            | 1 unit         | 0.20
     * 0 units            | 10 units       | 0.17
     * 0 units            | 100 units      | 0.17 -> 0.1667
     * 0 units            | 1000 units     | 0.17
     * -------------------|----------------|----------------------------------------
     * -4000 units        | 1 unit         | 0.20
     * -4000 units        | 10 units       | 0.17
     * -4000 units        | 100 units      | 0.17
     * -4000 units        | 1000 units     | 0.17
     * -4000 units        | 8000 units     | 0.17
     * 
     * -> Hammer 3.5 reliably detects invalid solids after 1 / 6 units of variance
     * -> Hammer 4.1 behaves in the same way
     * 
     * Running a test (TweakInvalidSolidsTest) shows that the only valid matching tolerance in the OnPlane function is 0.5.
     * 
     */

    [TestClass]
    public class InvalidSolidTest
    {
        [TestMethod]
        public void GenerateInvalidSolidsTest()
        {
            var path = @"D:\Github\sledge\_Resources\InvalidTest";
            MapProvider.Register(new RmfProvider());
            for (var i = 0.166m; i <= 0.167m; i += 0.0001m)
            {
                var map = GenerateInvalidSolidMap(i, 0, 1000);
                MapProvider.SaveMapToFile(System.IO.Path.Combine(path, i.ToString("#0.0000") + ".rmf"), map);
            }
        }

        [TestMethod]
        public void TweakInvalidSolidsTest()
        {
            Assert.Fail("You don't want to run this. It takes FOREVER.");
            var validTolerances = new List<decimal>();
            for (var ep = 0.001m; ep <= 1m; ep += 0.001m)
            {
                var allValid = true;
                for (var i = 0m; i <= 1m; i += 0.0001m)
                {

                    var map = GenerateInvalidSolidMap(i, 0, 100);
                    //Assert.AreEqual(i <= 0.1666m, ((Solid) map.WorldSpawn.Children[0]).IsValid(), "Solid is not " + (i <= 0.1666m ? "valid" : "invalid") + " when variance = " + i.ToString("#0.0000"));
                    if (i <= 0.1666m != ((Solid)map.WorldSpawn.Children[0]).IsValid(ep))
                    {
                        allValid = false;
                        break;
                    }
                }
                if (allValid) validTolerances.Add(ep);
            }
            Assert.Fail("Valid tolerances are: " + String.Join(", ", validTolerances));
            // The only matching tolerance is 0.5!
        }

        [TestMethod]
        public void EnsureHammerCompatibilityTest()
        {
            for (var i = 0m; i <= 1m; i += 0.0001m)
            {
                var map = GenerateInvalidSolidMap(i, 0, 100);
                Assert.AreEqual(i <= 0.1666m, ((Solid) map.WorldSpawn.Children[0]).IsValid(), "Solid is not " + (i <= 0.1666m ? "valid" : "invalid") + " when variance = " + i.ToString("#0.0000"));
            }
        }

        private Map GenerateInvalidSolidMap(decimal variance, decimal startLocation, decimal cubeSize)
        {
            var map = new Map();
            var block = new BlockBrush();
            var st = Coordinate.One * startLocation;
            var brush = block.Create(map.IDGenerator, new Box(st, st + Coordinate.One * cubeSize), null).OfType<Solid>().First();
            var verts = brush.Faces.SelectMany(x => x.Vertices).Where(x => x.Location == st).ToList();
            verts.ForEach(x => x.Location.X -= variance);
            brush.SetParent(map.WorldSpawn);
            return map;
        }
    }
}
