namespace Sledge.Providers.Model.Mdl10.Format
{
    public struct Mesh
    {
        public int NumTriangles;
        public int TriangleIndex;
        public int SkinRef;
        public int NumNormals;
        public int NormalIndex;

        public MeshVertex[] Vertices;
    }
}