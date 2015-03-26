using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using EnvDTE80;
using Microsoft.CSS.Core;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor;

namespace JSON_Intellisense
{
    static class Helper
    {
        public static DTE2 DTE = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
        public static bool IsSaving;

        public static JSONBlockItem ParseJSON(string document)
        {
            if (string.IsNullOrEmpty(document))
                return null;

            JSONTree tree = new JSONTree();

            try
            {
                tree.TextProvider = new StringTextProvider(document);
                var child = tree.JSONDocument.Children.First();

                var obj = child as JSONBlockItem;
                if (obj != null)
                    return obj;

                var arr = child as JSONArray;
                if (arr != null)
                    return arr;
            }
            catch (Exception)
            { }

            return null;
        }

        public static void ExecuteCommand(string commandName)
        {
            var command = DTE.Commands.Item(commandName);
            if (command.IsAvailable)
            {
                DTE.ExecuteCommand(command.Name);
            }
        }

        public static bool IsSupportedFile(string allowedName)
        {
            var doc = DTE.ActiveDocument;

            if (doc == null || string.IsNullOrEmpty(doc.FullName) || Path.GetFileName(doc.FullName) != allowedName)
                return false;

            return true;
        }

        public static string DownloadText(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch (Exception)
            { /* Can't download. Just ignore */ }

            return null;
        }

        public static void AnimateWindowSize(this UserControl target, double oldHeight, double newHeight)
        {
            target.Height = oldHeight;
            target.BeginAnimation(UserControl.HeightProperty, null);

            Storyboard sb = new Storyboard();

            var aniHeight = new DoubleAnimationUsingKeyFrames();
            aniHeight.Duration = new Duration(new TimeSpan(0, 0, 0, 2));
            aniHeight.KeyFrames.Add(new EasingDoubleKeyFrame(target.Height, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 1))));
            aniHeight.KeyFrames.Add(new EasingDoubleKeyFrame(newHeight, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 1, 200))));

            Storyboard.SetTarget(aniHeight, target);
            Storyboard.SetTargetProperty(aniHeight, new PropertyPath(UserControl.HeightProperty));

            sb.Children.Add(aniHeight);

            sb.Begin();
        }

        public static string GetFileName(this IPropertyOwner owner)
        {
            IVsTextBuffer bufferAdapter;

            if (!owner.Properties.TryGetProperty(typeof(IVsTextBuffer), out bufferAdapter))
                return null;

            var persistFileFormat = bufferAdapter as IPersistFileFormat;
            string ppzsFilename = null;
            uint pnFormatIndex;
            int returnCode = -1;

            if (persistFileFormat != null)
            {
                try
                {
                    returnCode = persistFileFormat.GetCurFile(out ppzsFilename, out pnFormatIndex);
                }
                catch (NotImplementedException)
                {
                    return null;
                }
            }

            if (returnCode != VSConstants.S_OK)
                return null;

            return ppzsFilename;
        }

        public static void RunProcess(string arguments, string directory, string statusbar, Action callback = null)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    RunProcessSync(arguments, directory, statusbar, true);

                    if (callback != null)
                        callback();
                }
                catch { /* Ignore any failure */ }
            });
        }

        public static void SaveDocument()
        {
            IsSaving = true;
            var doc = DTE.ActiveDocument;

            if (doc != null)
                doc.Save();

            IsSaving = false;
        }

        public static void RunProcessSync(string arguments, string directory, string statusbar, bool clearStatus)
        {
            DTE.StatusBar.Text = statusbar;

            ProcessStartInfo start = new ProcessStartInfo("cmd", "/c " + arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            using (Process p = new Process())
            {
                p.StartInfo = start;
                p.EnableRaisingEvents = true;
                p.OutputDataReceived += DataReceived;
                p.ErrorDataReceived += DataReceived;

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
            
            if (clearStatus)
                DTE.StatusBar.Clear();
        }

        static void DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Data))
                return;

            Logger.Log(e.Data);
        }
    }
}
