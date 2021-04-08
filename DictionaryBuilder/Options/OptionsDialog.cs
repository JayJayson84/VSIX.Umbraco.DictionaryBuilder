using DictionaryBuilder.UserControls;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DictionaryBuilder
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class OptionsDialog : DialogPage
    {

        #region " Instance Fields "

        public override object AutomationObject => VisualStudioOptions.Instance;

        #endregion

        #region " Protected Methods "

        protected override void OnActivate(CancelEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.OnActivate(e);

            VisualStudioOptions.Instance.Reload();
        }

        protected override IWin32Window Window
        {
            get
            {
                return new OptionsControl().Initialize(VisualStudioOptions.Instance);
            }
        }

        #endregion

        #region " Public Methods "

        public override void SaveSettingsToStorage()
        {
            VisualStudioOptions.Instance.Save();
        }

        public override void LoadSettingsFromStorage()
        {
            VisualStudioOptions.Instance.Load();
        }

        #endregion

    }
}
