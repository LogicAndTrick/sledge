using System.Collections.Generic;
using Sledge.BspEditor.Primitives;

namespace Sledge.BspEditor.Providers
{
    public class BspFileLoadResult
    {
        /// <summary>
        /// A list of messages to present to the user after loading the file
        /// </summary>
        public List<string> Messages { get; set; }
        
        /// <summary>
        /// A list of invalid objects that were present in the file
        /// </summary>
        public List<object> InvalidObjects { get; set; }

        /// <summary>
        /// The map that was loaded from the file. If the load wasn't successful, this should be null.
        /// </summary>
        public Map Map { get; set; }

        public BspFileLoadResult()
        {
            Messages = new List<string>();
            InvalidObjects = new List<object>();
        }
    }
}