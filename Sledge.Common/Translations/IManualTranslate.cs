namespace Sledge.Common.Translations
{
    /// <summary>
    /// An interface which indicates that a class implements its own
    /// translation method rather than relying on property injection.
    /// </summary>
    public interface IManualTranslate
    {
        /// <summary>
        /// Translate the class
        /// </summary>
        /// <param name="strings">The translation string provider</param>
        void Translate(ITranslationStringProvider strings);
    }
}