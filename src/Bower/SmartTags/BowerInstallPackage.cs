using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.Bower
{
    [Export(typeof(IJSONSmartTagProvider))]
    [Name("Bower Install Package")]
    class InstallPackageProvider : JSONSmartTagProviderBase
    {
        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        public override IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer)
        {
            // TODO: Check if the bower package is already installed and if so, don't show this SmartTag

            string directory = Path.GetDirectoryName(buffer.GetFileName());

            if (item.Value != null && item.Value.Text.Trim('"').Length > 0)
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
            get { return "Install package"; }
        }

        public override void Invoke()
        {
            Helper.SaveDocument();
            string param = GenerateSaveParam(_item);
            string command = "bower install " + _item.UnquotedNameText + " " + param;
            Helper.RunProcess(command, _directory, Resources.text.statusbarInstalling);
        }

        private static string GenerateSaveParam(JSONMember item)
        {
            JSONMember parent = item.Parent.FindType<JSONMember>();

            string param = "--save";

            if (parent.UnquotedNameText.Equals("devDependencies", StringComparison.OrdinalIgnoreCase))
                param += "-dev";

            return param;
        }
    }
}
