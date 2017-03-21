using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes.Controls;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering;
using Sledge.Editor.Rendering.Converters;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Tests.Rendering
{
    [TestClass]
    public class RenderingTests
    {
        [TestMethod]
        public async Task TestOctree()
        {
            var generator = new Sledge.Editor.Brushes.TextBrush();
            var octree = new Octree<RenderableObject>();
            var random = new Random();
            var ctor = typeof(Document).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            var document = (Document)ctor.Invoke(null);

            generator.GetControls().OfType<TextControl>().First().EnteredText = String.Join("", Enumerable.Range(0, 50).Select(x => random.Next(0, 100)));
            var someText = generator.Create(new IDGenerator(), new Sledge.DataStructures.Geometric.Box(Coordinate.Zero, Coordinate.One * 100), null, 2);

            Func<MapObject, Task<IEnumerable<RenderableObject>>> map = async x => (await MapObjectConverter.Convert(document, x)).OfType<RenderableObject>();
            var converted = new List<RenderableObject>();
            foreach (var o in someText)
            {
                converted.AddRange((await MapObjectConverter.Convert(document, o)).OfType<RenderableObject>());
            }
            var count = converted.Count;

            Assert.AreEqual(0, octree.Count);
            octree.Add(converted);
            Assert.AreEqual(count, octree.Count);
            octree.Remove(converted);
            Assert.AreEqual(0, octree.Count);
        }
    }
}
