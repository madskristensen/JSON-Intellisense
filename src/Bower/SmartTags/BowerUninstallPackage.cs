using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.Bower
{
    [Export(typeof(IJSONSmartTagProvider))]
    [Name("Bower Uninstall Package")]
    class UninstallPackageProvider : JSONSmartTagProviderBase
    {
        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        public override IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer)
        {
            string directory = Path.GetDirectoryName(buffer.GetFileName());

            if (item.Value != null && item.Value.Text.Trim('"').Length > 0)
                yield return new UninstallPackageAction(item, directory);
        }
    }

    internal class UninstallPackageAction : JSONSmartTagActionBase
    {
        private JSONMember _item;
        private string _directory;

        public UninstallPackageAction(JSONMember item, string directory)
        {
            _item = item;
            _directory = directory;
            Icon = Constants.Icon;
        }

        public override string DisplayText
        {
            get { return "Uninstall package"; }
        }

        public override void Invoke()
        {
            string param = InstallPackageAction.GenerateSaveParam(_item);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo("cmd", "/k bower uninstall " + _item.UnquotedNameText + " " + param)
                {
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = _directory
                }
            };

            p.Start();
            p.Dispose();
        }
    }
}
