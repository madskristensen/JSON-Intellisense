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
                string url = string.Format(Constants.PackageUrl, HttpUtility.UrlEncode(name));
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
