using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace JSON_Intellisense
{
    public class Options : DialogPage
    {
        public Options()
        {
            BowerInstallOnOpen = true;
            NpmInstallOnOpen = true;
            BowerInstallOnSave = true;
            NpmInstallOnSave = true;
            NpmCustomFeedUrl = string.Empty;
        }

        [Category("Project open")]
        [DisplayName("Automatic Bower install")]
        [Description("Calls 'bower install' on project open.")]
        [DefaultValue(true)]
        public bool BowerInstallOnOpen { get; set; }

        [Category("Project open")]
        [DisplayName("Automatic npm install")]
        [Description("Calls 'npm install' on project open.")]
        [DefaultValue(true)]
        public bool NpmInstallOnOpen { get; set; }

        [Category("Restore on save")]
        [DisplayName("bower.json")]
        [Description("Automatically calls 'bower install' when bower.json is saved.")]
        [DefaultValue(true)]
        public bool BowerInstallOnSave { get; set; }

        [Category("Restore on save")]
        [DisplayName("package.json")]
        [Description("Automatically calls 'npm install' when package.json is saved.")]
        [DefaultValue(true)]
        public bool NpmInstallOnSave { get; set; }
        
        [Category("Custom Feeds")]
        [DisplayName("Custom NPM Feed Url")]
        [Description("Enter the URL to your private or custom NPM feed.")]
        [DefaultValue(true)]
        public string NpmCustomFeedUrl { get; set; }
        
        [Category("Custom Feeds")]
        [DisplayName("Custom Bower Feed Url")]
        [Description("Enter the URL to your private or custom Bower feed.")]
        [DefaultValue(true)]
        public string BowerCustomFeedUrl { get; set; }
    }
}
