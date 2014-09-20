using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text;
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
        private static readonly Regex _regex = new Regex("\"?version\"?: (\"|')(.+)(\"|')", RegexOptions.Compiled);
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
                yield return new BowerVersionCompletionEntry(_version, Resources.text.CompletionVersionLatest, context.Session);
                yield return new BowerVersionCompletionEntry("~" + _version, Resources.text.CompletionVersionMinor, context.Session);
                yield return new BowerVersionCompletionEntry("^" + _version, Resources.text.CompletionVersionMajor, context.Session);

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
                    Helper.DTE.StatusBar.Text = Resources.text.CompletionRetrievingVersion;
                    Helper.DTE.StatusBar.Animate(true, _animation);

                    Execute(dependency);
                }
                catch
                {
                    Helper.DTE.StatusBar.Clear();
                }

                Helper.DTE.StatusBar.Animate(false, _animation);
                _isProcessing = false;
            });
        }

        private void Execute(JSONMember dependency)
        {
            string package = dependency.Name.Text.Trim('"');

            ProcessStartInfo start = new ProcessStartInfo("cmd", "/c bower info " + package)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            using (Process p = new Process())
            {
                p.StartInfo = start;
                p.EnableRaisingEvents = true;
                p.OutputDataReceived += OutputDataReceived;
                p.ErrorDataReceived += ErrorDataReceived;
                p.Exited += Exited;

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
        }

        private void Exited(object sender, System.EventArgs e)
        {
            Process p = (Process)sender;

            try
            {
                if (p.ExitCode == 0)
                    Helper.DTE.StatusBar.Clear();
                else
                    Helper.DTE.StatusBar.Text = Resources.text.CompletionRetrievingVersionError;
            }
            catch
            {
                Helper.DTE.StatusBar.Text = Resources.text.CompletionRetrievingVersionError;
            }
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Data))
                return;

            var match = _regex.Match(e.Data);

            if (match.Success)
            {
                _version = match.Groups[2].Value;
                Helper.ExecuteCommand("Edit.ListMembers");
            }
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Data))
                return;

            Logger.Log(e.Data);
        }
    }
}
