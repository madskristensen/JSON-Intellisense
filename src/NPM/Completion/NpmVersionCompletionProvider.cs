using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.NPM
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("NpmVersionCompletionProvider")]
    class NpmVersionCompletionProvider : CompletionProviderBase
    {
        internal static string _version;

        public override JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.PropertyValue; }
        }

        public override string SupportedFileName
        {
            get { return Constants.FileName; }
        }

        protected override IEnumerable<JSONCompletionEntry> GetEntries(JSONCompletionContext context)
        {
            if (_version != null)
            {
                yield return new NpmVersionCompletionEntry(_version, Resources.text.CompletionVersionLatest, context.Session);
                yield return new NpmVersionCompletionEntry("~" + _version, Resources.text.CompletionVersionMinor, context.Session);
                yield return new NpmVersionCompletionEntry("^" + _version, Resources.text.CompletionVersionMajor, context.Session);

                _version = null;
            }
            else
            {
                JSONMember dependency = GetDependency(context);

                if (dependency != null)
                    ExecuteRemoteSearch(dependency);
            }
        }

        private void ExecuteRemoteSearch(JSONMember dependency)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                string package = dependency.Name.Text.Trim('"');
                string url = string.Format(Constants.PackageUrl, package);
                string result = Helper.DownloadText(url);
                _version = ParseVersion(result);

                if (_version != null)
                    Helper.ExecuteCommand("Edit.ListMembers");
            });
        }

        private string ParseVersion(string result)
        {
            var root = Helper.ParseJSON(result);

            if (root != null)
                return root.SelectItemText("version");

            return null;
        }
    }
}
