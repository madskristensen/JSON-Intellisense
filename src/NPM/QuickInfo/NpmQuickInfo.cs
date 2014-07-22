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
    internal class NpmQuickInfo : IQuickInfoSource
    {
        private ITextBuffer _buffer;
        private DTE2 _dte;

        public NpmQuickInfo(ITextBuffer subjectBuffer, DTE2 dte)
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

            NpmPackage package = GetText(dependency.UnquotedNameText);

            if (package != null)
            {
                applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(item.Start, item.Length, SpanTrackingMode.EdgeNegative);
                qiContent.Add(NpmInfoBox.Create(package));
            }
        }

        private NpmPackage GetText(string packageName)
        {
            string url = "http://registry.npmjs.org/" + HttpUtility.UrlEncode(packageName) + "/latest";
            string result = Helper.DownloadText(_dte, url);

            if (string.IsNullOrEmpty(result))
                return null;

            try
            {
                JObject obj = JObject.Parse(result);

                return new NpmPackage()
                {
                    Name = packageName,
                    Description = (string)obj["description"],
                    Version = (string)obj["version"],
                    Author = (string)obj["author"]["name"]
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
