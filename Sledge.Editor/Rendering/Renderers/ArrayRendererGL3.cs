using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.DataStructures.Rendering;
using Sledge.Editor.Documents;
using Sledge.Editor.UI;
using Sledge.Graphics.Renderables;
using Sledge.Graphics.Shaders;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Renderers
{
    public class ArrayRendererGL3 : IRenderer
    {
        #region Shaders
        public const string VertexShader = @"#version 120

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec4 colour;
layout(location = 4) in float selected;

const vec3 light1direction = vec3(-1, -2, 3);
const vec3 light2direction = vec3(1, 2, 3);
const vec4 light1intensity = vec4(0.6, 0.6, 0.6, 1.0);
const vec4 light2intensity = vec4(0.3, 0.3, 0.3, 1.0);
const vec4 ambient = vec4(0.5, 0.5, 0.5, 1.0);

varying vec4 worldPosition;
varying vec4 worldNormal;
varying vec4 vertexLighting;
varying vec4 vertexColour;
varying vec2 texCoord;
varying float vertexSelected;

uniform bool isWireframe;
uniform bool drawUntransformed;
uniform bool drawSelectedOnly;
uniform bool drawUnselectedOnly;
uniform bool in3d;
uniform bool showGrid;
uniform float gridSpacing;

uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;
uniform mat4 selectionTransform;
uniform mat4 inverseSelectionTransform;

void main()
{
    vec4 pos = vec4(position, 1);
    if (selected > 0.9 && !drawUntransformed) pos = selectionTransform * pos;
    vec4 modelPos = modelViewMatrix * pos;
    
	vec4 cameraPos = cameraMatrix * modelPos;
	gl_Position = perspectiveMatrix * cameraPos;

    vec4 npos = vec4(normal, 1);
    // http://www.arcsynthesis.org/gltut/Illumination/Tut09%20Normal%20Transformation.html
    if (selected > 0.9 && !drawUntransformed) npos = transpose(inverseSelectionTransform) * npos;
    vec3 normalPos = normalize(npos.xyz);
    npos = vec4(normalPos, 1);

    worldPosition = pos;
    worldNormal = npos;

    float incidence1 = dot(normalPos, light1direction);
    float incidence2 = dot(normalPos, light2direction);

    incidence1 = clamp(incidence1, 0, 1);
    incidence2 = clamp(incidence2, 0, 1);

	vertexColour = colour;
    vertexLighting = (vec4(1,1,1,1) * light1intensity * incidence1) * 0.5
                   + (vec4(1,1,1,1) * light2intensity * incidence2) * 0.5
                   + (vec4(1,1,1,1) * ambient);
    vertexLighting.w = 1.0; // Reset the alpha channel or transparency gets messed up later
    texCoord = texture;
    vertexSelected = selected;
}
";

        public const string FragmentShader = @"#version 120

varying vec4 worldPosition;
varying vec4 worldNormal;
varying vec4 vertexColour;
varying vec4 vertexLighting;
varying vec2 texCoord;
varying float vertexSelected;

uniform bool isWireframe;
uniform bool drawUntransformed;
uniform bool drawSelectedOnly;
uniform bool drawUnselectedOnly;
uniform bool isTextured;
uniform vec4 wireframeColour;
uniform vec4 selectedWireframeColour;
uniform vec4 selectionColourMultiplier;
uniform bool in3d;
uniform bool showGrid;
uniform float gridSpacing;
uniform sampler2D currentTexture;

void main()
{
    vec4 outputColor;
    float alpha = vertexColour.w;
    if (drawSelectedOnly && vertexSelected <= 0.9) discard;
    if (drawUnselectedOnly && vertexSelected > 0.9) discard;
    if (isWireframe) {
        if (!in3d && vertexSelected > 0.9) outputColor = selectedWireframeColour;
        else if (wireframeColour.w == 0) outputColor = vertexColour;
        else outputColor = wireframeColour;
    } else {
        if (isTextured) {
            vec4 texColour = texture2D(currentTexture, texCoord);
            outputColor = texColour * vertexLighting;
            if (texColour.w < alpha) alpha = texColour.w;
        } else {
            outputColor = vertexColour * vertexLighting;
        }
        if (vertexSelected > 0.9) {
            outputColor = outputColor * selectionColourMultiplier; //vec4(1, 0.5, 0.5, 1);
        }
    }
    if (in3d && showGrid) {
        if (abs(worldNormal).x < 0.9999) outputColor = mix(outputColor, vec4(1, 0, 0, 1), step(mod(worldPosition.x, gridSpacing), 0.5));
        if (abs(worldNormal).y < 0.9999) outputColor = mix(outputColor, vec4(0, 1, 0, 1), step(mod(worldPosition.y, gridSpacing), 0.5));
        if (abs(worldNormal).z < 0.9999) outputColor = mix(outputColor, vec4(0, 0, 1, 1), step(mod(worldPosition.z, gridSpacing), 0.5));
    }
    outputColor.w = alpha;
    gl_FragColor = outputColor;
}
";
        #endregion Shaders

        public string Name { get { return "OpenGL 3.0 Renderer"; } }
        public Document Document { get { return _document; } set { _document = value; } }

        private Document _document;
        private MapObjectArray _array;
        private ShaderProgram Shader { get; set; }

        #region Shader Variables

        private bool Show3DGrid { set { Shader.Set("showGrid", value); } }
        private float GridSpacing { set { Shader.Set("gridSpacing", value); } }

        private Matrix4 Perspective { set { Shader.Set("perspectiveMatrix", value); } }
        private Matrix4 Camera { set { Shader.Set("cameraMatrix", value); } }
        private Matrix4 ModelView { set { Shader.Set("modelViewMatrix", value); } }

        public Matrix4 SelectionTransform
        {
            set
            {
                Shader.Set("selectionTransform", value);
                Shader.Set("inverseSelectionTransform", Matrix4.Invert(value));
            }
        }

        private bool IsTextured { set { Shader.Set("isTextured", value); } }
        private bool IsWireframe { set { Shader.Set("isWireframe", value); } }
        private bool DrawUntransformed { set { Shader.Set("drawUntransformed", value); } }
        private bool DrawSelectedOnly { set { Shader.Set("drawSelectedOnly", value); } }
        private bool DrawUnselectedOnly { set { Shader.Set("drawUnselectedOnly", value); } }
        private bool In3D { set { Shader.Set("in3d", value); } }
        private Vector4 WireframeColour { set { Shader.Set("wireframeColour", value); } }
        private Vector4 SelectedWireframeColour { set { Shader.Set("selectedWireframeColour", value); } }
        private Vector4 SelectionColourMultiplier { set { Shader.Set("selectionColourMultiplier", value); } }

        #endregion

        private Dictionary<ViewportBase, GridRenderable> GridRenderables { get; set; }

        public ArrayRendererGL3(Document document)
        {
            _document = document;

            var all = GetAllVisible(document.Map.WorldSpawn);
            _array = new MapObjectArray(all);

            Shader = new ShaderProgram(
                new Shader(ShaderType.VertexShader, VertexShader),
                new Shader(ShaderType.FragmentShader, FragmentShader));
            GridRenderables = ViewportManager.Viewports.OfType<Viewport2D>().ToDictionary(x => (ViewportBase)x, x => new GridRenderable(_document));

            // Set up default values
            Shader.Bind();
            Perspective = Camera = ModelView = SelectionTransform = Matrix4.Identity;
            IsTextured = IsWireframe = In3D = false;
            Show3DGrid = document.Map.Show3DGrid;
            GridSpacing = (float) document.Map.GridSpacing;
            WireframeColour = Vector4.Zero;
            SelectionColourMultiplier = new Vector4(1, 0.5f, 0.5f, 1);
            Shader.Unbind();
        }

        public void Dispose()
        {
            _array.Dispose();
            Shader.Dispose();
        }

        public void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D)
        {
            Shader.Bind();
            GridSpacing = (float)gridSpacing;
            Show3DGrid = showIn3D;
            Shader.Unbind();

            foreach (var kv in GridRenderables)
            {
                kv.Value.RebuildGrid(((Viewport2D)kv.Key).Zoom);
            }
        }

        public void SetSelectionTransform(Matrix4 selectionTransform)
        {
            Shader.Bind();
            SelectionTransform = selectionTransform;
            Shader.Unbind();
        }

        public void Draw2D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            if (GridRenderables.ContainsKey(context)) GridRenderables[context].Render(context);

            Shader.Bind();
            Perspective = viewport;
            Camera = camera;
            IsTextured = false;
            IsWireframe = true;
            WireframeColour = Vector4.Zero;
            In3D = false;
            ModelView = modelView;
            DrawUntransformed = true;
            DrawSelectedOnly = true;
            SelectedWireframeColour = new Vector4(0.6f, 0, 0, 1);
            _array.RenderWireframe(context.Context, Shader);

            DrawUntransformed = false;
            DrawSelectedOnly = false;
            SelectedWireframeColour = new Vector4(1, 0, 0, 1);
            _array.RenderWireframe(context.Context, Shader);

            Shader.Unbind();
        }

        public void Draw3D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            Shader.Bind();

            Perspective = viewport;
            Camera = camera;
            ModelView = modelView;
            IsTextured = true;
            IsWireframe = false;
            In3D = true;
            DrawUntransformed = false;
            DrawSelectedOnly = false;
            SelectionColourMultiplier = _document.Selection.InFaceSelection && _document.Map.HideFaceMask ? Vector4.One : new Vector4(1, 0.5f, 0.5f, 1);

            var cam = ((Viewport3D)context).Camera.Location;
            var location = new Coordinate((decimal)cam.X, (decimal)cam.Y, (decimal)cam.Z);

            GL.ActiveTexture(TextureUnit.Texture0);
            Shader.Set("currentTexture", 0);
            _array.RenderTextured(context.Context, Shader, location);
            // todo render helpers...

            _array.RenderTransparent(context.Context, Shader, location);
            
            DrawUntransformed = true;
            DrawSelectedOnly = true;
            IsWireframe = true;
            WireframeColour = new Vector4(1, 1, 0, 1);
            SelectedWireframeColour = new Vector4(1, 0, 0, 1);

            _array.RenderWireframe(context.Context, Shader);
            
            Shader.Unbind();
        }

        public void Update()
        {
            var all = GetAllVisible(Document.Map.WorldSpawn);
            _array.Update(all);
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _array.UpdatePartial(objects);
            //_array.UpdateDecals(_document.Map);
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _array.UpdatePartial(faces);
            //_array.UpdateDecals(_document.Map);
        }

        public IRenderable CreateRenderable(Model model)
        {
            throw new NotImplementedException();
        }

        public void UpdateDocumentToggles()
        {
            // Not needed
        }

        private static IEnumerable<MapObject> GetAllVisible(MapObject root)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            return list.Where(x => !x.IsCodeHidden).ToList();
        }

        private static void FindRecursive(ICollection<MapObject> items, MapObject root, Predicate<MapObject> matcher)
        {
            if (!matcher(root)) return;
            items.Add(root);
            root.Children.ForEach(x => FindRecursive(items, x, matcher));
        }
    }
}
