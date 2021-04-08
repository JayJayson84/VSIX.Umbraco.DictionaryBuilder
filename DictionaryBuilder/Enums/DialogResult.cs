namespace DictionaryBuilder
{
    /// <summary>
    /// Specifies identifiers to indicate the return value of a dialog box.
    /// </summary>
    internal enum DialogResult
    {
        /// <summary>
        /// If the method succeeds, it returns <see cref="Success"/>.
        /// </summary>
        Success = 0,
        /// <summary>
        /// The dialog return value is <see cref="OK"/> (usually sent from a button labelled OK).
        /// </summary>
        OK = 1,
        /// <summary>
        /// The dialog return value is <see cref="Cancel"/> (usually sent from a button labelled Cancel).
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// The dialog return value is <see cref="Abort"/> (usually sent from a button labelled Abort).
        /// </summary>
        Abort = 3,
        /// <summary>
        /// The dialog return value is <see cref="Retry"/> (usually sent from a button labelled Retry).
        /// </summary>
        Retry = 4,
        /// <summary>
        /// The dialog return value is <see cref="Ignore"/> (usually sent from a button labelled Ignore).
        /// </summary>
        Ignore = 5,
        /// <summary>
        /// The dialog return value is <see cref="Yes"/> (usually sent from a button labelled Yes).
        /// </summary>
        Yes = 6,
        /// <summary>
        /// The dialog return value is <see cref="No"/> (usually sent from a button labelled No).
        /// </summary>
        No = 7,
        /// <summary>
        /// Indicates a referernce to the IVsUIShell interface cannot be retrived from the service provider.
        /// </summary>
        Error = 8
    }
}
