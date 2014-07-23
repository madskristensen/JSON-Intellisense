using System.Collections.Generic;
using System.ComponentModel.Composition;
using JSON_Intellisense._Shared.Completion;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor.Intellisense;

namespace JSON_Intellisense.NPM
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("NpmNameCompletionProvider")]
    class NpmNameCompletionProvider : CompletionProviderBase
    {
        public override JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.PropertyName; }
        }

        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        protected override IEnumerable<CompletionEntry> GetEntries(JSONCompletionContext context)
        {
            if (NpmNameCompletionEntry._searchResults != null)
            {
                foreach (string value in NpmNameCompletionEntry._searchResults)
                {
                    yield return new NpmNameCompletionEntry(value, context.Session, _dte, null);
                }

                NpmNameCompletionEntry._searchResults = null;
            }
            else
            {
                JSONMember dependency = GetDependency(context);

                if (dependency != null)
                    yield return new NpmNameCompletionEntry("Search NPM...", context.Session, _dte, dependency.JSONDocument);
            }
        }
    }
}
