using System.ComponentModel.Composition;

namespace Sledge.Common.Translations
{
    public class AutoTranslateAttribute : ExportAttribute
    {
        public string Namespace { get; set; }

        public AutoTranslateAttribute() : base("AutoTranslate")
        {
        }
    }
}
