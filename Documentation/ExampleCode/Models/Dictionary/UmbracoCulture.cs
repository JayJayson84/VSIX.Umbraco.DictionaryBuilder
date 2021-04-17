using System;
using System.Globalization;

namespace System
{
	/// <summary>
	/// Provides information about a specific Umbraco language culture.
	/// The information includes the culture code (e.g. en-GB) and the equivalent <see cref="Globalization.CultureInfo"/>.
	/// </summary>
	public struct UmbracoCulture
	{
		public static UmbracoCulture Default => new UmbracoCulture("en-GB");

		public readonly string ISO;
		public readonly CultureInfo CultureInfo;

		public UmbracoCulture(string iso)
		{
			ISO = iso;
			try { CultureInfo = new CultureInfo(ISO); } catch { CultureInfo = null; }
		}

		public static implicit operator UmbracoCulture(string iso) => new UmbracoCulture(iso);
		public static implicit operator CultureInfo(UmbracoCulture uc) => uc.CultureInfo;
		public static implicit operator string(UmbracoCulture uc) => uc.ISO;
	}
}

namespace Umbraco.Core.Models
{
	/// <summary>
	/// Defines the Umbraco language cultures.
	/// </summary>
	public partial class UmbracoCultures
	{
		public static readonly UmbracoCulture EnglishGB = "en-GB";
	}
}
