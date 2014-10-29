namespace Sledge.Gui.Resources
{
    public class PassthroughStringProvider : IStringProvider
    {
        public string Fetch(string key)
        {
            return key;
        }
    }
}