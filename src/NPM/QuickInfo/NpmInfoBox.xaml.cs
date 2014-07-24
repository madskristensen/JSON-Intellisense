using System.Windows.Controls;

namespace JSON_Intellisense.NPM
{
    public partial class NpmInfoBox : UserControl
    {
        private static NpmInfoBox _box = new NpmInfoBox();

        public NpmInfoBox()
        {
            InitializeComponent();
            imgLogo.Source = Constants.Icon;
        }

        public static NpmInfoBox Create(NpmPackage package)
        {
            _box.lblName.Content = package.Name;
            _box.lblDesc.Content = package.Description;
            _box.lblLatest.Content = package.Version;
            _box.lblAuthor.Content = package.Author ?? "n/a";
            _box.lblLicense.Content = package.License ?? "n/a";
            _box.lblHomepage.Content = package.Homepage ?? "n/a";

            _box.AnimateWindowSize(40, 108);

            return _box;
        }
    }
}
