using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class WireframeProperties
    {
        public static readonly WireframeProperties Default = new WireframeProperties();

        public bool DepthTested { get; set; }
        public bool Smooth { get; set; }
        public float Width { get; set; }
        public int StippleFactor { get; set; }
        public ushort StipplePattern { get; set; }

        public bool IsStippled
        {
            get { return StipplePattern != 0xFFFF; }
            set
            {
                StippleFactor = value ? 5 : 1;
                StipplePattern = (ushort)(value ? 0xAAAA : 0xFFFF);
            }
        }

        public WireframeProperties()
        {
            DepthTested = true;
            Smooth = false;
            Width = 1;
            StippleFactor = 1;
            StipplePattern = 0xFFFF;
        }

        public void Bind()
        {
            if (DepthTested) GL.Enable(EnableCap.DepthTest);
            else GL.Disable(EnableCap.DepthTest);

            if (Smooth)
            {
                GL.Enable(EnableCap.LineSmooth);
                GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            }
            else
            {
                GL.Disable(EnableCap.LineSmooth);
            }

            GL.LineWidth(Width);

            if (IsStippled)
            {
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple(StippleFactor, StipplePattern);
            }
            else
            {
                GL.Disable(EnableCap.LineStipple);
            }
        }

        public static void RestoreDefaults()
        {
            Default.Bind();
        }

        protected bool Equals(WireframeProperties other)
        {
            return DepthTested.Equals(other.DepthTested) && Smooth.Equals(other.Smooth) && Width.Equals(other.Width) && StippleFactor == other.StippleFactor && StipplePattern == other.StipplePattern;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WireframeProperties) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DepthTested.GetHashCode();
                hashCode = (hashCode * 397) ^ Smooth.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ StippleFactor;
                hashCode = (hashCode * 397) ^ StipplePattern.GetHashCode();
                return hashCode;
            }
        }
    }
}