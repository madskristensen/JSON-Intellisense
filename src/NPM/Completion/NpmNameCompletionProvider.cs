using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor.Intellisense;

namespace JSON_Intellisense
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("NpmNameCompletionProvider")]
    class NpmNameCompletionProvider : IJSONCompletionListProvider
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

            if (!Helper.IsSupportedFile(_dte, "package.json"))
                yield break;

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
                JSONMember dependency = context.ContextItem.FindType<JSONMember>();

                var parent = dependency.Parent.FindType<JSONMember>();
                if (parent == null || !parent.Name.Text.Trim('"').EndsWith("dependencies", StringComparison.OrdinalIgnoreCase))
                    yield break;

                yield return new NpmNameCompletionEntry("Search NPM...", context.Session, _dte, dependency.JSONDocument);
            }
        }
    }
}
