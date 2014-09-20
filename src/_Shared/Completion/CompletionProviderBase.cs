using System;
using System.Collections.Generic;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;

namespace JSON_Intellisense
{
    public abstract class CompletionProviderBase : IJSONCompletionListProvider
    {
        public abstract JSONCompletionContextType ContextType { get; }

        public abstract string SupportedFileName { get; }

        public IEnumerable<JSONCompletionEntry> GetListEntries(JSONCompletionContext context)
        {
            if (!Helper.IsSupportedFile(SupportedFileName))
                return new List<JSONCompletionEntry>();

            return GetEntries(context);
        }

        protected abstract IEnumerable<JSONCompletionEntry> GetEntries(JSONCompletionContext context);

        protected JSONMember GetDependency(JSONCompletionContext context)
        {
            JSONMember dependency = context.ContextItem.FindType<JSONMember>();
            JSONMember parent = dependency.Parent.FindType<JSONMember>();

            if (parent == null || !parent.UnquotedNameText.EndsWith("dependencies", StringComparison.OrdinalIgnoreCase))
                return null;

            if (dependency.UnquotedNameText.Length == 0)
                return null;

            return dependency;
        }
    }
}
