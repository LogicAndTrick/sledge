using System;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Common.Shell.Components;

namespace Sledge.BspEditor.Tools
{
    public class ToolSceneObjectProvider : ISceneObjectProvider
    {
        public event EventHandler<SceneObjectsChangedEventArgs> SceneObjectsChanged;

        private WeakReference<BaseTool> _activeTool;

        private BaseTool ActiveTool => _activeTool == null ? null : _activeTool.TryGetTarget(out BaseTool t) ? t : null;
        
        public ToolSceneObjectProvider()
        {
            Oy.Subscribe<ITool>("Tool:Activated", ToolActivated);
        }

        private async Task ToolActivated(ITool tool)
        {
            var at = ActiveTool;
            if (at != null)
            {
                at.ToolDeselected();
                at.SceneObjectsChanged -= ActiveToolSceneObjectsChanged;
            }

            _activeTool = new WeakReference<BaseTool>(tool as BaseTool);

            at = ActiveTool;
            if (at != null)
            {
                at.SceneObjectsChanged += ActiveToolSceneObjectsChanged;
                at.ToolSelected();
            }
        }

        private void ActiveToolSceneObjectsChanged(object sender, SceneObjectsChangedEventArgs e)
        {
            SceneObjectsChanged?.Invoke(this, e);
        }

        public Task Initialise()
        {
            return Task.FromResult(0);
        }
    }
}