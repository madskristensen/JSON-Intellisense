using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JSON_Intellisense.Bower
{
    public static class Constants
    {
        public const string FileName = "bower.json";
        public const string SearchUrl = "https://bower.herokuapp.com/packages/search/{0}";
        public const string PackageUrl = "https://bower.herokuapp.com/packages/{0}";
        public static ImageSource Icon = BitmapFrame.Create(new Uri("pack://application:,,,/JSON Intellisense;component/Bower/Resources/bower.png", UriKind.RelativeOrAbsolute));
    }
}
