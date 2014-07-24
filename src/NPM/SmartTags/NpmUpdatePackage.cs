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
    [Name("NPM Update Package")]
    class UpdatePackageProvider : JSONSmartTagProviderBase
    {
        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        public override IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer)
        {
            string directory = Path.GetDirectoryName(buffer.GetFileName());
            yield return new UpdatePackageAction(item.UnquotedNameText, directory);
        }
    }

    internal class UpdatePackageAction : JSONSmartTagActionBase
    {
        private string _name;
        private string _directory;

        public UpdatePackageAction(string name, string directory)
        {
            _name = name;
            _directory = directory;
            Icon = Constants.Icon;
        }

        public override string DisplayText
        {
            get { return "Update package"; }
        }

        public override void Invoke()
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo("cmd", "/k npm update " + _name)
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
