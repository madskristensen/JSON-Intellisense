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
    [Name("Bower Uninstall Package")]
    [Order(After = "Bower Update Package")]
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
                yield return new UninstallPackageAction(item, directory, buffer);
        }
    }

    internal class UninstallPackageAction : JSONSmartTagActionBase
    {
        private JSONMember _item;
        private string _directory;
        private ITextBuffer _buffer;

        public UninstallPackageAction(JSONMember item, string directory, ITextBuffer buffer)
        {
            _item = item;
            _directory = directory;
            _buffer = buffer;
            Icon = Globals.UninstallIcon;
        }

        public override string DisplayText
        {
            get { return Resources.text.SmartTagUninstallPackage; }
        }

        public override void Invoke()
        {
            string command = "bower uninstall " + _item.UnquotedNameText;
            Helper.RunProcess(command, _directory, Resources.text.statusbarUninstalling, Helper.SaveDocument);

            Helper.DTE.UndoContext.Open(Resources.text.SmartTagUninstallPackage);
            _item.DeletePackageMember(_buffer);
            Helper.DTE.UndoContext.Close();
        }
    }
}
