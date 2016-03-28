using System.Collections.Generic;

namespace Sledge.Rendering.Interfaces
{
    public interface IModelProvider
    {
        bool Exists(string name);

        ModelDetails Fetch(string name);
        IEnumerable<ModelDetails> Fetch(IEnumerable<string> names);

        void Request(IEnumerable<string> names);
        IEnumerable<ModelDetails> PopRequestedModels(int count);
    }
}