using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DictionaryBuilder.Models
{
    internal sealed class DictionaryItemDto
    {
        public DictionaryItemDto() { }

        public DictionaryItemDto(Guid id, string key, string value, Guid? parentId, string parentKey)
        {
            Id = id;
            Key = key;
            Value = value;
            ParentId = parentId;
            ParentKey = parentKey;
        }

        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentKey { get; set; }

        /// <summary>
        /// Sanitises the <see cref="Key"/> value into a compatible class or property name using title case e.g. KeyValueAsClassName
        /// </summary>
        /// <returns>A sanitised key value in title case.</returns>
        public string SanitisedKey()
        {
            var key = !Regex.IsMatch(Key, "^[A-Z][a-zA-Z0-9]*$")
                ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Key.ToLower())
                : Key;

            return Regex.Replace(key, @"^[0-9]+|[^A-Z0-9]", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}
