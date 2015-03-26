using System;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace JSON_Intellisense
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [InstalledProductRegistration("#110", "#112", Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Package Intellisense", "General", 101, 101, true, new[] { "bower, npm, install, restore" })]
    [Guid(GuidList.guidJSON_IntellisensePkgString)]
    public sealed class JSON_IntellisensePackage : Package
    {
        public const string Version = "1.6";
        
        protected override void Initialize()
        {
            base.Initialize();

            Events2 events = (Events2)Helper.DTE.Events;
            events.SolutionEvents.Opened += SolutionEvents_Opened;

            // Add our command handlers for menu (commands must exist in the .vsct file)
            //OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            //if (null != mcs)
            //{
            //    // Create the command for the menu item.
            //    CommandID menuCommandID = new CommandID(GuidList.guidJSON_IntellisenseCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
            //    MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
            //    mcs.AddCommand(menuItem);
            //}
        }

        void SolutionEvents_Opened()
        {
            Options options = (Options)GetDialogPage(typeof(Options));

            try
            {
                ProjectOpen po = new ProjectOpen();
                po.InstallPackages(options);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }
    }
}
