using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Rendering;
using Sledge.Database.Models;
using Sledge.Editor.Editing;
using Sledge.Editor.History;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools;
using Sledge.Editor.UI;
using Sledge.Editor.Visgroups;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Shaders;
using Sledge.Providers;
using Sledge.Providers.GameData;
using Sledge.Providers.Texture;
using Sledge.Settings;
using Sledge.UI;
using Path = System.IO.Path;

namespace Sledge.Editor.Documents
{
    public class Document
    {
        public string MapFile { get; set; }
        public Map Map { get; set; }

        public Game Game { get; set; }
        public GameData GameData { get; set; }
        public decimal GridSpacing { get; set; }

        public bool HideFaceMask { get; set; }

        private RenderManager Renderer { get; set; }

        public SelectionManager Selection { get; private set; }
        public HistoryManager History { get; private set; }

        private readonly DocumentSubscriptions _subscriptions;

        public Document(string mapFile, Map map, Game game)
        {
            MapFile = mapFile;
            Map = map;
            Game = game;

            _subscriptions = new DocumentSubscriptions(this);

            Selection = new SelectionManager(this);
            History = new HistoryManager(this);

            Renderer = new RenderManager(this);
            GridSpacing = Grid.DefaultSize;
            HideFaceMask = false;

            try
            {
                GameData =  GameDataProvider.GetGameDataFromFiles(game.Fgds.Select(f => f.Path));
            }
            catch(ProviderException)
            {
                // TODO: Error logging
                GameData = new GameData();
            }

            foreach (var wad in game.Wads.OrderBy(x => Path.GetFileName(x.Path)))
            {
                TexturePackage.Load(wad.Path);
            }
            var texList = Map.GetAllTextures();
            TexturePackage.LoadTextureData(texList);

            Map.PostLoadProcess(GameData, TextureHelper.Get);

            if (MapFile != null) Mediator.Publish(EditorMediator.FileOpened, MapFile);
        }

        public void SetActive()
        {
            Editor.Instance.SelectTool(ToolManager.Tools[0]); // todo keep this? cache?

            ViewportManager.Viewports.OfType<Viewport2D>().ToList().ForEach(x => x.RenderContext.Add(new GridRenderable(this, x)));

            MapDisplayLists.RegenerateSelectLists(Selection);
            MapDisplayLists.RegenerateDisplayLists(Map.WorldSpawn.Children, false);

            Renderer.Register(ViewportManager.Viewports);

            ViewportManager.Viewports.ForEach(vp => vp.RenderContext.Add(new ToolRenderable()));
            ViewportManager.AddContext3D(new WidgetLinesRenderable());

            VisgroupManager.SetCurrentDocument(this);

            _subscriptions.Subscribe();
        }

        public void SetInactive()
        {
            // todo save state (camera locations, selected tool)
            ViewportManager.ClearContexts();
            VisgroupManager.SetCurrentDocument(null);
            MapDisplayLists.DeleteLists();

            _subscriptions.Unsubscribe();
        }

        public void StartSelectionTransform()
        {
            // todo selection transform shader
            //foreach (var dl in DisplayLists)
            //{
            //    dl.SetTintSelectListEnabled(false);
            //}
            //UpdateDisplayLists(true);
        }

        public void SetSelectListTransform(Matrix4d matrix)
        {
            //foreach (var dl in DisplayLists)
            //{
            //    dl.SetSelectListTransform(matrix);
            //}
        }

        public void EndSelectionTransform()
        {
            //foreach (var dl in DisplayLists)
            //{
            //    dl.SetSelectListTransform(Matrix4d.Identity);
            //    dl.SetTintSelectListEnabled(true);
            //}
            //UpdateDisplayLists();
        }

        public void UpdateDisplayLists(bool exclude = false)
        {
            Map.PartialPostLoadProcess(GameData, TextureHelper.Get);
            //MapDisplayLists.RegenerateSelectLists(Selection);
            //MapDisplayLists.RegenerateDisplayLists(Map.WorldSpawn.Children, exclude);
            Renderer.Update();
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }

