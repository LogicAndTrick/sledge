using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering
{
    public enum LightingType
    {
        None,
        Ambient,
        Lit,
        Fullbright
    }

    public static class GeometricExtensions
    {
        public static Vector3 ToVector3(this Coordinate coordinate)
        {
            return new Vector3((float)coordinate.DX, (float)coordinate.DY, (float)coordinate.DZ);
        }

        public static Coordinate ToCoordinate(this Vector3 vector3)
        {
            return new Coordinate((decimal)vector3.X, (decimal)vector3.Y, (decimal)vector3.Z);
        }

        public static int ToAbgr(this Color color, byte alpha)
        {
            return (alpha << 24) | (color.B << 16) | (color.G << 8) | color.R;
        }
    }

    public abstract class SceneObject
    {
        public int ID { get; internal set; }
        public Scene Scene { get; set; }
        public bool IsVisible { get; set; }
    }

    public abstract class Light : SceneObject
    {
        public Color Color { get; set; }
        public float Intensity { get; set; }
    }

    public class AmbientLight : Light
    {
        public Coordinate Direction { get; set; }

        public AmbientLight(Color color, Coordinate direction, float intensity)
        {
            Color = color;
            Direction = direction.Normalise();
            Intensity = intensity;
        }
    }

    public class PointLight : Light
    {
        public Coordinate Position { get; set; }
        public float Distance { get; set; }
    }

    public interface IOrigin
    {
        Coordinate Origin { get; }
    }

    public abstract class RenderableObject : SceneObject, IOrigin
    {
        public Coordinate Origin { get; set; }
        public LightingType Lighting { get; set; }
        public bool IsWireframe { get; set; }
        public byte Opacity { get; set; }
    }

    public class Line : RenderableObject
    {
        public int Width { get; set; }
        public List<Coordinate> Vertices { get; set; }
    }
    
    // Line / Mesh / Curve
    // Face
    // Sprite
    // Model
    // Displacement
    // Dynamic (water)
}
