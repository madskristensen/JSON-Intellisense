using System.Web;
using System.Windows;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Text;

namespace JSON_Intellisense.Bower
{
    internal class BowerQuickInfo : QuickInfoSourceBase
    {
        public BowerQuickInfo(ITextBuffer subjectBuffer, DTE2 dte)
            : base(subjectBuffer, dte)
        { }

        public override UIElement CreateTooltip(string name, JSONParseItem item)
        {
            BowerPackage package = BowerPackage.FromPackageName(name);

            if (package == null)
                return null;

            return BowerInfoBox.Create(package);
        }
    }
}