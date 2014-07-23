using System;
using System.IO;
using System.Net;
using EnvDTE;
using System.Linq;
using EnvDTE80;
using Microsoft.CSS.Core;
using Microsoft.JSON.Core.Parser;

namespace JSON_Intellisense
{
    static class Helper
    {
        public static JSONBlockItem ParseJSON(string document)
        {
            if (string.IsNullOrEmpty(document))
                return null;

            JSONTree tree = new JSONTree();

            try
            {
                tree.TextProvider = new StringTextProvider(document);
                var child = tree.JSONDocument.Children.First();

                var obj = child as JSONBlockItem;
                if (obj != null)
                    return obj;

                var arr = child as JSONArray;
                if (arr != null)
                    return arr;
            }
            catch (Exception)
            { }

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
