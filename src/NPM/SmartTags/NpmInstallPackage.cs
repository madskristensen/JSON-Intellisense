using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.NPM
{
    [Export(typeof(IJSONSmartTagProvider))]
    [Name("Npm Install Package")]
    [Order(After = "NPM Update Package")]
    class InstallPackageProvider : JSONSmartTagProviderBase
    {
        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        public override IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer)
        {
            string directory = Path.GetDirectoryName(buffer.GetFileName());
            string node_module = Path.Combine(directory, "node_modules", item.UnquotedNameText);

            if (item.Value != null && item.Value.Text.Trim('"').Length > 0 && !Directory.Exists(node_module))
                yield return new InstallPackageAction(item, directory);
        }
    }

    internal class InstallPackageAction : JSONSmartTagActionBase
    {
        private JSONMember _item;
        private string _directory;

        public InstallPackageAction(JSONMember item, string directory)
        {
            _item = item;
            _directory = directory;
            Icon = Globals.InstallIcon;
        }

        public override string DisplayText
        {
            get { return Resources.text.SmartTagInstallPackage; }
        }

        public override void Invoke()
        {
            Helper.SaveDocument();
            Helper.RunProcess("npm install " + _item.UnquotedNameText, _directory);
        }
    }
}
