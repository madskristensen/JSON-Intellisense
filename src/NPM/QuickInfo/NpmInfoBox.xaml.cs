using System.Windows.Controls;

namespace JSON_Intellisense
{
    public partial class NpmInfoBox : UserControl
    {
        private static NpmInfoBox _box = new NpmInfoBox();

        public NpmInfoBox()
        {
            InitializeComponent();
        }

        public static NpmInfoBox Create(NpmPackage package)
        {
            _box.lblName.Content = package.Name;
            _box.lblDesc.Content = package.Description;
            _box.lblLatest.Content = package.Version;
            _box.lblAuthor.Content = package.Author;

            return _box;
        }
    }
}
