using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.Web.Editor;
using Microsoft.Web.Editor.Intellisense;
using Newtonsoft.Json.Linq;

namespace JSON_Intellisense
{
    class BowerNameCompletionEntry : JSONCompletionEntry
    {
        private static ImageSource _glyph = BitmapFrame.Create(new Uri("pack://application:,,,/JSON Intellisense;component/Resources/bower.png", UriKind.RelativeOrAbsolute));//GlyphService.GetGlyph(StandardGlyphGroup.GlyphLibrary, StandardGlyphItem.GlyphItemPublic);
        private DTE2 _dte;
        private JSONDocument _doc;

        public BowerNameCompletionEntry(string text, IIntellisenseSession session, DTE2 dte, JSONDocument doc)
            : this(text, null, session)
        {
            _dte = dte;
            _doc = doc;

            base.SortingPriority = 2;
            base.FilterType = CompletionEntryFilterTypes.AlwaysVisible;
        }

        public BowerNameCompletionEntry(string text, StandardGlyphGroup glyph, IIntellisenseSession session)
            : this(text, null, glyph, session)
        { }

        public BowerNameCompletionEntry(string text, string description, IIntellisenseSession session)
            : base(text, "\"" + text + "\"", description, _glyph, null, false, session as ICompletionSession)
        { }

        public BowerNameCompletionEntry(string text, string description, StandardGlyphGroup glyph, IIntellisenseSession session)
            : base(text, "\"" + text + "\"", description, GlyphService.GetGlyph(glyph, StandardGlyphItem.GlyphItemPublic), null, false, session as ICompletionSession)
        { }

        public BowerNameCompletionEntry(string displayText, string insertionText, string description, IIntellisenseSession session)
            : base(displayText, insertionText, description, _glyph, null, false, session as ICompletionSession)
        { }

        internal static IEnumerable<string> _searchResults;

        public override void Commit()
        {
            if (base.DisplayText != "Search Bower...")
            {
                base.Commit();
            }
            else
            {
                string searchTerm = FindSearchTerm();

                if (string.IsNullOrEmpty(searchTerm))
                    return;

                ThreadPool.QueueUserWorkItem(o =>
                {
                    string result = SearchBower(searchTerm);
                    var children = GetChildren(result);

                    if (children.Count() == 0)
                    {
                        base.Session.Dismiss();
                        return;
                    }

                    _dte.StatusBar.Text = string.Empty;
                    _searchResults = children.Take(25).Select(c => (string)c["name"]);

                    Helper.ExecuteCommand(_dte, "Edit.ListMembers");
                });
            }
        }

        private static JEnumerable<JToken> GetChildren(string result)
        {
            try
            {
                JArray array = JArray.Parse(result);
                return array.Children();
            }
            catch
            { }

            return JEnumerable<JToken>.Empty;
        }

        private string FindSearchTerm()
        {
            try
            {
                JSONParseItem member = _doc.ItemBeforePosition(base.Session.TextView.Caret.Position.BufferPosition.Position);
                return member.Text.Trim('"');
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string SearchBower(string searchTerm)
        {
            _dte.StatusBar.Text = "Searching Bower for " + searchTerm + "...";
            _dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);

            try
            {
                Uri url = new Uri("https://bower.herokuapp.com/packages/search/" + HttpUtility.UrlEncode(searchTerm));
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                _dte.StatusBar.Text = "No packages could be found (" + ex.Message + ")";
            }
            finally
            {
                _dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSync);
            }

            return null;
        }
    }
}