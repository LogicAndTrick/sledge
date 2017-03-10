using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Providers.Map;

namespace Sledge.Tests.Benchmarking
{
    [TestClass]
    public class MapLoadTest
    {
        private List<long> Benchmark<T>(Func<T> action, Action<T> destructor, int iterations = 10)
        {
            var times = new List<long>();
            var watch = new Stopwatch();

            // Warm up
            for (int i = 0; i < (iterations / 2); i++)
            {
                destructor(action());
            }

            // actual test
            for (var i = 0; i < iterations + 2; i++)
            {
                watch.Start();
                var t = action();
                watch.Stop();

                destructor(t);

                times.Add(watch.ElapsedMilliseconds);
                watch.Reset();
            }


            // remove max and min from results
            times.Remove(times.Max());
            times.Remove(times.Min());

            var sum = times.Sum();
            var avg = sum / times.Count;

            Debug.WriteLine($"{times.Count} iterations took {sum}ms. Average: {avg}ms, Min: {times.Min()}ms, Max: {times.Max()}ms.");

            return times;
        }

        [TestMethod]
        public void TestLoadVmf()
        {
            var vmf = @"D:\Github\sledge\_Resources\VMF\sdk_pl_goldrush.vmf";
            MapProvider.Register(new VmfProvider());

            Benchmark(
                () => MapProvider.GetMapFromFile(vmf),
                x => { });
        }

        [TestMethod]
        public void TestIntersectingPlanes()
        {
            var planes = new List<Plane>
            {
                new Plane(new Coordinate(3168, 2240, 720), new Coordinate(3136, 2240, 720), new Coordinate(3024, 2544, 704)),
                new Plane(new Coordinate(3056, 2544, 640), new Coordinate(3024, 2544, 640), new Coordinate(3136, 2240, 640)),
                new Plane(new Coordinate(3136, 2240, 640), new Coordinate(3024, 2544, 640), new Coordinate(3024, 2544, 704)),
                new Plane(new Coordinate(3168, 2240, 720), new Coordinate(3056, 2544, 704), new Coordinate(3056, 2544, 640)),
                new Plane(new Coordinate(3024, 2544, 640), new Coordinate(3056, 2544, 640), new Coordinate(3056, 2544, 704)),
                new Plane(new Coordinate(3136, 2240, 720), new Coordinate(3168, 2240, 720), new Coordinate(3168, 2240, 640)),
            };
            var id = new IDGenerator();

            Benchmark(
                () =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Solid.CreateFromIntersectingPlanes(planes, id);
                    }
                    return new object();
                },
                x => { });
        }
    }
}
