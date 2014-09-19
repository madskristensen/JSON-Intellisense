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
    [Name("Npm Uninstall Package")]
    [Order(After = "NPM Update Package")]
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
            Icon = Resources.UninstallIcon;
        }

        public override string DisplayText
        {
            get { return "Uninstall package"; }
        }

        public override void Invoke()
        {
            Helper.RunProcess("npm uninstall " + _item.UnquotedNameText, _directory);
            RemoveLine();
        }

        private void RemoveLine()
        {
            var line = _buffer.CurrentSnapshot.GetLineFromPosition(_item.Start);

            Span span = new Span(line.Start.Position - 1, line.Length + 1);
            _buffer.Delete(span);
        }
    }
}
