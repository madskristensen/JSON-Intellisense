using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JSON_Intellisense.NPM
{
    static class Constants
    {
        public const string FileName = "package.json";
        public const string SearchUrl = "https://typeahead.npmjs.com/search?q={0}";
        public const string PackageUrl = "http://registry.npmjs.org/{0}/latest";
        public static ImageSource Icon = BitmapFrame.Create(new Uri("pack://application:,,,/JSON Intellisense;component/NPM/Resources/npm.png", UriKind.RelativeOrAbsolute));
    }
}
