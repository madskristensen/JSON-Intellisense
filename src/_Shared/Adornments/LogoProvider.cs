using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("json")]
    [ContentType("javascript")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class LogoProvider : IWpfTextViewCreationListener
    {
        private static bool _isVisible, _hasLoaded;
        private const string _propertyName = "ShowWatermark";
        private static readonly Dictionary<string, string> _map = new Dictionary<string, string>()
        {
            { "bower.json", "bower.png"},
            { "package.json", "npm.png"},
            { "gruntfile.js", "grunt.png"},
            { "gulpfile.js", "gulp.png"},
        };

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        [Import]
        public SVsServiceProvider serviceProvider { get; set; }

        private void ManageSettings()
        {
            _hasLoaded = true;

            SettingsManager settingsManager = new ShellSettingsManager(serviceProvider);
            SettingsStore store = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

            LogoAdornment.VisibilityChanged += (sender, isVisible) =>
            {
                WritableSettingsStore wstore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
                _isVisible = isVisible;

                if (!wstore.CollectionExists(Globals.VsixName))
                    wstore.CreateCollection(Globals.VsixName);

                wstore.SetBoolean(Globals.VsixName, _propertyName, isVisible);
            };

            _isVisible = store.GetBoolean(Globals.VsixName, _propertyName, true);
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            if (!_hasLoaded)
                ManageSettings();

            ITextDocument document;
            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
            {
                string fileName = Path.GetFileName(document.FilePath).ToLowerInvariant();

                if (!string.IsNullOrEmpty(fileName) && _map.ContainsKey(fileName))
                {
                    LogoAdornment highlighter = new LogoAdornment(textView, _map[fileName], _isVisible);
                }
            }
        }
    }
}