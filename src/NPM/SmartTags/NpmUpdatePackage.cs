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
            string node_module = Path.Combine(directory, "node_modules", item.UnquotedNameText);

            if (item.Value != null && item.Value.Text.Trim('"').Length > 0 && Directory.Exists(node_module))
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
            Icon = Resources.UpdateIcon;
        }

        public override string DisplayText
        {
            get { return "Update package"; }
        }

        public override void Invoke()
        {
            Helper.RunProcess("npm update " + _name, _directory);
        }
    }
}
