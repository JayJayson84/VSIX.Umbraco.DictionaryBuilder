using System;

namespace Umbraco.Core.Models
{
	/// <summary>
	/// Defines the Umbraco dictionary keys.
	/// </summary>
	public partial class Dictionary
	{
		/// <summary>
		/// Defines the <see cref="Events"/> Umbraco dictionary keys.
		/// </summary>
		public partial class Events
		{
			public static DictionaryKey Key => DictionaryKeys.Events;
			/// <summary>
			/// Defines the <see cref="System"/> Umbraco dictionary keys.
			/// </summary>
			public partial class System
			{
				public static DictionaryKey Key => DictionaryKeys.System;
				public static DictionaryKey DeleteActionDisabled => DictionaryKeys.DeleteActionDisabled;
				public static DictionaryKey UnpublishActionDisabled => DictionaryKeys.UnpublishActionDisabled;
			}
		}
	}
}
