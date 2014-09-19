using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.JSON.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense.Bower
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("BowerVersionCompletionProvider")]
    class BowerVersionCompletionProvider : CompletionProviderBase
    {
        internal static string _version;
        private static readonly Regex _regex = new Regex("\"version\": \"(.+)\"", RegexOptions.Compiled);
        private static readonly EnvDTE.vsStatusAnimation _animation = EnvDTE.vsStatusAnimation.vsStatusAnimationFind;
        private static bool _isProcessing = false;

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
                yield return new BowerVersionCompletionEntry(_version, "The currently latest version of the package", context.Session, _dte);
                yield return new BowerVersionCompletionEntry("~" + _version, "Matches the most recent minor version (1.2.x)", context.Session, _dte);
                yield return new BowerVersionCompletionEntry("^" + _version, "Matches the most recent major version (1.x.x)", context.Session, _dte);

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
            if (_isProcessing)
                return;

            ThreadPool.QueueUserWorkItem(o =>
            {
                _isProcessing = true;

                try
                {
                    _dte.StatusBar.Text = "Retrieving version number from Bower...";
                    _dte.StatusBar.Animate(true, _animation);

                    Execute(dependency);
                }
                catch
                {
                    _dte.StatusBar.Clear();
                }

                _dte.StatusBar.Animate(false, _animation);
                _isProcessing = false;
            });
        }

        private void Execute(JSONMember dependency)
        {
            string package = dependency.Name.Text.Trim('"');

            ProcessStartInfo start = new ProcessStartInfo("cmd", "/c bower info " + package + " -j")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardOutput = true,
            };

            using (Process p = Process.Start(start))
            {
                p.EnableRaisingEvents = true;
                p.OutputDataReceived += OutputDataReceived;
                p.Exited += Exited;

                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }
        }

        private void Exited(object sender, System.EventArgs e)
        {
            Process p = (Process)sender;

            if (p.ExitCode == 0)
                _dte.StatusBar.Clear();
            else
                _dte.StatusBar.Text = "Could not retrive the version number";
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e == null || e.Data == null)
                return;

            Process p = (Process)sender;

            var match = _regex.Match(e.Data);

            if (match.Success)
            {
                _version = match.Groups[1].Value;
                Helper.ExecuteCommand(_dte, "Edit.ListMembers");
            }
        }
    }
}
