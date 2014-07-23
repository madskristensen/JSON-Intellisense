using System;
using System.IO;
using System.Net;
using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JSON_Intellisense
{
    static class Helper
    {
        public static string GetMemberName(this JSONDocument doc, ICompletionSession session)
        {
            if (session == null)
                return null;

            JSONParseItem member = doc.ItemBeforePosition(session.TextView.Caret.Position.BufferPosition.Position);

            if (member != null)
                return member.Text.Trim('"');

            return null;
        }

        public static void ExecuteCommand(DTE2 dte, string commandName)
        {
            var command = dte.Commands.Item(commandName);
            if (command.IsAvailable)
            {
                dte.ExecuteCommand(command.Name);
            }
        }

        public static bool IsSupportedFile(DTE2 dte, string allowedName)
        {
            if (dte == null)
                return false;

            var doc = dte.ActiveDocument;

            if (doc == null || string.IsNullOrEmpty(doc.FullName) || Path.GetFileName(doc.FullName) != allowedName)
                return false;

            return true;
        }

        public static string DownloadText(DTE2 dte, string url)
        {
            dte.StatusBar.Text = "Searching for packages...";
            dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);

            try
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                dte.StatusBar.Text = "No packages could be found (" + ex.Message + ")";
            }
            finally
            {
                dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSync);
            }

            return null;
        }
    }
}
