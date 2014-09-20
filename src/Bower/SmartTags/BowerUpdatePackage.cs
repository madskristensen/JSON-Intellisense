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
    [Name("Bower Update Package")]
    class UpdatePackageProvider : JSONSmartTagProviderBase
    {
        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        public override IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer)
        {
            string directory = Path.GetDirectoryName(buffer.GetFileName());

            if (item.Value != null && item.Value.Text.Trim('"').Length > 0)
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
            Icon = Globals.UpdateIcon;
        }

        public override string DisplayText
        {
            get { return Resources.text.SmartTagUpdatePackage; }
        }

        public override void Invoke()
        {
            Helper.RunProcess("bower update " + _name, _directory);
        }
    }
}
