using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.StateTool;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Settings;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.Editor.Tools.ExampleStateTool
{
    public class ExampleStateTool : StateTool.StateTool
    {
        public override void ToolSelected(bool preventHistory)
        {
            CurrentState = new IdleState(this);
            base.ToolSelected(preventHistory);
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Test;
        }

        public override string GetName()
        {
            return "Example State Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Displacement;
        }

        public override string GetContextualHelp()
        {
            return "";
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }

    public class IdleState : IState
    {
        public StateTool.StateTool Owner { get; set; }

        public IdleState(StateTool.StateTool owner)
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

    public class FirstPointDrawnState : IState
    {
        public StateTool.StateTool Owner { get; set; }
        public Coordinate FirstPoint { get; set; }
        public Coordinate SecondPoint { get; set; }

        public FirstPointDrawnState(StateTool.StateTool owner, Coordinate firstPoint)
        {
            Owner = owner;
            FirstPoint = SecondPoint = firstPoint;
        }

        public IState GetNextState(StateEvent ev)
        {
            if (ev.Action == StateAction.MouseMove)
            {
                SecondPoint = ev.Viewport.ProperScreenToWorld(ev.ViewportEvent.Location);
            }
            else if (ev.Action == StateAction.MouseClick && ev.ViewportEvent.Button == MouseButtons.Left)
            {
                return new LineDrawnState(Owner, FirstPoint, ev.Viewport.ProperScreenToWorld(ev.ViewportEvent.Location));
            }
            return this;
        }

        public IEnumerable<SceneObject> GetSceneObjects()
        {
            yield return new Line(Color.Red, FirstPoint.ToVector3(), SecondPoint.ToVector3());
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

    public class LineDrawnState : IState
    {
        public StateTool.StateTool Owner { get; set; }
        public Coordinate FirstPoint { get; set; }
        public Coordinate SecondPoint { get; set; }

        public LineDrawnState(StateTool.StateTool owner, Coordinate firstPoint, Coordinate secondPoint)
        {
            Owner = owner;
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
        }

        public IState GetNextState(StateEvent ev)
        {
            if (ev.Action == StateAction.MouseClick && ev.ViewportEvent.Button == MouseButtons.Left) return new IdleState(Owner);
            return this;
        }

        public IEnumerable<SceneObject> GetSceneObjects()
        {
            yield return new Line(Color.Red, FirstPoint.ToVector3(), SecondPoint.ToVector3());
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
