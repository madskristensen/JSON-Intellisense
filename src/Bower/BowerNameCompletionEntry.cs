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
using Newtonsoft.Json.Linq;

namespace JSON_Intellisense
{
    class BowerNameCompletionEntry : JSONCompletionEntry
    {
        private static ImageSource _glyph = BitmapFrame.Create(new Uri("pack://application:,,,/JSON Intellisense;component/Resources/bower.png", UriKind.RelativeOrAbsolute));//GlyphService.GetGlyph(StandardGlyphGroup.GlyphLibrary, StandardGlyphItem.GlyphItemPublic);
        private DTE2 _dte;
        private JSONDocument _doc;
        internal static IEnumerable<string> _searchResults;

        public BowerNameCompletionEntry(string text, IIntellisenseSession session, DTE2 dte, JSONDocument doc)
            : base(text, "\"" + text + "\"", null, _glyph, null, false, session as ICompletionSession)
        {
            _dte = dte;
            _doc = doc;
        }

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

                ExecuteSearch(searchTerm);
            }
        }

        private void ExecuteSearch(string searchTerm)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                string result = SearchBower(searchTerm);
                var children = GetChildren(result);

                if (children.Count() == 0)
                {
                    _dte.StatusBar.Text = "No packages found matching '" + searchTerm + "'";
                    base.Session.Dismiss();
                    return;
                }

                _dte.StatusBar.Text = string.Empty;
                _searchResults = children.Take(25).Select(c => (string)c["name"]);

                Helper.ExecuteCommand(_dte, "Edit.CompleteWord");
            });
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