using System.Web;

namespace JSON_Intellisense.Bower
{
    public class BowerPackage
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Hits { get; set; }

        public static BowerPackage FromPackageName(string name)
        {
            try
            {
                Options options = JSON_IntellisensePackage.Options;
                string packageUrl = Constants.PackageUrl;
    
                if (!string.IsNullOrEmpty(options.BowerCustomFeedUrl)) {
                    packageUrl = options.BowerCustomFeedUrl;
                }
                
                string url = string.Format(packageUrl, HttpUtility.UrlEncode(name));
                string result = Helper.DownloadText(url);

                if (string.IsNullOrEmpty(result))
                    return null;

                var root = Helper.ParseJSON(result);

                return new BowerPackage()
                {
                    Name = name,
                    Url = root.SelectItemText("url"),
                    Hits = int.Parse(root.SelectItemText("hits")),
                };
            }
            catch { /* JSON result is invalid. Ignore */ }

            return null;
        }
    }
}
