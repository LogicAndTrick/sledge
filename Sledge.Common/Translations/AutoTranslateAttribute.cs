using System.ComponentModel.Composition;

namespace Sledge.Common.Translations
{
    public class AutoTranslateAttribute : ExportAttribute
    {
        public AutoTranslateAttribute() : base("AutoTranslate")
        {
        }
    }
}
