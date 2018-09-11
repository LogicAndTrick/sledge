namespace Sledge.Common.Translations
{
    public interface ITranslationStringProvider
    {
        void Translate(object target);
        string GetString(params string[] path);
        string GetSetting(params string[] path);
    }
}