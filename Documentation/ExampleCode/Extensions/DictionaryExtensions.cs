using System;
using System.Globalization;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services.Implement;

namespace Umbraco.Core.Services
{
	/// <summary>
	/// Provides extension methods to the <see cref="DictionaryService"/> class.
	/// </summary>
	public static class DictionaryServiceExtensions
	{
		/// <summary>
		/// Gets the value of the <paramref name="dictionaryItem"/> with the matching <paramref name="culture"/>.
		/// </summary>
		/// <param name="dictionaryItem">An <see cref="IDictionaryItem"/> item.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the <paramref name="dictionaryItem"/> with matching <paramref name="culture"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="UmbracoCulture"/> will be used.</remarks>
		public static string GetLocaleValue(this IDictionaryItem dictionaryItem, CultureInfo culture = null)
		{
			return GetFormattedLocaleValue(dictionaryItem, null, culture);
		}

		/// <summary>
		/// Gets the formatted value of the <paramref name="dictionaryItem"/> with the matching <paramref name="culture"/>.
		/// </summary>
		/// <param name="dictionaryItem">An <see cref="IDictionaryItem"/> item.</param>
		/// <param name="formatArgs">An array of string representation to replace within a composite format string.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the <paramref name="dictionaryItem"/> with matching <paramref name="culture"/> and format items replaced by the representation <paramref name="formatArgs"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="UmbracoCulture"/> will be used.</remarks>
		public static string GetFormattedLocaleValue(this IDictionaryItem dictionaryItem, string[] formatArgs, CultureInfo culture = null)
		{
			if (dictionaryItem == null) return null;

			culture = culture ?? UmbracoCulture.Default;

			var translation = dictionaryItem.Translations.SingleOrDefault(x => x.Language.CultureInfo.Equals(culture));
			if (translation == null) return null;

			return formatArgs == null
				? translation.Value
				: string.Format(translation.Value, formatArgs);
		}
	}
}
