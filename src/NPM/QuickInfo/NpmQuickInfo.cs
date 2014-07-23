using System.Web;
using System.Windows;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Text;

namespace JSON_Intellisense.NPM
{
    internal class NpmQuickInfo : QuickInfoSourceBase
    {
        public NpmQuickInfo(ITextBuffer subjectBuffer, DTE2 dte)
            : base(subjectBuffer, dte)
        { }

        public override UIElement CreateTooltip(string name, JSONParseItem item)
        {
            NpmPackage package = GetText(name);

            if (package == null)
                return null;

            return NpmInfoBox.Create(package);
        }

        private NpmPackage GetText(string packageName)
        {
            string url = string.Format(Constants.PackageUrl, HttpUtility.UrlEncode(packageName));
            string result = Helper.DownloadText(_dte, url);

            if (string.IsNullOrEmpty(result))
                return null;

            try
            {
                var root = Helper.ParseJSON(result);

                return new NpmPackage()
                {
                    Name = packageName,
                    Description = root.SelectItemText("description"),
                    Version = root.SelectItemText("version"),
                    Author = root.SelectItemText("author/name"),
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