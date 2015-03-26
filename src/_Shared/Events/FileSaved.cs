using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using JSON_Intellisense._Shared.Resources;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JSON_Intellisense
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("JSON")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class FileSaved : IWpfTextViewCreationListener
    {
        ITextDocument _document;

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _document))
            {
                _document.FileActionOccurred += OnSave;
            }

            textView.Closed += OnViewClosed;
        }

        private void OnSave(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType != FileActionTypes.ContentSavedToDisk || Helper.IsSaving)
                return;

            string fileName = Path.GetFileName(e.FilePath);

            if (!fileName.Equals(NPM.Constants.FileName) && !fileName.Equals(Bower.Constants.FileName))
                return;

            Options options = JSON_IntellisensePackage.Options;

            if (!options.NpmInstallOnSave && !options.BowerInstallOnSave)
                return;

            string rootFolder = Path.GetDirectoryName(e.FilePath);

            RestorePackages(fileName, options, rootFolder);
        }

        private static void RestorePackages(string fileName, Options options, string rootFolder)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    Helper.DTE.StatusBar.Animate(true, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);

                    if (fileName.Equals(NPM.Constants.FileName) && options.NpmInstallOnSave)
                        Helper.RunProcessSync("npm install", rootFolder, Resource.RunningNpmRestore, false);
                    else if (fileName.Equals(Bower.Constants.FileName) && options.BowerInstallOnSave)
                        Helper.RunProcessSync("bower install", rootFolder, Resource.RunningNpmRestore, false);

                    Helper.DTE.StatusBar.Text = Resource.PackageRestoreComplete;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                    Helper.DTE.StatusBar.Text = Resource.ErrorRestoringPackages;
                }
                finally
                {
                    Helper.DTE.StatusBar.Animate(false, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
                }
            });
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            IWpfTextView view = (IWpfTextView)sender;

            if (_document != null)
            {
                _document.FileActionOccurred -= OnSave;
                _document = null;
            }

            if (view != null)
            {
                view.Closed -= OnViewClosed;
            }
        }
    }
}
