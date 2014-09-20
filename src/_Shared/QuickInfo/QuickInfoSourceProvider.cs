using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
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
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            if (Helper.IsSupportedFile(Bower.Constants.FileName))
                return new Bower.BowerQuickInfo(textBuffer);
            else if (Helper.IsSupportedFile(NPM.Constants.FileName))
                return new NPM.NpmQuickInfo(textBuffer);

            return null;
        }
    }
}
