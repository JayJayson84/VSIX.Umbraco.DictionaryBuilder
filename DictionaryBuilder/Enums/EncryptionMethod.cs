namespace DictionaryBuilder
{
    /// <summary>
    /// Specifies identifiers to indicate the type of EncryptionKey to use when storing passwords.
    /// </summary>
    public enum EncryptionMethod
    {
        /// <summary>
        /// Indicates a <see cref="Common"/> shared encryption key should be used.
        /// </summary>
        Common,
        /// <summary>
        /// Indicates a <see cref="System"/> specific encryption key should be used.
        /// </summary>
        System
    }
}
