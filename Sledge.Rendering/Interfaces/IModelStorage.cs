using Sledge.Rendering.DataStructures.Models;
using Sledge.Rendering.OpenGL.Arrays;

namespace Sledge.Rendering.Interfaces
{
    public interface IModelStorage : IUpdatable
    {
        void Add(string name);
        void Add(string name, Model model);
        bool Exists(string name);
        Model Get(string name);
        ModelVertexArray GetArray(string name);
        void Remove(string name);
    }
}