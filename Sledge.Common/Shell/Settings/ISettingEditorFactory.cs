namespace Sledge.Common.Shell.Settings
{
    public interface ISettingEditorFactory
    {
        bool Supports(SettingKey key);
        ISettingEditor CreateEditorFor(SettingKey key);
    }
}