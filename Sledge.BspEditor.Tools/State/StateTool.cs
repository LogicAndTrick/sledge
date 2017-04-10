using System.Collections.Generic;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Tools.State
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
}
