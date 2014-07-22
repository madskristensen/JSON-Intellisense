using System.ComponentModel.Composition;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense
{
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("JSON QuickInfo Source")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType("JSON")]
    internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        private SVsServiceProvider serviceProvider { get; set; }
        private static DTE2 _dte;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            if (_dte == null)
                _dte = serviceProvider.GetService(typeof(DTE)) as DTE2;

            if (Helper.IsSupportedFile(_dte, "bower.json"))
                return new Bower.BowerQuickInfo(textBuffer, _dte);
            else if (Helper.IsSupportedFile(_dte, "package.json"))
                return new NPM.NpmQuickInfo(textBuffer, _dte);

            return null;
        }
    }
}
