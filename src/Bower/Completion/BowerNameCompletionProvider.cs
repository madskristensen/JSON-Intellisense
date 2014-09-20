using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.Bower
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("BowerNameCompletionProvider")]
    class BowerNameCompletionProvider : CompletionProviderBase
    {
        public override JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.PropertyName; }
        }

        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        protected override IEnumerable<JSONCompletionEntry> GetEntries(JSONCompletionContext context)
        {
            if (BowerNameCompletionEntry._searchResults != null)
            {
                foreach (string value in BowerNameCompletionEntry._searchResults)
                {
                    yield return new BowerNameCompletionEntry(value, context.Session, null);
                }

                BowerNameCompletionEntry._searchResults = null;
            }
            else
            {
                JSONMember dependency = GetDependency(context);

                if (dependency != null)
                    yield return new BowerNameCompletionEntry("Search Bower...", context.Session, dependency.JSONDocument);
            }
        }
    }
}
