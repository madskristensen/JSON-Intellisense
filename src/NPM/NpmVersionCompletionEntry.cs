using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EnvDTE80;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JSON_Intellisense
{
    class NpmVersionCompletionEntry : JSONCompletionEntry
    {
        private static ImageSource _glyph = BitmapFrame.Create(new Uri("pack://application:,,,/JSON Intellisense;component/Resources/npm.png", UriKind.RelativeOrAbsolute));//GlyphService.GetGlyph(StandardGlyphGroup.GlyphLibrary, StandardGlyphItem.GlyphItemPublic);
        private DTE2 _dte;

        public NpmVersionCompletionEntry(string text, string description, IIntellisenseSession session, DTE2 dte)
            : base(text, "\"" + text + "\"", description, _glyph, null, false, session as ICompletionSession)
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