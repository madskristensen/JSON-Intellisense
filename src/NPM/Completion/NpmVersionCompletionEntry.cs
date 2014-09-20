using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JSON_Intellisense.NPM
{
    class NpmVersionCompletionEntry : JSONCompletionEntry
    {
        public NpmVersionCompletionEntry(string text, string description, IIntellisenseSession session)
            : base(text, "\"" + text + "\"", description, Constants.Icon, null, false, session as ICompletionSession)
        { }

        public override void Commit()
        {
            base.Commit();
            Helper.DTE.StatusBar.Text = string.Empty;
        }
    }
}