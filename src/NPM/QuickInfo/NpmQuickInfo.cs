using System.Linq;
using System.Web;
using System.Windows;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Text;

namespace JSON_Intellisense.NPM
{
    internal class NpmQuickInfo : QuickInfoSourceBase
    {
        public NpmQuickInfo(ITextBuffer subjectBuffer)
            : base(subjectBuffer)
        { }

        public override UIElement CreateTooltip(string name, JSONParseItem item)
        {
            NpmPackage package = NpmPackage.FromPackageName(name);

            if (package == null)
                return null;

            return NpmInfoBox.Create(package);
        }

        private NpmPackage GetText(string packageName)
        {
            string url = string.Format(Constants.PackageUrl, HttpUtility.UrlEncode(packageName));
            string result = Helper.DownloadText(url);

            if (string.IsNullOrEmpty(result))
                return null;

            try
            {
                var root = Helper.ParseJSON(result);

                NpmPackage package = new NpmPackage
                {
                    Name = packageName,
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
            finally
            {
                Helper.DTE.StatusBar.Text = string.Empty;
            }

            return null;
        }
    }
}