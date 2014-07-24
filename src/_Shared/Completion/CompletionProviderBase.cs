using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Shell;

namespace JSON_Intellisense
{
    public abstract class CompletionProviderBase : IJSONCompletionListProvider
    {
        [Import]
        private SVsServiceProvider serviceProvider { get; set; }

        protected static DTE2 _dte {get; private set;}

        public abstract JSONCompletionContextType ContextType { get; }

        public abstract string SupportedFileName { get; }

        public IEnumerable<JSONCompletionEntry> GetListEntries(JSONCompletionContext context)
        {
            if (_dte == null)
                _dte = serviceProvider.GetService(typeof(DTE)) as DTE2;

            if (!Helper.IsSupportedFile(_dte, SupportedFileName))
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

            if (dependency.UnquotedNameText.Length < 2)
                return null;

            return dependency;
        }
    }
}
