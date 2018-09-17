using System.Numerics;

namespace Sledge.Providers.Model.Mdl10
{
    public struct MeshVertex
    {
        public int VertexBone;
        public int NormalBone;
        public Vector3 Vertex;
        public Vector3 Normal;
        public Vector2 Texture;
    }
}