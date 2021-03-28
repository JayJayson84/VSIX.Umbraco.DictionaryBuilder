using System;

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
    }
}
