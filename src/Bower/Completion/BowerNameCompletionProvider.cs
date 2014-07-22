using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor.Intellisense;

namespace JSON_Intellisense
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("BowerNameCompletionProvider")]
    class BowerNameCompletionProvider : IJSONCompletionListProvider
    {
        [Import]
        private SVsServiceProvider serviceProvider { get; set; }
        private static DTE2 _dte;

        public JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.PropertyName; }
        }

        public IEnumerable<CompletionEntry> GetListEntries(JSONCompletionContext context)
        {
            if (_dte == null)
                _dte = serviceProvider.GetService(typeof(DTE)) as DTE2;

            if (!Helper.IsSupportedFile(_dte, "bower.json"))
                yield break;

            if (BowerNameCompletionEntry._searchResults != null)
            {
                foreach (string value in BowerNameCompletionEntry._searchResults)
                {
                    yield return new BowerNameCompletionEntry(value, context.Session, _dte, null);
                }

                BowerNameCompletionEntry._searchResults = null;
            }
            else
            {
                JSONMember dependency = context.ContextItem.FindType<JSONMember>();

                var parent = dependency.Parent.FindType<JSONMember>();
                if (parent == null || !parent.Name.Text.Trim('"').EndsWith("dependencies", StringComparison.OrdinalIgnoreCase))
                    yield break;

                yield return new BowerNameCompletionEntry("Search Bower...", context.Session, _dte, dependency.JSONDocument);
            }
        }
    }
}
