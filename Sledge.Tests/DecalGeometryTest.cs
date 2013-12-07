using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Graphics;
using Sledge.Providers;
using Sledge.Providers.Map;

namespace Sledge.Tests
{
    [TestClass]
    public class DecalGeometryTest
    {
        [TestMethod]
        public void TestValidDecalGeometry()
        {
            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new VmfProvider());

            var fi = new FileInfo("decalflip.rmf");
            var ctor = typeof(Document).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            var document = (Document)ctor.Invoke(null);
            document.Map = MapProvider.GetMapFromFile(fi.FullName);

            var ents = document.Map.WorldSpawn.FindAll().OfType<Entity>();
            foreach (var entity in ents)
            {
                entity.Decal = new MockTexture(64, 64, "Test");
                entity.CalculateDecalGeometry();
            }
        }

        [TestMethod]
        public void TestInvalidDecalGeometry()
        {
            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new VmfProvider());

            var fi = new FileInfo("decalflip_bad.rmf");
            var ctor = typeof(Document).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            var document = (Document)ctor.Invoke(null);
            document.Map = MapProvider.GetMapFromFile(fi.FullName);

            var ents = document.Map.WorldSpawn.FindAll().OfType<Entity>();
            foreach (var entity in ents)
            {
                entity.Decal = new MockTexture(64, 64, "Test");
                entity.CalculateDecalGeometry();
            }
        }

        class MockTexture : ITexture
        {
            public string Name { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public bool HasTransparency { get; set; }
            public Bitmap BitmapImage { get; set; }

            public MockTexture(int width, int height, string name)
            {
                Width = width;
                Height = height;
                Name = name;
                BitmapImage = new Bitmap(width, height);
            }

            public void Bind()
            {
                //
            }

            public void Unbind()
            {
                //
            }

            public void Dispose()
            {
                //
            }
        }
    }
}