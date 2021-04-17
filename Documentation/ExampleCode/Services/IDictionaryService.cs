using System;
using System.Globalization;
using Umbraco.Core.Models;

namespace Umbraco.Core.Services
{
	/// <summary>
	/// Defines the Dictionary Service, which provides operations to easily retrieve localised Dictionary members.
	/// </summary>
	public interface IDictionaryService
	{
		/// <summary>
		/// Gets an <see cref="IDictionaryItem"/> from the description attribute of a <see cref="DictionaryKey"/> enum.
		/// </summary>
		/// <param name="key">A <see cref="DictionaryKey"/> member.</param>
		/// <returns>An <see cref="IDictionaryItem"/> item.</returns>
		IDictionaryItem GetDictionaryItemByKey(DictionaryKey key);

		/// <summary>
		/// Gets an <see cref="IDictionaryItem"/> by it's key.
		/// </summary>
		/// <param name="key">The dictionary key value.</param>
		/// <returns>An <see cref="IDictionaryItem"/> item.</returns>
		IDictionaryItem GetDictionaryItemByKey(string key);

		/// <summary>
		/// Gets the value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">A <see cref="DictionaryKey"/> member.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="CultureInfo"/> will be used.</remarks>
		string GetLocaleValueByKey(DictionaryKey key, CultureInfo culture = null);

		/// <summary>
		/// Gets the value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">The dictionary key.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="CultureInfo"/> will be used.</remarks>
		string GetLocaleValueByKey(string key, CultureInfo culture = null);

		/// <summary>
		/// Gets the formatted value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">A <see cref="DictionaryKey"/> member.</param>
		/// <param name="formatArgs">An array of string representation to replace within a composite format string.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/> and format items replaced by the representation <paramref name="formatArgs"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="CultureInfo"/> will be used.</remarks>
		string GetFormattedLocaleValueByKey(DictionaryKey key, string[] formatArgs, CultureInfo culture = null);

		/// <summary>
		/// Gets the formatted value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">The dictionary key.</param>
		/// <param name="formatArgs">An array of string representation to replace within a composite format string.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/> and format items replaced by the representation <paramref name="formatArgs"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="CultureInfo"/> will be used.</remarks>
		string GetFormattedLocaleValueByKey(string key, string[] formatArgs, CultureInfo culture = null);
	}
}
