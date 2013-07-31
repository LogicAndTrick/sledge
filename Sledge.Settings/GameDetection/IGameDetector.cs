namespace Sledge.Settings.GameDetection
{
    public interface IGameDetector
    {
        string Name { get; }

        void Detect();
    }
}
