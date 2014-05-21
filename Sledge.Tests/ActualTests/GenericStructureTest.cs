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
        [TestMethod]
        public void TestParseBasic()
        {
            var input =
                @"Test
                {
                    key value
                }";
            var parsed = GenericStructure.Parse(new StringReader(input)).First();
            Assert.AreEqual("Test", parsed.Name);
            CollectionAssert.AreEquivalent(new List<string> { "key" }, parsed.GetPropertyKeys().ToList());
            Assert.AreEqual("value", parsed["key"]);
        }

        [TestMethod]
        public void TestParseMultiple()
        {
            var input =
                @"Test
                {
                    key value
                }
                Test2
                {
                    key2 value2
                }";
            var parsed = GenericStructure.Parse(new StringReader(input)).ToList();
            Assert.AreEqual(2, parsed.Count);
            Assert.AreEqual("Test", parsed[0].Name);
            CollectionAssert.AreEquivalent(new List<string> { "key" }, parsed[0].GetPropertyKeys().ToList());
            Assert.AreEqual("value", parsed[0]["key"]);
            Assert.AreEqual("Test2", parsed[1].Name);
            CollectionAssert.AreEquivalent(new List<string> { "key2" }, parsed[1].GetPropertyKeys().ToList());
            Assert.AreEqual("value2", parsed[1]["key2"]);
        }

        [TestMethod]
        public void TestParseChildren()
        {
            var input =
                @"Test
                {
                    key value
                    Child
                    {
                        key2 value2
                        Child2
                        {
                            key3 value3
                        }
                    }
                }";
            var parsed = GenericStructure.Parse(new StringReader(input)).First();
            Assert.AreEqual("Test", parsed.Name);
            CollectionAssert.AreEquivalent(new List<string> { "key" }, parsed.GetPropertyKeys().ToList());
            Assert.AreEqual("value", parsed["key"]);
            Assert.AreEqual(1, parsed.Children.Count);

            var child = parsed.Children.First();
            Assert.AreEqual("Child", child.Name);
            CollectionAssert.AreEquivalent(new List<string> { "key2" }, child.GetPropertyKeys().ToList());
            Assert.AreEqual("value2", child["key2"]);
            Assert.AreEqual(1, child.Children.Count);

            child = child.Children.First();
            Assert.AreEqual("Child2", child.Name);
            CollectionAssert.AreEquivalent(new List<string> { "key3" }, child.GetPropertyKeys().ToList());
            Assert.AreEqual("value3", child["key3"]);
        }

        [TestMethod]
        public void TestParseQuotes()
        {
            var input =
                @"""Test Name""
                {
                    ""key"" ""value""
                    ""key with spaces"" ""value with spaces""
                }";
            var parsed = GenericStructure.Parse(new StringReader(input)).First();
            Assert.AreEqual("Test Name", parsed.Name);
            CollectionAssert.AreEquivalent(new List<string> { "key", "key with spaces" }, parsed.GetPropertyKeys().ToList());
            Assert.AreEqual("value", parsed["key"]);
            Assert.AreEqual("value with spaces", parsed["key with spaces"]);
        }

        [TestMethod]
        public void TestParseComments()
        {
            var input =
                @"Test
                // This is another comment
                {
                    // This is a comment
                    key value
                }";
            var parsed = GenericStructure.Parse(new StringReader(input)).First();
            Assert.AreEqual("Test", parsed.Name);
            CollectionAssert.AreEquivalent(new List<string> { "key" }, parsed.GetPropertyKeys().ToList());
            Assert.AreEqual("value", parsed["key"]);
        }

        [TestMethod]
        public void TestParseWhitespace()
        {
            var input =
                @"

                Test

                {

                    key value



                }";
            var list = GenericStructure.Parse(new StringReader(input)).ToList();
            Assert.AreEqual(1, list.Count);
            var parsed = list[0];
            Assert.AreEqual("Test", parsed.Name);
            CollectionAssert.AreEquivalent(new List<string> { "key" }, parsed.GetPropertyKeys().ToList());
            Assert.AreEqual("value", parsed["key"]);
        }

        /*
         * VMTs were found with `"vertexlitgeneric" {` on one line, rather than having the "{" on a new line.
         */
        [TestMethod]
        public void TestFirstLineNoWhitespace()
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
