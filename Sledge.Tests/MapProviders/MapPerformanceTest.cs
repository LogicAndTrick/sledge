using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.Common.Extensions;
using Sledge.Providers.Map;

namespace Sledge.Tests.MapProviders
{
    [TestClass]
    public class MapPerformanceTest
    {
        [TestMethod]
        public void LoadVmfTest()
        {
            MapProvider.Register(new VmfProvider());
            var file = @"D:\Github\sledge\_Resources\VMF\sdk_d2_coast_12.vmf";
            var map = MapProvider.GetMapFromFile(file);
        }

        [TestMethod]
        public void SplitWithQuotesTest()
        {
            var strings = new[]
            {
                "asdf",
                @"""test"" ""test2""",
                "asdf asdf",
                @"""asdf"""
            };
            for (int i = 0; i < 10000; i++)
            {
                foreach (var s in strings)
                {
                    s.SplitWithQuotes();
                }
            }
        }
    }
}
