using EnvDTE80;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JSON_Intellisense.NPM
{
    class NpmVersionCompletionEntry : JSONCompletionEntry
    {
        private DTE2 _dte;

        public NpmVersionCompletionEntry(string text, string description, IIntellisenseSession session, DTE2 dte)
            : base(text, "\"" + text + "\"", description, Constants.Icon, null, false, session as ICompletionSession)
        {
            _dte = dte;
        }

        public override void Commit()
        {
            base.Commit();
            _dte.StatusBar.Text = string.Empty;
        }
    }
}