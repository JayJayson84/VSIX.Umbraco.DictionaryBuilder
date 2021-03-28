using System.Windows.Forms;

namespace DictionaryBuilder.UserControls
{
    public partial class OptionsControl : UserControl
    {
        public OptionsControl()
        {
            InitializeComponent();
        }

        public OptionsControl Initialize(VisualStudioOptions optionsPage)
        {
            propertyGrid.SelectedObject = optionsPage;

            return this;
        }
    }
}
