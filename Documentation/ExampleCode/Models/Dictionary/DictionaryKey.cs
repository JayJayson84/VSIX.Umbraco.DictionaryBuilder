using System;

namespace System
{
	/// <summary>
	/// Provides information about a specific Umbraco dicionary key.
	/// The information includes the key value for the dictionary member.
	/// </summary>
	public struct DictionaryKey
	{
		public readonly string Key;

		public bool HasValue => !string.IsNullOrWhiteSpace(Key);

		public DictionaryKey(string key)
		{
			Key = key;
		}

		public static implicit operator DictionaryKey(string key) => new DictionaryKey(key);
		public static implicit operator string(DictionaryKey dk) => dk.Key;
	}
}

namespace Umbraco.Core.Models
{
	/// <summary>
	/// Defines the Umbraco dictionary keys.
	/// </summary>
	public partial class DictionaryKeys
	{
		public static DictionaryKey DeleteActionDisabled = "DeleteActionDisabled";
		public static DictionaryKey Events = "Events";
		public static DictionaryKey System = "System";
		public static DictionaryKey UnpublishActionDisabled = "UnpublishActionDisabled";
	}
}
