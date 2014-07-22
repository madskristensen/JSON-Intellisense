using System.Windows.Controls;

namespace JSON_Intellisense.Bower
{
    public partial class BowerInfoBox : UserControl
    {
        private static BowerInfoBox _box = new BowerInfoBox();

        public BowerInfoBox()
        {
            InitializeComponent();
        }

        public static BowerInfoBox Create(BowerPackage package)
        {
            _box.lblName.Content = package.Name;
            _box.lblUrl.Content = package.Url;
            _box.lblHits.Content = package.Hits.ToString("N0");

            return _box;
        }
    }
}
