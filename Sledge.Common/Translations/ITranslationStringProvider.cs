namespace Sledge.Common.Translations
{
    /// <summary>
    /// An interface which represents a translation string provider
    /// </summary>
    public interface ITranslationStringProvider
    {
        /// <summary>
        /// Translate an object, using either property injection or <see cref="IManualTranslate"/>, if applicable.
        /// </summary>
        /// <param name="target">The target to translate</param>
        void Translate(object target);

        /// <summary>
        /// Get a transation string from the provider.
        /// </summary>
        /// <param name="path">A path to the string. The path will be joined with periods before lookup.</param>
        /// <returns>The translation string, or null if none was found</returns>
        string GetString(params string[] path);

        /// <summary>
        /// Get a setting transation from the provider.
        /// </summary>
        /// <param name="path">A path to the setting. The path will be joined with periods before lookup.</param>
        /// <returns>The setting translation, or null if none was found</returns>
        string GetSetting(params string[] path);
    }
}