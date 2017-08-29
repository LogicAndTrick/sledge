using System;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives
{
    [Serializable]
    public class Texture : ISerializable
    {
        public string Name { get; set; }

        public decimal Rotation { get; set; }

        private Coordinate _uAxis;
        public Coordinate UAxis
        {
            get => _uAxis;
            set => _uAxis = value.Normalise();
        }

        private Coordinate _vAxis;
        public Coordinate VAxis
        {
            get => _vAxis;
            set => _vAxis = value.Normalise();
        }

        public decimal XShift { get; set; }
        public decimal XScale { get; set; }

        public decimal YShift { get; set; }
        public decimal YScale { get; set; }

        public Texture()
        {
            Name = "";
            Rotation = 0;
            _uAxis = -Coordinate.UnitZ;
            _vAxis = Coordinate.UnitX;
            XShift = YShift = 0;
            XScale = YScale = 1;
        }

        protected Texture(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Rotation = info.GetInt32("Rotation");
            _uAxis = (Coordinate)info.GetValue("UAxis", typeof(Coordinate));
            _vAxis = (Coordinate)info.GetValue("VAxis", typeof(Coordinate));
            XShift = info.GetDecimal("XShift");
            XScale = info.GetDecimal("XScale");
            YShift = info.GetDecimal("YShift");
            YScale = info.GetDecimal("YScale");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Rotation", Rotation);
            info.AddValue("UAxis", _uAxis);
            info.AddValue("VAxis", _vAxis);
            info.AddValue("XShift", XShift);
            info.AddValue("XScale", XScale);
            info.AddValue("YShift", YShift);
            info.AddValue("YScale", YScale);
        }

        public Coordinate GetNormal()
        {
            return UAxis.Cross(VAxis).Normalise();
        }

        public void Unclone(Texture source)
        {
            Name = source.Name;
            Rotation = source.Rotation;
            UAxis = source.UAxis.Clone();
            VAxis = source.VAxis.Clone();
            XShift = source.XShift;
            XScale = source.XScale;
            YShift = source.YShift;
            YScale = source.YScale;
        }

        public Texture Clone()
        {
            return new Texture
            {
                Name = Name,
                Rotation = Rotation,
                UAxis = UAxis.Clone(),
                VAxis = VAxis.Clone(),
                XShift = XShift,
                XScale = XScale,
                YShift = YShift,
                YScale = YScale
            };
        }
        
        /// <summary>
        /// Transform this texture in a non-destructive way.
        /// All vertices in the texture must remain unchanged relative to each
        /// other for this transformation to be valid.
        /// </summary>
        /// <param name="matrix">The matrix to transform the texture by</param>
        public void TransformUniform(Matrix matrix)
        {
            #if DEBUG

            // Validate that the transformation is indeed uniform.
            // If it's not, bail out.

            var one = Coordinate.One * matrix;
            var two = Coordinate.One * 2 * matrix;
            if (Math.Abs((two - one).VectorMagnitude() - Coordinate.One.VectorMagnitude()) > 0.0001m)
                throw new InvalidOperationException("Transform isn't uniform!");

            #endif

            // If the determinant isn't 1, then we can't transform safely.
            if (Math.Abs(matrix.Determinant() - 1) > 0.0001m) return;

            var startU = UAxis * matrix;
            var startV = VAxis * matrix;

            var origin = Coordinate.Zero * matrix;
            UAxis = (startU - origin).Normalise();
            VAxis = (startV - origin).Normalise();

            if (XScale != 0 && YScale != 0)
            {
                XShift -= (startU - UAxis).Dot(UAxis) / XScale;
                YShift -= (startV - VAxis).Dot(VAxis) / YScale;
            }
        }

        /// <summary>
        /// Rescale this texture based on the given transformation matrix.
        /// Only scale changes of the transformation are taken into account.
        /// </summary>
        /// <param name="matrix">The transformation matrix</param>
        public void TransformScale(Matrix matrix)
        {
            var startU = UAxis * matrix;
            var startV = VAxis * matrix;

            var zero = Coordinate.Zero * matrix;
            var deltaU = (startU - zero).VectorMagnitude();
            var deltaV = (startV - zero).VectorMagnitude();

            if (deltaU != 0) XScale *= deltaU;
            if (deltaV != 0) YScale *= deltaV;
        }
    }
}