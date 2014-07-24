using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.NPM
{
    [Export(typeof(IJSONSmartTagProvider))]
    [Name("NPM Navigate To Homepage")]
    [Order(Before = "NPM Update Package")]
    class NavigateToHomepageProvider : JSONSmartTagProviderBase
    {
        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        public override IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer)
        {
            yield return new NavigateToHomePageAction(item.UnquotedNameText);
        }
    }

    internal class NavigateToHomePageAction : JSONSmartTagActionBase
    {
        private string _name;

        public NavigateToHomePageAction(string name)
        {
            _name = name;
            Icon = Constants.Icon;
        }

        public override string DisplayText
        {
            get { return "Open homepage in browser"; }
        }

        public override void Invoke()
        {
            NpmPackage package = NpmPackage.FromPackageName(_name);

            Uri url;

            if (package != null && Uri.TryCreate(package.Homepage, UriKind.Absolute, out url))
            {
                System.Diagnostics.Process.Start(url.ToString());
            }
            else
            {
                MessageBox.Show("The package's doesn't specify a valid homepage", "JSON Intellisense", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }
}
