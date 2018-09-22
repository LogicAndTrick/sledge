using System.Numerics;

namespace Sledge.Providers.Model.Mdl10.Format
{
    public struct Header
    {
        public ID ID;
        public Version Version;
        public string Name;
        public int Size;

        public Vector3 EyePosition;
        public Vector3 HullMin;
        public Vector3 HullMax;
        public Vector3 BoundingBoxMin;
        public Vector3 BoundingBoxMax;

        public int Flags;
    }
}