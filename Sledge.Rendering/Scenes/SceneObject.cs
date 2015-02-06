namespace Sledge.Rendering.Scenes
{
    public abstract class SceneObject
    {
        public int ID { get; internal set; }
        public Scene Scene { get; set; }
        public bool IsVisible { get; set; }
    }
}