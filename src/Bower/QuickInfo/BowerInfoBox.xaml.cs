using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace JSON_Intellisense.Bower
{
    public partial class BowerInfoBox : UserControl
    {
        private static BowerInfoBox _box = new BowerInfoBox();

        public BowerInfoBox()
        {
            InitializeComponent();
            imgLogo.Source = Constants.Icon;
        }

        public static BowerInfoBox Create(BowerPackage package)
        {
            _box.lblName.Content = package.Name;
            _box.lblUrl.Content = package.Url.Replace("git://", string.Empty).Replace(".git", string.Empty);
            _box.lblHits.Content = package.Hits.ToString("N0");

            return _box;
        }
    }
}
