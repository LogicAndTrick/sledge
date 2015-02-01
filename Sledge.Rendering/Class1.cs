using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering
{
    public enum Renderer
    {
        OpenGL1,
        OpenGL3,
        DirectX
    }

    public enum MaterialType
    {
        Flat,
        Textured,
        Animated,
        Blended,
        // ? Randomised
    }

    public enum LightingType
    {
        None,
        Ambient,
        Lit,
        Fullbright
    }

    public class Material
    {
        public MaterialType Type { get; set; }
        public Color Color { get; set; }
        public int NumFrames { get; set; }
        public List<string> Frames { get; set; } // ???

        public static Material Flat(Color color)
        {
            return new Material {Type = MaterialType.Flat, Color = color};
        }
    }

    public abstract class SceneObject
    {
        public int ID { get; internal set; }
        public Scene Scene { get; set; }
        public bool IsVisible { get; set; }
    }

    public abstract class Camera
    {
        public abstract Matrix4 GetMatrix();
    }

    public class PerspectiveCamera : Camera
    {
        public int FOV { get; set; }
        public int ClipDistance { get; set; }
        public Coordinate Position { get; set; }
        public Coordinate Direction { get; set; }

        public override Matrix4 GetMatrix()
        {
            throw new NotImplementedException();
        }
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

    public abstract class RenderableObject : SceneObject
    {
        public LightingType Lighting { get; set; }
        public bool IsWireframe { get; set; }
    }

    public class Face : RenderableObject
    {
        public Material Material { get; set; }
        public List<Coordinate> Vertices { get; set; }

        public Face(Material material, List<Coordinate> vertices)
        {
            Material = material;
            Vertices = vertices;
        }
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
