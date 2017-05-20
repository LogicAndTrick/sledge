using System.ComponentModel.Composition;
using System.Drawing;
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
            else if (key.Type == typeof(int) || key.Type == typeof(decimal))
            {
                return true;
            }
            else if (key.Type == typeof(Color))
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
            else if (key.Type == typeof(int) || key.Type == typeof(decimal))
            {
                if (key.EditorType == "Slider") return new SliderEditor();
                else return new NumericEditor();
            }
            else if (key.Type == typeof(Color))
            {
                return new ColorEditor();
            }
            return null;
        }
    }
}