namespace DictionaryBuilder
{
    /// <summary>
    /// Specifies identifiers to indicate the type of SQL connection.
    /// </summary>
    public enum SqlConnectionType
    {
        /// <summary>
        /// Indicates a connection type is not provided.
        /// </summary>
        None,
        /// <summary>
        /// Indicates a <see cref="Development"/> SQL connection should be used.
        /// </summary>
        Development,
        /// <summary>
        /// Indicates a <see cref="Staging"/> SQL connection should be used.
        /// </summary>
        Staging,
        /// <summary>
        /// Indicates a <see cref="Live"/> SQL connection should be used.
        /// </summary>
        Live
    }
}
