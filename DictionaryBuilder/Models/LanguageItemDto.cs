using System.Text.RegularExpressions;

namespace DictionaryBuilder.Models
{
    /// <summary>
    /// A data transefer object for UmbracoLanguage table items. 
    /// </summary>
    internal sealed class LanguageItemDto
    {
        public LanguageItemDto() { }

        public LanguageItemDto(int id, string languageISOCode, string languageCultureName, bool isDefaultVariantLang)
        {
            Id = id;
            LanguageISOCode = languageISOCode;
            LanguageCultureName = languageCultureName;
            IsDefaultVariantLang = isDefaultVariantLang;
        }

        public int Id { get; set; }
        public string LanguageISOCode { get; set; }
        public string LanguageCultureName { get; set; }
        public bool IsDefaultVariantLang { get; set; }

        /// <summary>
        /// Santizes the culture name to the language and two letter country code e.g. EnglishUK.
        /// </summary>
        /// <returns>A santised culture language and two letter country code.</returns>
        public string GetSanitisedCultureName()
        {
            var twoLetterCountryCode = string.IsNullOrWhiteSpace(LanguageISOCode)
                ? string.Empty
                : Regex.Match(LanguageISOCode, "[A-Z]{2}$").Value;
            var language = string.IsNullOrWhiteSpace(LanguageCultureName)
                ? string.Empty
                : Regex.Match(LanguageCultureName, "^[^(]*").Value;
            var value = $"{language}{twoLetterCountryCode}";

            return Regex.Replace(value, @"^[0-9]+|[^A-Z0-9]", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}
