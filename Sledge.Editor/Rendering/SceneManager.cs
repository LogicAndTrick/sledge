using Sledge.Rendering;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL;

namespace Sledge.Editor.Rendering
{
    public static class SceneManager
    {
        public static Engine Engine { get; private set; }

        static SceneManager()
        {
            var renderer = new OpenGLRenderer();
            Engine = new Engine(renderer);
        }
    }
}
