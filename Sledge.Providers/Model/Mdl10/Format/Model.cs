namespace Sledge.Providers.Model.Mdl10.Format
{
    public struct Model
    {
        public string Name;
        public int Type;
        public float Radius;

        public int NumMesh;
        public int MeshIndex;

        public int NumVerts;
        public int VertInfoIndex;
        public int VertIndex;

        public int NumNormals;
        public int NormalInfoIndex;
        public int NormalIndex;

        public int NumGroups; // Not used
        public int GroupIndex;

        public Mesh[] Meshes;
    }
}