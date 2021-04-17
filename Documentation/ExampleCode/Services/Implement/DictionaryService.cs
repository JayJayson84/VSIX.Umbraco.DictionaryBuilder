using System;
using System.Globalization;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Umbraco.Core.Services.Implement
{
	/// <summary>
	/// Defines the Dictionary Service, which provides operations to easily retrieve localised Dictionary members.
	/// </summary>
	public class DictionaryService : IDictionaryService
	{

		#region " Instance Fields "

		private readonly ILocalizationService _localizationService;

		#endregion

		#region " Constructors "

		/// <summary>
		/// Defines the Dictionary Service, which provides operations to easily retrieve localised Dictionary members.
		/// </summary>
		public DictionaryService()
		{
			_localizationService = Current.Factory.GetInstance<ILocalizationService>();

			Culture = Culture ?? UmbracoCulture.Default;
		}

		/// <summary>
		/// Defines the Dictionary Service, which provides operations to easily retrieve localised Dictionary members.
		/// </summary>
		/// <param name="localizationService">A reference to the <see cref="ILocalizationService"/> instance.</param>
		public DictionaryService(ILocalizationService localizationService)
		{
			_localizationService = localizationService;
		}

		/// <summary>
		/// Defines the Dictionary Service, which provides operations to easily retrieve localised Dictionary members.
		/// </summary>
		/// <param name="culture">The <see cref="CultureInfo"/> to use when getting localised dictionary values.</param>
		public DictionaryService(CultureInfo culture) : this()
		{
			Culture = culture ?? UmbracoCulture.Default;
		}

		/// <summary>
		/// Defines the Dictionary Service, which provides operations to easily retrieve localised Dictionary members.
		/// </summary>
		/// <param name="cultureCode">The language/region (e.g. en-UK) of the <see cref="CultureInfo"/> to use when getting localised dictionary values.</param>
		public DictionaryService(string cultureCode) : this()
		{
			CultureCode = cultureCode;
		}

		#endregion

		#region " Properties "

		/// <summary>
		/// Gets or sets the <see cref="CultureInfo"/> to use when getting localised dictionary values.
		/// </summary>
		public CultureInfo Culture
		{
			get => _culture;
			set => _culture = value ?? UmbracoCulture.Default;
		}
		private CultureInfo _culture = UmbracoCulture.Default;

		/// <summary>
		/// Gets or sets the <see cref="Culture"/> language/region e.g. en-UK.
		/// </summary>
		public string CultureCode
		{
			get
			{
				return Culture.Name;
			}
			set
			{
				try
				{
					Culture = string.IsNullOrWhiteSpace(value)
						? UmbracoCulture.Default
						: new CultureInfo(value);
				}
				catch
				{
					Culture = UmbracoCulture.Default;
				}
			}
		}

		#endregion

		#region " Public Methods "

		/// <summary>
		/// Gets an <see cref="IDictionaryItem"/> from the description attribute of a <see cref="DictionaryKey"/> enum.
		/// </summary>
		/// <param name="key">A <see cref="DictionaryKey"/> member.</param>
		/// <returns>An <see cref="IDictionaryItem"/> item.</returns>
		public IDictionaryItem GetDictionaryItemByKey(DictionaryKey key)
		{
			if (!key.HasValue) return null;

			return _localizationService.GetDictionaryItemByKey(key);
		}

		/// <summary>
		/// Gets an <see cref="IDictionaryItem"/> by it's key.
		/// </summary>
		/// <param name="key">The dictionary key value.</param>
		/// <returns>An <see cref="IDictionaryItem"/> item.</returns>
		public IDictionaryItem GetDictionaryItemByKey(string key)
		{
			return _localizationService.GetDictionaryItemByKey(key);
		}

		/// <summary>
		/// Gets the value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">A <see cref="DictionaryKey"/> member.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="UmbracoCulture"/> will be used.</remarks>
		public string GetLocaleValueByKey(DictionaryKey key, CultureInfo culture = null)
		{
			if (!key.HasValue) return null;

			return GetFormattedLocaleValueByKey(key, null, culture);
		}

		/// <summary>
		/// Gets the value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">The dictionary key.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="UmbracoCulture"/> will be used.</remarks>
		public string GetLocaleValueByKey(string key, CultureInfo culture = null)
		{
			return GetFormattedLocaleValueByKey(key, null, culture);
		}

		/// <summary>
		/// Gets the formatted value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">A <see cref="DictionaryKey"/> member.</param>
		/// <param name="formatArgs">An array of string representation to replace within a composite format string.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/> and format items replaced by the representation <paramref name="formatArgs"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="UmbracoCulture"/> will be used.</remarks>
		public string GetFormattedLocaleValueByKey(DictionaryKey key, string[] formatArgs, CultureInfo culture = null)
		{
			if (!key.HasValue) return null;

			return _localizationService
				.GetDictionaryItemByKey(key)
				.GetFormattedLocaleValue(formatArgs, culture ?? Culture);
		}

		/// <summary>
		/// Gets the formatted value of the dictionary item with the matching <paramref name="key"/> and <paramref name="culture"/>.
		/// </summary>
		/// <param name="key">The dictionary key.</param>
		/// <param name="formatArgs">An array of string representation to replace within a composite format string.</param>
		/// <param name="culture">Optionally specify the <see cref="CultureInfo"/> of the dictionary value to return.</param>
		/// <returns>A locale <see cref="string"/> for the dictionary item with matching <paramref name="key"/> and <paramref name="culture"/> and format items replaced by the representation <paramref name="formatArgs"/>. Otherwise <see langword="null"/>.</returns>
		/// <remarks>If a <paramref name="culture"/> is not provided, the project's default <see cref="UmbracoCulture"/> will be used.</remarks>
		public string GetFormattedLocaleValueByKey(string key, string[] formatArgs, CultureInfo culture = null)
		{
			return _localizationService
				.GetDictionaryItemByKey(key)
				.GetFormattedLocaleValue(formatArgs, culture ?? Culture);
		}

		#endregion

	}
}
