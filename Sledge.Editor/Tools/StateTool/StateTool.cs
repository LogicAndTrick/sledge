using System.Collections.Generic;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools.StateTool
{
    public abstract class StateTool : BaseTool
    {
        public IState CurrentState { get; set; }


        public override void MouseDown(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            CurrentState = CurrentState.GetNextState(new StateEvent(viewport, e, StateAction.MouseDown));
        }

        public override void MouseClick(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            CurrentState = CurrentState.GetNextState(new StateEvent(viewport, e, StateAction.MouseClick));
        }

        public override void MouseUp(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            CurrentState = CurrentState.GetNextState(new StateEvent(viewport, e, StateAction.MouseUp));
        }

        public override void MouseMove(MapViewport viewport, ViewportEvent e)
        {
            if (!Active) return;
            CurrentState = CurrentState.GetNextState(new StateEvent(viewport, e, StateAction.MouseMove));
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            return CurrentState.GetSceneObjects();
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            return CurrentState.GetViewportElements(viewport, camera);
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            return CurrentState.GetViewportElements(viewport, camera);
        }
    }

    public enum StateAction
    {
        None,
        MouseUp,
        MouseDown,
        MouseClick,
        MouseMove
    }

    public class StateEvent
    {
        public MapViewport Viewport { get; set; }
        public ViewportEvent ViewportEvent { get; set; }
        public StateAction Action { get; set; }

        public StateEvent(MapViewport viewport, ViewportEvent viewportEvent, StateAction action)
        {
            Viewport = viewport;
            ViewportEvent = viewportEvent;
            Action = action;
        }
    }

    public interface IState
    {
        StateTool Owner { get; set; }

        IState GetNextState(StateEvent ev);
        IEnumerable<SceneObject> GetSceneObjects();
        IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera);
        IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera);
    }
}
