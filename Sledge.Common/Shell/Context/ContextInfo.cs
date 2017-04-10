using System;

namespace Sledge.Common.Shell.Context
{
    /// <summary>
    /// A holder for a context id and a parameter.
    /// Uses weak references to avoid possible memory leaks.
    /// </summary>
    public class ContextInfo
    {
        /// <summary>
        /// The context id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The parameter value. Could be null.
        /// </summary>
        public WeakReference Value { get; set; }

        public ContextInfo(string id, object value = null)
        {
            ID = id;
            if (value != null) Value = new WeakReference(value);
        }

        /// <summary>
        /// Check if the value exists and is an instance of the supplied type
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <returns>True if the parameter is this type and hasn't been GC'd</returns>
        public bool ValueIs<T>()
        {
            return Value != null && Value.IsAlive && Value.Target is T;
        }
    }
}