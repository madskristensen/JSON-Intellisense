using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EnvDTE80;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.Web.Editor;
using Microsoft.Web.Editor.Intellisense;
using Newtonsoft.Json.Linq;

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

        public NpmVersionCompletionEntry(string text, string description, StandardGlyphGroup glyph, IIntellisenseSession session)
            : base(text, "\"" + text + "\"", description, GlyphService.GetGlyph(glyph, StandardGlyphItem.GlyphItemPublic), null, false, session as ICompletionSession)
        { }

        public NpmVersionCompletionEntry(string displayText, string insertionText, string description, IIntellisenseSession session)
            : base(displayText, insertionText, description, _glyph, null, false, session as ICompletionSession)
        { }

        public override void Commit()
        {
            base.Commit();
            _dte.StatusBar.Text = string.Empty;
        }
    }
}