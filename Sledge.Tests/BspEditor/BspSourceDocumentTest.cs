using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Providers;

namespace Sledge.Tests.BspEditor
{
    [TestClass]
    public class BspSourceDocumentTest
    {
        private static List<IBspSourceProvider> _loaders;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new ApplicationCatalog());
            var container = new CompositionContainer(catalog);

            _loaders = container.GetExports<IBspSourceProvider>().Select(x => x.Value).ToList();
        }

        [TestMethod]
        public void SimpleSaveLoad()
        {
            var map = new Map();
            map.Root.Data.Add(new EntityData {Name = "worldspawn"});

            var entity = new Entity(2);
            entity.Data.Add(new EntityData());
            entity.EntityData.Name = "test";

            entity.Hierarchy.Parent = map.Root;

            _loaders.ForEach(loader =>
            {
                using (var ms = new MemoryStream())
                {
                    loader.Save(ms, map).Wait();
                    ms.Seek(0, SeekOrigin.Begin);
                    
                    var loaded = loader.Load(ms).Result;
                    Assert.AreEqual(map.Root.Data.GetOne<EntityData>().Name, loaded.Root.Data.GetOne<EntityData>()?.Name, loader.GetType().Name);
                    
                    var loadedEntity = (Entity) loaded.Root.Hierarchy.FirstOrDefault();
                    Assert.IsNotNull(loadedEntity, loader.GetType().Name);
                    Assert.AreEqual(entity.EntityData.Name, loadedEntity.EntityData?.Name, loader.GetType().Name);
                }
            });
        }
    }
}
