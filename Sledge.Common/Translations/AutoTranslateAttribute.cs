using System.ComponentModel.Composition;

namespace Sledge.Common.Translations
{
    /// <summary>
    /// An attribute that indicates that the class should be automatically translated.
    /// This attribute will export the class to the MEF container.
    /// </summary>
    public class AutoTranslateAttribute : ExportAttribute
    {
        public AutoTranslateAttribute() : base("AutoTranslate")
        {
        }
    }
}
