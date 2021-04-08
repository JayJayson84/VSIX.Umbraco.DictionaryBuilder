using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace DictionaryBuilder
{
    internal sealed class MessageBox
    {
        /// <summary>
        /// Shows a message box to the user and returns a <see cref="DialogResult"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="icon">The icon to show on the message box.</param>
        /// <param name="msgButton">The button type.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>A <see cref="DialogResult"/>.</returns>
        public static DialogResult Show(
            IServiceProvider serviceProvider,
            string title,
            string message,
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_INFO,
            OLEMSGBUTTON msgButton = OLEMSGBUTTON.OLEMSGBUTTON_OK,
            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST)
        {
            try
            {
                return (DialogResult)VsShellUtilities.ShowMessageBox(
                    serviceProvider,
                    message,
                    title,
                    icon,
                    msgButton,
                    defaultButton);
            }
            catch
            {
                return DialogResult.Error;
            }
        }
    }
}
