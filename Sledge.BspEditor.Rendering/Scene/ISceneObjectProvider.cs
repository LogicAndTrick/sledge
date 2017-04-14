using System;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Rendering.Scene
{
    public interface ISceneObjectProvider
    {
        event EventHandler<SceneObjectsChangedEventArgs> SceneObjectsChanged;
        Task Initialise();
    }
}