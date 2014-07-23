using System.Web;
using System.Windows;
using EnvDTE80;
using JSON_Intellisense._Shared.QuickInfo;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Text;
using Newtonsoft.Json.Linq;

namespace JSON_Intellisense.Bower
{
    internal class BowerQuickInfo : QuickInfoSourceBase
    {
        public BowerQuickInfo(ITextBuffer subjectBuffer, DTE2 dte)
            :base (subjectBuffer, dte)
        { }

        public override UIElement Process(string name, JSONParseItem item)
        {
            BowerPackage package = GetText(name);

            if (package == null)
                return null;

            return BowerInfoBox.Create(package);
        }

        private BowerPackage GetText(string packageName)
        {
            string url = string.Format(Constants.PackageUrl, HttpUtility.UrlEncode(packageName));
            string result = Helper.DownloadText(_dte, url);

            if (string.IsNullOrEmpty(result))
                return null;

            try
            {
                JObject obj = JObject.Parse(result);

                return new BowerPackage()
                {
                    Name = packageName,
                    Url = (string)obj["url"],
                    Hits = (int)obj["hits"],
                };
            }
            catch
            { }
            finally
            {
                _dte.StatusBar.Text = string.Empty;
            }

            return null;
        }
    }
}