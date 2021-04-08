using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace DictionaryBuilder
{
    /// <summary>
    /// Helper methods to write status strings to the output window.
    /// </summary>
    internal sealed class LogHelper
    {

        #region " Constants "

        private const string paneId = "68EE7B36-CFEE-4773-80D6-EFB293416019";
        private const string paneTitle = "DictionaryBuilder";

        #endregion

        #region " Public Methods "

        /// <summary>
        /// Writes an opening timestamp log entry to the output window along with the provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to write. A newline will be suffixed to the message if not already provided.</param>
        /// <param name="activate"><see langword="true"/> to show and activate the Output window pane. Otherwise <see langword="false"/>.</param>
        public static void WriteFirstLog(string message, bool activate = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ClearWindow();

            message = $"Operation Started on {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n\n{(!message.EndsWith("\n") ? message + "\n" : message)}";

            LogToWindow(message, activate);
        }

        /// <summary>
        /// Writes a closing timestamp log entry to the output window along with the provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to write. A newline will be suffixed to the message if not already provided.</param>
        /// <param name="activate"><see langword="true"/> to show and activate the Output window pane. Otherwise <see langword="false"/>.</param>
        public static void WriteLastLog(string message, bool activate = false)
        {
            message = $"{(!message.EndsWith("\n") ? message + "\n" : message)}\nOperation completed on {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            LogToWindow(message, activate);
        }

        /// <summary>
        /// Writes a log entry to the output window.
        /// </summary>
        /// <param name="message">The message to write. A newline will be suffixed to the message if not already provided.</param>
        /// <param name="activate"><see langword="true"/> to show and activate the Output window pane. Otherwise <see langword="false"/>.</param>
        public static void LogToWindow(string message, bool activate = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var paneGuid = new Guid(paneId);

            var pane = GetPane(paneGuid, paneTitle, true, true);

            if (pane == null) return;

            message = !message.EndsWith("\n")
                ? message + "\n"
                : message;

            pane.OutputString(message);

            if (activate) pane.Activate();
        }

        /// <summary>
        /// Removes all text from Output window pane.
        /// </summary>
        public static void ClearWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var paneGuid = new Guid(paneId);

            var pane = GetPane(paneGuid, paneTitle, true, true);

            if (pane == null) return;

            pane.Clear();
        }

        /// <summary>
        /// Creates and/or returns an Output window pane, given its identifying GUID.
        /// </summary>
        /// <param name="paneGuid">The GUID of the Output window pane.</param>
        /// <param name="title">The title given to the pane if it does not already exist.</param>
        /// <param name="visible">If <see langword="true"/> the Output window pane is initially visible when created.</param>
        /// <param name="clearWithSolution">If <see langword="true"/> the Output window pane is cleared when the solution closes.</param>
        /// <returns>An <see cref="IVsOutputWindowPane"/> reference for the matching pane.</returns>
        /// <remarks>A new Output window pane will be created using the <paramref name="paneGuid"/> and <paramref name="title"/> if it does not already exist.</remarks>
        public static IVsOutputWindowPane GetPane(Guid paneGuid, string title, bool visible, bool clearWithSolution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(Package.GetGlobalService(typeof(SVsOutputWindow)) is IVsOutputWindow outputWindow)) return null;
            
            if (outputWindow.GetPane(ref paneGuid, out var outputPane) == VSConstants.S_OK) return outputPane;
            outputWindow.CreatePane(
                ref paneGuid,
                title,
                Convert.ToInt32(visible),
                Convert.ToInt32(clearWithSolution));

            if (outputWindow.GetPane(ref paneGuid, out outputPane) != VSConstants.S_OK) return null;

            return outputPane;
        }

        #endregion

    }
}
