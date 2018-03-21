using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace Sledge.Common
{
    public static class Container
    {
        private static CompositionContainer _container;
        public static void Initialise(CompositionContainer container) => _container = container;

        public static T Get<T>() => _container.GetExportedValues<T>().First();
        public static T Get<T>(string name) => _container.GetExportedValues<T>(name).First();
        public static IEnumerable<T> GetMany<T>() => _container.GetExportedValues<T>();
        public static IEnumerable<T> GetMany<T>(string name) => _container.GetExportedValues<T>(name);
    }
}
