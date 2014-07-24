using System.Linq;
using System;
using System.Net;
using System.Web;
using Microsoft.JSON.Core.Parser;

namespace JSON_Intellisense.NPM
{
    public class NpmPackage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string License { get; set; }
        public string Homepage { get; set; }

        public static NpmPackage FromPackageName(string name)
        {
            try
            {
                string url = string.Format(Constants.PackageUrl, HttpUtility.UrlEncode(name));
                string result = Helper.DownloadText(url);
                var root = Helper.ParseJSON(result);

                NpmPackage package = new NpmPackage
                {
                    Name = name,
                    Description = root.SelectItemText("description"),
                    Version = root.SelectItemText("version"),
                    Author = root.SelectItemText("author/name"),
                    Homepage = root.SelectItemText("homepage"),
                };

                var licenses = root.SelectItem("licenses") as JSONArray;

                if (licenses != null)
                {
                    var obj = licenses.BlockItemChildren.First().Value as JSONObject;
                    if (obj != null)
                        package.License = obj.SelectItemText("type");
                }
                else
                {
                    package.License = root.SelectItemText("license");
                }

                return package;
            }
            catch
            { /* JSON result is invalid. Ignore */ }

            return null;
        }
    }
}
