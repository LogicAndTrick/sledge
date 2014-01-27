using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.Providers.GameData;

namespace Sledge.Tests
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void ParseSourceFGD()
        {
            var hl2 = @"D:\Github\sledge\_Resources\FGD\portal2.fgd";
            GameDataProvider.Register(new FgdProvider());
            var gd = GameDataProvider.GetGameDataFromFile(hl2);
            Assert.IsTrue(gd.Classes.Count > 0);
        }
        [TestMethod]
        public void ParseTF2FGD()
        {
            var tf2 = @"D:\Github\sledge\_Resources\FGD\tf2.fgd";
            GameDataProvider.Register(new FgdProvider());
            var gd = GameDataProvider.GetGameDataFromFile(tf2);
            Assert.IsTrue(gd.MaterialExclusions.Count > 0);
            Assert.IsTrue(gd.AutoVisgroups.Count > 0);
        }
    }
}
