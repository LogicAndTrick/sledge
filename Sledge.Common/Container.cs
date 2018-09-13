using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace Sledge.Common
{
    /// <summary>
    /// A service locator for the MEF container. Sometimes dependency injection just adds cruft.
    /// </summary>
    public static class Container
    {
        private static CompositionContainer _container;

        /// <summary>
        /// Initialise the container
        /// </summary>
        /// <param name="container">The MEF container</param>
        public static void Initialise(CompositionContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Get an unnamed export from the container
        /// </summary>
        /// <typeparam name="T">The export type</typeparam>
        /// <returns>The first export of this type, or the type default if none found</returns>
        public static T Get<T>()
        {
            return _container.GetExportedValues<T>().FirstOrDefault();
        }

        /// <summary>
        /// Get a names export from the container
        /// </summary>
        /// <typeparam name="T">The export type</typeparam>
        /// <param name="name">The name of the export</param>
        /// <returns>The first export of this type and name, or the type default if none found</returns>
        public static T Get<T>(string name)
        {
            return _container.GetExportedValues<T>(name).FirstOrDefault();
        }

        /// <summary>
        /// Get a list of unnamed exports from the container
        /// </summary>
        /// <typeparam name="T">The export type</typeparam>
        /// <returns>All the export of this type</returns>
        public static IEnumerable<T> GetMany<T>()
        {
            return _container.GetExportedValues<T>();
        }

        /// <summary>
        /// Get a list of named exports from the container
        /// </summary>
        /// <typeparam name="T">The export type</typeparam>
        /// <param name="name">The name of the export</param>
        /// <returns>All the export of this type and name</returns>
        public static IEnumerable<T> GetMany<T>(string name)
        {
            return _container.GetExportedValues<T>(name);
        }
    }
}
