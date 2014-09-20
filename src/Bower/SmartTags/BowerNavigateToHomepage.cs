using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.Bower
{
    [Export(typeof(IJSONSmartTagProvider))]
    [Name("Bower Navigate To Homepage")]
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
            Icon = Globals.BrowseIcon;
        }

        public override string DisplayText
        {
            get { return Resources.text.SmartTagNavigateToHomepage; }
        }

        public override void Invoke()
        {
            BowerPackage package = BowerPackage.FromPackageName(_name);
            string cleanUrl = "http://" + package.Url.Replace("git://", string.Empty);
            Uri url;

            if (package != null && Uri.TryCreate(cleanUrl, UriKind.Absolute, out url))
            {
                System.Diagnostics.Process.Start(url.ToString());
            }
            else
            {
                MessageBox.Show(Resources.text.SmartTagNavigateToHomepageError, Globals.VsixName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }
}
