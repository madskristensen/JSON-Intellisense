using System;
using System.ComponentModel.Composition;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace JSON_Intellisense
{
    public class Logger
    {
        private static OutputWindowPane pane;
        private static object _syncRoot = new object();

        [Import]
        private static SVsServiceProvider serviceProvider { get; set; }

        private static DTE2 _dte { get; set; }

        public static void Log(string message)
        {
            if (EnsurePane())
            {
                pane.OutputString(message + Environment.NewLine);
            }
        }

        private static bool EnsurePane()
        {
            if (pane == null)
            {
                lock (_syncRoot)
                {
                    if (pane == null)
                    {
                        _dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
                        pane = _dte.ToolWindows.OutputWindow.OutputWindowPanes.Add("Package Intellisense");
                    }
                }
            }

            return pane != null;
        }
    }
}
