namespace Sledge.Common.Shell.Settings
{
    public interface ISettingEditorFactory
    {
        string OrderHint { get; }
        bool Supports(SettingKey key);
        ISettingEditor CreateEditorFor(SettingKey key);
    }
}