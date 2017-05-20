using System.ComponentModel.Composition;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
    [Export(typeof(ISettingEditorFactory))]
    public class ShellEditorsFactory : ISettingEditorFactory
    {
        public bool Supports(SettingKey key)
        {
            if (key.Type == typeof(string))
            {
                return true;
            }
            else if (key.Type.IsEnum)
            {
                return true;
            }
            else if (key.Type == typeof(bool))
            {
                return true;
            }
            return false;
        }

        public ISettingEditor CreateEditorFor(SettingKey key)
        {
            if (key.Type == typeof(string))
            {
                return new TextEditor();
            }
            else if (key.Type.IsEnum)
            {
                return new EnumEditor(key.Type);
            }
            else if (key.Type == typeof(bool))
            {
                return new BooleanEditor();
            }
            return null;
        }
    }
}