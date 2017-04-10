using System.Collections.Generic;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Tools.State
{
    public interface IState
    {
        StateTool Owner { get; set; }

        IState GetNextState(StateEvent ev);
        IEnumerable<SceneObject> GetSceneObjects();
        IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera);
        IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera);
    }
}