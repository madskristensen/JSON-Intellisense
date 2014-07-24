using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("JSON")]
    [TagType(typeof(SmartTag))]
    internal class SmartTagViewTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer textBuffer) where T : ITag
        {
            JSONSmartTagger tagger = new JSONSmartTagger(textView, textBuffer);
            return tagger as ITagger<T>;
        }
    }
}
