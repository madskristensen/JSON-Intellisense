using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor.Intellisense;
using Newtonsoft.Json.Linq;

namespace JSON_Intellisense
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("NpmVersionCompletionProvider")]
    class NpmVersionCompletionProvider : IJSONCompletionListProvider
    {
        [Import]
        private SVsServiceProvider serviceProvider { get; set; }
        private static DTE2 _dte;
        internal static string _version;

        public JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.PropertyValue; }
        }

        public IEnumerable<CompletionEntry> GetListEntries(JSONCompletionContext context)
        {
            if (_dte == null)
                _dte = serviceProvider.GetService(typeof(DTE)) as DTE2;

            if (!Helper.IsSupportedFile(_dte, "package.json"))
                yield break;

            if (_version != null)
            {
                yield return new NpmVersionCompletionEntry(_version, "The currently latest version of the package", context.Session, _dte);
                yield return new NpmVersionCompletionEntry("~" + _version, "Matches the most recent minor version (1.2.x)", context.Session, _dte);
                yield return new NpmVersionCompletionEntry("^" + _version, "Matches the most recent major version (1.x.x)", context.Session, _dte);

                _version = null;
            }
            else
            {
                JSONMember dependency = context.ContextItem.FindType<JSONMember>();
                JSONMember parent = dependency.Parent.FindType<JSONMember>();

                if (parent == null || !parent.Name.Text.Trim('"').EndsWith("dependencies", StringComparison.OrdinalIgnoreCase))
                    yield break;

                ExecuteRemoteSearch(dependency);
            }
        }

        private void ExecuteRemoteSearch(JSONMember dependency)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                string package = dependency.Name.Text.Trim('"');
                string result = SearchVersion(package);
                _version = ParseVersion(result);

                if (_version == null)
                    return;

                Helper.ExecuteCommand(_dte, "Edit.ListMembers");
            });
        }

        private string ParseVersion(string result)
        {
            try
            {
                JObject obj = JObject.Parse(result);
                return (string)obj["version"];
            }
            catch
            { }

            return null;
        }

        private string SearchVersion(string package)
        {
            _dte.StatusBar.Text = "Getting the version for " + package + "...";
            _dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);
            try
            {
                Uri url = new Uri("http://registry.npmjs.org/" + package + "/latest");
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                _dte.StatusBar.Text = ex.Message;
            }
            finally
            {
                _dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSync);
            }

            return null;
        }
    }
}
