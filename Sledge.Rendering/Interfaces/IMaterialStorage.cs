using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Interfaces
{
    public interface IMaterialStorage : IUpdatable
    {
        void Add(Material material);
        void Bind(string uniqueIdentifier);
        bool Exists(string uniqueIdentifier);
        Material Get(string uniqueIdentifier);
        void Remove(string uniqueIdentifier);
        void Clear();
    }
}