namespace Sledge.Common.Translations
{
    public interface ITranslationStringProvider
    {
        void Translate(object target);
        string GetString(string key);
        string GetSetting(string key);
    }
}