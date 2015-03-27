using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using JSON_Intellisense._Shared.Resources;

namespace JSON_Intellisense
{
    class ProjectOpen
    {
        public void InstallPackages(Options options)
        {
            if (!options.BowerInstallOnOpen && !options.NpmInstallOnOpen)
                return;

            var projects = GetProjects();
            List<string> _dupeCache = new List<string>();

            foreach (Project project in projects)
            {
                string projectFolder = GetRootFolder(project);

                if (string.IsNullOrEmpty(projectFolder) || !Directory.Exists(projectFolder))
                    continue;

                string package = GetFileName(Path.Combine(projectFolder, NPM.Constants.FileName));
                string bower = GetFileName(Path.Combine(projectFolder, Bower.Constants.FileName));
                bool npmExist = File.Exists(package);
                bool bowerExist = File.Exists(bower);

                if (!npmExist && !bowerExist)
                    continue;

                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        Helper.DTE.StatusBar.Animate(true, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);

                        if (options.BowerInstallOnOpen && bowerExist && !_dupeCache.Contains(bower))
                        {
                            _dupeCache.Add(bower);
                            Helper.RunProcessSync("bower install", Path.GetDirectoryName(bower), Resource.RunningBowerRestore, false);
                        }

                        if (options.NpmInstallOnOpen && npmExist && !_dupeCache.Contains(package))
                        {
                            _dupeCache.Add(package);
                            Helper.RunProcessSync("npm install", Path.GetDirectoryName(package), Resource.RunningNpmRestore, false);
                        }

                        Helper.DTE.StatusBar.Text = Resource.PackageRestoreComplete;
                    }
                    catch (Exception)
                    {
                        Helper.DTE.StatusBar.Text = Resource.ErrorRestoringPackages;
                    }
                    finally
                    {
                        Helper.DTE.StatusBar.Animate(false, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
                    }
                });
            }
        }

        private static string GetFileName(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    return fileName;

                DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(fileName));
                if (directory.Parent == null)
                    return fileName;

                string parent = Path.Combine(directory.Parent.FullName, Path.GetFileName(fileName));
                return GetFileName(parent);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return fileName;
            }
        }

        private IEnumerable<Project> GetProjects()
        {
            return Helper.DTE.Solution.Projects
                  .Cast<Project>()
                  .SelectMany(GetChildProjects)
                  .Union(Helper.DTE.Solution.Projects.Cast<Project>())
                  .Where(p => { try { return !string.IsNullOrEmpty(p.FullName); } catch { return false; } });
        }

        private static IEnumerable<Project> GetChildProjects(Project parent)
        {
            try
            {
                if (parent.Kind != "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}" && parent.Collection == null)  // Unloaded
                    return Enumerable.Empty<Project>();

                if (!string.IsNullOrEmpty(parent.FullName))
                    return new[] { parent };
            }
            catch (COMException)
            {
                return Enumerable.Empty<Project>();
            }

            return parent.ProjectItems
                    .Cast<ProjectItem>()
                    .Where(p => p.SubProject != null)
                    .SelectMany(p => GetChildProjects(p.SubProject));
        }

        public static string GetRootFolder(Project project)
        {
            try
            {
                if (string.IsNullOrEmpty(project.FullName))
                    return null;

                string fullPath;

                try
                {
                    fullPath = project.Properties.Item("FullPath").Value as string;
                }
                catch (ArgumentException)
                {
                    try
                    {
                        // MFC projects don't have FullPath, and there seems to be no way to query existence
                        fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                    }
                    catch (ArgumentException)
                    {
                        // Installer projects have a ProjectPath.
                        fullPath = project.Properties.Item("ProjectPath").Value as string;
                    }
                }

                if (String.IsNullOrEmpty(fullPath))
                    return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : "";

                if (Directory.Exists(fullPath))
                    return fullPath;

                if (File.Exists(fullPath))
                    return Path.GetDirectoryName(fullPath);

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
