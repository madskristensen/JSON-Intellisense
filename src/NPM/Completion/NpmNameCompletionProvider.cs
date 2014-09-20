﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

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

        protected override IEnumerable<JSONCompletionEntry> GetEntries(JSONCompletionContext context)
        {
            if (NpmNameCompletionEntry._searchResults != null)
            {
                foreach (string value in NpmNameCompletionEntry._searchResults)
                {
                    yield return new NpmNameCompletionEntry(value, context.Session, null);
                }

                NpmNameCompletionEntry._searchResults = null;
            }
            else
            {
                JSONMember dependency = GetDependency(context);

                if (dependency != null)
                    yield return new NpmNameCompletionEntry(Resources.text.CompletionSearch, context.Session, dependency.JSONDocument);
            }
        }
    }
}
