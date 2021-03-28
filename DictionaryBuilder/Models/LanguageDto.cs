using System.Collections.ObjectModel;
using System.Linq;

namespace DictionaryBuilder.Models
{
    /// <summary>
    /// A data transefer object for the UmbracoLanguage table. 
    /// </summary>
    internal sealed class LanguageDto : Collection<LanguageItemDto>
    {
        public void Add(int id, string languageISOCode, string languageCultureName, bool isDefaultVariantLang)
        {
            Add(new LanguageItemDto(id, languageISOCode, languageCultureName, isDefaultVariantLang));
        }

        /// <summary>
        /// Gets the default <see cref="LanguageDto"/> record.
        /// </summary>
        /// <returns>The default <see cref="LanguageDto"/> record.</returns>
        public LanguageItemDto GetDefaultLanguage()
        {
            return Items?.FirstOrDefault(x => x.IsDefaultVariantLang);
        }
    }
}
