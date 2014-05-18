using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.Providers;

namespace Sledge.Tests.ActualTests
{
    [TestClass]
    public class GenericStructureTest
    {
        /*
         * VMTs were found with `"vertexlitgeneric" {` on one line, rather than having the "{" on a new line.
         */
        [TestMethod]
        public void FirstLineWhitespaceTest()
        {
            // Found in TF2: materials\ambulance\amb_red.vmt
            var input =
                @"""vertexlitgeneric"" {
                    ""$basetexture"" ""ambulance/amb_red""
                    ""$envmapmask"" ""ambulance/amb_spec""
                    ""$envmap"" ""env_cubemap""
                    ""$envmaptint"" ""[ 0.5020 0.5020 0.5020 ]""
                    ""$surfaceprop"" ""metal""
                }";
            var parsed = GenericStructure.Parse(new StringReader(input)).Single();
            Assert.AreEqual("vertexlitgeneric", parsed.Name);

            var keys = parsed.GetPropertyKeys().ToList();
            Assert.AreEqual(5, keys.Count);
            CollectionAssert.AreEquivalent(new List<string> {"$basetexture", "$envmapmask", "$envmap", "$envmaptint", "$surfaceprop"}, keys);

            Assert.AreEqual("ambulance/amb_red", parsed["$basetexture"]);
            Assert.AreEqual("ambulance/amb_spec", parsed["$envmapmask"]);
            Assert.AreEqual("env_cubemap", parsed["$envmap"]);
            Assert.AreEqual("[ 0.5020 0.5020 0.5020 ]", parsed["$envmaptint"]);
            Assert.AreEqual("metal", parsed["$surfaceprop"]);
        }
    }
}
