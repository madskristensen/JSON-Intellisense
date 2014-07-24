using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using EnvDTE;
using EnvDTE80;
using Microsoft.CSS.Core;
using Microsoft.JSON.Core.Parser;

namespace JSON_Intellisense
{
    static class Helper
    {
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

        public static void ExecuteCommand(DTE2 dte, string commandName)
        {
            var command = dte.Commands.Item(commandName);
            if (command.IsAvailable)
            {
                dte.ExecuteCommand(command.Name);
            }
        }

        public static bool IsSupportedFile(DTE2 dte, string allowedName)
        {
            if (dte == null)
                return false;

            var doc = dte.ActiveDocument;

            if (doc == null || string.IsNullOrEmpty(doc.FullName) || Path.GetFileName(doc.FullName) != allowedName)
                return false;

            return true;
        }

        public static string DownloadText(DTE2 dte, string url)
        {
            dte.StatusBar.Text = "Searching for packages...";
            dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);

            try
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                dte.StatusBar.Text = "No packages could be found (" + ex.Message + ")";
            }
            finally
            {
                dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSync);
            }

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
    }
}
