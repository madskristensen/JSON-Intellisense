using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace JSON_Intellisense
{
    class Options : DialogPage
    {
        public Options()
        {
            BowerInstallOnOpen = true;
            NpmInstallOnOpen = true;
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
    }
}
