using System;
using EnvDTE;

namespace JSON_Intellisense
{
    class Logger
    {
        private static OutputWindowPane pane;
        private static object _syncRoot = new object();

        public static void Log(string message)
        {
            if (EnsurePane())
            {
                pane.OutputString("[" + DateTime.Now.ToLongTimeString() + "] " + message + Environment.NewLine);
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
                        pane = Helper.DTE.ToolWindows.OutputWindow.OutputWindowPanes.Add("Package Intellisense");
                    }
                }
            }

            return pane != null;
        }
    }
}
