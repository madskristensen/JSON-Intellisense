using System.IO;
using EnvDTE80;

namespace JSON_Intellisense
{
    static class Helper
    {
        public static void ExecuteCommand(DTE2 dte, string commandName)
        {
            var command = dte.Commands.Item(commandName);
            if (command.IsAvailable)
            {
                dte.ExecuteCommand(command.Name);
            }
        }

        public static bool IsSupportedFile(DTE2 dte)
        {
            if (dte == null)
                return false;

            var doc = dte.ActiveDocument;

            if (doc == null || string.IsNullOrEmpty(doc.FullName) || Path.GetFileName(doc.FullName) != "package.json")
                return false;

            return true;
        }
    }
}
