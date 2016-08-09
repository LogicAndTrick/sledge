using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.Common.Extensions;
using Sledge.Packages.Vpk;
using Sledge.Packages.Wad;
using Sledge.Providers.Texture;

namespace Sledge.Tests.Benchmarking
{
    [TestClass]
    public class TextureBenchmarkTest
    {
        private List<long> Benchmark<T>(Func<T> action, Action<T> destructor)
        {
            var times = new List<long>();
            var watch = new Stopwatch();

            // Warm up
            for (int i = 0; i < 5; i++)
            {
                destructor(action());
            }

            // actual test
            for (var i = 0; i < 12; i++)
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
        public void TestWadLoading()
        {
            var wad = @"D:\Github\sledge\_Resources\WAD\halflife.wad";
            TextureProvider.Register(new WadProvider());

            Benchmark(
                () =>
                {
                    var tp = TextureProvider.CreateCollection(new string[0], new[] {wad}, new string[0], new string[0]);
                    var items = tp.GetAllItems().ToList();
                    return tp;
                },
                TextureProvider.DeleteCollection);
        }

        [TestMethod]
        public void TestWadPackage()
        {
            var wad = @"D:\Github\sledge\_Resources\WAD\halflife.wad";
            var fi = new FileInfo(wad);
            Benchmark(
                () =>
                {
                    var wp = new WadPackage(fi);
                    return wp;
                },
                x =>
                {
                    x.Dispose();
                });
        }

        [TestMethod]
        public void TestVmtLoading()
        {
            var vtf = @"F:\Steam\SteamApps\common\Team Fortress 2\tf";
            TextureProvider.Register(new VmtProvider());

            Benchmark(
                () =>
                {
                    var tp = TextureProvider.CreateCollection(new[] {vtf}, new string[0], new string[0], new string[0]);
                    var items = tp.GetAllItems().ToList();
                    return tp;
                },
                TextureProvider.DeleteCollection);
        }

        [TestMethod]
        public void TestVpkPackage()
        {
            var vpk = @"F:\Steam\SteamApps\common\Team Fortress 2\tf\tf2_textures_dir.vpk";
            var fi = new FileInfo(vpk);
            Benchmark(
                () =>
                {
                    var vp = new VpkDirectory(fi);
                    return vp;
                },
                x =>
                {
                    x.Dispose();
                });
        }
    }
}
