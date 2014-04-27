using System.ComponentModel;

namespace Sledge.Settings
{
    public enum RenderMode
    {
        [Description("OpenGL 2.1 (Fastest, requires compatible GPU)")]
        OpenGL3,

        [Description("OpenGL 1.0 Display Lists (Should work for most GPUs)")]
        OpenGL1DisplayLists
    }
}