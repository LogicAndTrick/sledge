using System.Collections.Generic;

namespace Sledge.Common.Shell.Context
{
    public interface IContext
    {
        /// <summary>
        /// Check if all of the given contexts are current
        /// </summary>
        /// <param name="context">The list of contexts</param>
        /// <returns>True if all contexts exist</returns>
        bool HasAll(params string[] context);
        
        /// <summary>
        /// Check if any of the given contexts are current
        /// </summary>
        /// <param name="context">The list of contexts</param>
        /// <returns>True if any contexts exist</returns>
        bool HasAny(params string[] context);

        /// <summary>
        /// Check if the given context exists with the given parameter
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="context">The context to check</param>
        /// <param name="value">The parameter to match</param>
        /// <returns>True if the context is current and the parameter matches</returns>
        bool Has<T>(string context, T value);

        /// <summary>
        /// Get a context parameter
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="context">The context to get</param>
        /// <param name="defaultValue">The default value if the context isn't current</param>
        /// <returns>The parameter or default value if it wasn't found</returns>
        T Get<T>(string context, T defaultValue = default(T));

        /// <summary>
        /// Try and get a context parameter
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="context">The context to get</param>
        /// <param name="value">The value to populate the parameter into</param>
        /// <returns>True if the parameter was found and exists, false otherwise</returns>
        bool TryGet<T>(string context, out T value);

        /// <summary>
        /// Get all contexts that are current
        /// </summary>
        /// <returns>The list of current contexts</returns>
        IEnumerable<string> GetAll();
    }
}
