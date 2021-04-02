using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DictionaryBuilder.Models
{
    /// <summary>
    /// A data transefer object for the CMSDictionary table. 
    /// </summary>
    internal sealed class DictionaryDto : Collection<DictionaryItemDto>
    {
        /// <summary>
        /// Adds an object to the end of the <see cref="Collection{T}"/>.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> field value.</param>
        /// <param name="key">The <paramref name="key"/> field value.</param>
        /// <param name="value">The <paramref name="value"/> field value.</param>
        /// <param name="parentId">The <paramref name="parentId"/> field value.</param>
        /// <param name="parentKey">The <paramref name="parentKey"/> field value.</param>
        public void Add(Guid id, string key, string value, Guid? parentId, string parentKey)
        {
            Add(new DictionaryItemDto(id, key, value, parentId, parentKey));
        }

        /// <summary>
        /// Gets an array of Keys from the dictionary.
        /// </summary>
        public string[] Keys => Items?
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => x.Key)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        /// <summary>
        /// Gets all <see cref="DictionaryItemDto"/> items ordered by key.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{DictionaryItemDto}"/> of all items ordered by key.</returns>
        public IEnumerable<DictionaryItemDto> OrderedItems => Items.OrderBy(x => x.Key);

        /// <summary>
        /// Gets an <see cref="IEnumerable{DictionaryItemDto}"/> of top level dictionary records.
        /// </summary>
        public IEnumerable<DictionaryItemDto> RootItems => Items?
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.Key);

        /// <summary>
        /// Gets descendant <see cref="DictionaryItemDto"/> records beneath the given <paramref name="Id"/>.
        /// </summary>
        /// <param name="Id">The <paramref name="Id"/> of the parent record to traverse.</param>
        /// <returns>An <see cref="IEnumerable{DictionaryItemDto}"/> of records beneath the given <paramref name="Id"/>.</returns>
        public IEnumerable<DictionaryItemDto> ItemsById(Guid Id)
        {
            return Items?
                .Where(x => x.ParentId.HasValue && x.ParentId.Value == Id)
                .OrderBy(x => x.Key);
        }

        /// <summary>
        /// Returns a value indicating whether the current <paramref name="item"/> contains child records.
        /// </summary>
        /// <param name="item">A <see cref="DictionaryItemDto"/> object.</param>
        /// <returns><see langword="true"/> if the current <paramref name="item"/> contains child records. Otherwise <see langword="false"/>.</returns>
        public bool HasChildren(DictionaryItemDto item)
        {
            return Items?.Where(x => x.ParentId.HasValue && x.ParentId.Value == item.Id).Count() != 0;
        }
    }
}