        public void UpdateSelectLists()
        {
            Map.PartialPostLoadProcess(GameData, TextureHelper.Get);
            //MapDisplayLists.RegenerateSelectLists(Selection);
            Renderer.Update();
            ViewportManager.Viewports.ForEach(vp => vp.UpdateNextFrame());
        }
    }

    public class RenderManager
    {
        #region Shaders
        public const string VertexShader = @"#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec3 colour;

const vec3 light1direction = vec3(-1, -2, 3);
const vec3 light2direction = vec3(1, 2, 3);
const vec4 light1intensity = vec4(0.6, 0.6, 0.6, 1.0);
const vec4 light2intensity = vec4(0.3, 0.3, 0.3, 1.0);
const vec4 ambient = vec4(0.5, 0.5, 0.5, 1.0);

smooth out vec4 vertexLighting;
smooth out vec4 vertexColour;
smooth out vec2 texCoord;

uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;

void main()
{
	vec4 cameraPos = cameraMatrix * modelViewMatrix * vec4(position, 1);
	gl_Position = perspectiveMatrix * cameraPos;

    vec3 normalPos = normalize(normal);

    float incidence1 = dot(normalPos, light1direction);
    float incidence2 = dot(normalPos, light2direction);

    incidence1 = clamp(incidence1, 0, 1);
    incidence2 = clamp(incidence2, 0, 1);

	vertexColour = vec4(colour, 1);
    vertexLighting = (vec4(1,1,1,1) * light1intensity * incidence1) * 0.5
                   + (vec4(1,1,1,1) * light2intensity * incidence2) * 0.5
                   + (vec4(1,1,1,1) * ambient);
    texCoord = texture;
}
";

        public const string FragmentShader = @"#version 330

smooth in vec4 vertexColour;
smooth in vec4 vertexLighting;
smooth in vec2 texCoord;

uniform bool isSelected;
uniform bool isWireframe;
uniform bool isTextured;
uniform sampler2D currentTexture;

out vec4 outputColor;
void main()
{
    if (isWireframe) {
        outputColor = vec4(1, 1, 0, 1);
    } else {
        if (isTextured) {
            outputColor = texture2D(currentTexture, texCoord) * vertexLighting;
        } else {
            outputColor = vertexColour * vertexLighting;
        }
        if (isSelected) {
            outputColor = outputColor * vec4(1, 0, 0, 1);
        }
    }
}
";
        #endregion Shaders

        private Document _document;
        private ArrayManager _array;
        public ShaderProgram Shader { get; private set; }

        public RenderManager(Document document)
        {
            _document = document;
            _array = new ArrayManager(document.Map);
            Shader = new ShaderProgram(
                new Shader(ShaderType.VertexShader, VertexShader),
                new Shader(ShaderType.FragmentShader, FragmentShader));

            // Set up default values
            Shader.Bind();
            Shader.Set("perspectiveMatrix", Matrix4.Identity);
            Shader.Set("cameraMatrix", Matrix4.Identity);
            Shader.Set("modelViewMatrix", Matrix4.Identity);
            Shader.Set("isTextured", false);
            Shader.Set("isSelected", false);
            Shader.Set("isWireframe", false);
            Shader.Unbind();
        }

        public void Draw2D(object context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            Shader.Bind();
            Shader.Set("perspectiveMatrix", viewport);
            Shader.Set("cameraMatrix", camera);
            Shader.Set("modelViewMatrix", modelView);
            _array.Draw2D(context, Shader);
            Shader.Unbind();
        }

        public void Draw3D(object context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            Shader.Bind();
            Shader.Set("perspectiveMatrix", viewport);
            Shader.Set("cameraMatrix", camera);
            Shader.Set("modelViewMatrix", modelView);
            _array.Draw3D(context, Shader);
            Shader.Unbind();
        }

        public void Update()
        {
            _array.Update(_document.Map);
        }

        public void Register(IEnumerable<ViewportBase> viewports)
        {
            foreach (var vp in viewports)
            {
                vp.RenderContext.Add(new RenderManagerRenderable(vp, this));
            }
        }
    }
}
