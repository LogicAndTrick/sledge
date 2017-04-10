using System.Collections.Generic;
using System.Windows.Forms;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.State;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Tools.ExampleStateTool
{
    public class IdleState : IState
    {
        public StateTool Owner { get; set; }

        public IdleState(StateTool owner)
        {
            Owner = owner;
        }

        public IState GetNextState(StateEvent ev)
        {
            if (ev.Action == StateAction.MouseClick && ev.ViewportEvent.Button == MouseButtons.Left)
            {
                var point = ev.Viewport.ProperScreenToWorld(ev.ViewportEvent.Location);
                return new FirstPointDrawnState(Owner, point);
            }
            return this;
        }

        public IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        public IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            yield break;
        }
    }
}