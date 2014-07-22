using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EnvDTE80;
using Microsoft.CSS.Core;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Newtonsoft.Json.Linq;

namespace JSON_Intellisense
{
    internal class BowerQuickInfo : IQuickInfoSource
    {
        private ITextBuffer _buffer;
        private DTE2 _dte;

        public BowerQuickInfo(ITextBuffer subjectBuffer, DTE2 dte)
        {
            _buffer = subjectBuffer;
            _dte = dte;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (session == null || qiContent == null)
                return;

            // Map the trigger point down to our buffer.
            SnapshotPoint? point = session.GetTriggerPoint(_buffer.CurrentSnapshot);
            if (!point.HasValue)
                return;

            JSONEditorDocument doc = JSONEditorDocument.FromTextBuffer(_buffer);
            JSONParseItem item = doc.JSONDocument.ItemBeforePosition(point.Value.Position);

            if (item == null || !item.IsValid)
                return;

            JSONMember dependency = item.FindType<JSONMember>();
            if (dependency == null)
                return;

            var parent = dependency.Parent.FindType<JSONMember>();
            if (parent == null || !parent.UnquotedNameText.EndsWith("dependencies", StringComparison.OrdinalIgnoreCase))
                return;

            BowerPackage package = GetText(dependency.UnquotedNameText);

            if (package != null)
            {
                applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(item.Start, item.Length, SpanTrackingMode.EdgeNegative);
                qiContent.Add(BowerInfoBox.Create(package));
            }
        }

        private BowerPackage GetText(string packageName)
        {
            string url = "https://bower.herokuapp.com/packages/" + HttpUtility.UrlEncode(packageName);
            string result = Helper.DownloadText(_dte, url);

            if (string.IsNullOrEmpty(result))
                return null;

            try
            {
                JObject obj = JObject.Parse(result);

                return new BowerPackage()
                {
                    Name = packageName,
                    Url = (string)obj["url"],
                    Hits = (int)obj["hits"],
                };
            }
            catch
            { }
            finally
            {
                _dte.StatusBar.Text = string.Empty;
            }

            return null;
        }


        private static UIElement CreateFontPreview(BowerPackage package)
        {
            System.Windows.Controls.Grid host = new Grid();

            // Name
            host.Children.Add(new TextBlock() { Text = "Name", FontWeight = FontWeights.Bold});
            host.Children.Add(new TextBlock() { Text = package.Name + Environment.NewLine, Padding = new Thickness(100, 0, 0, 0) });

            // Url
            host.Children.Add(new TextBlock() { Text = "Url", FontWeight = FontWeights.Bold });
            host.Children.Add(new TextBlock() { Text = package.Url, Margin = new Thickness(100, 0, 0, 0) });

            // Downloads
            host.Children.Add(new TextBlock() { Text = "Downloads", FontWeight = FontWeights.Bold });
            host.Children.Add(new TextBlock() { Text = package.Hits.ToString(), Margin = new Thickness(100, 0, 0, 0) });

            return host;
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }
}
