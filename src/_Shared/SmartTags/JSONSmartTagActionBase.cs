using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JSON_Intellisense
{
    internal abstract class JSONSmartTagActionBase : ISmartTagAction
    {
        public virtual ReadOnlyCollection<SmartTagActionSet> ActionSets
        {
            get { return null; }
        }

        public virtual string DisplayText
        {
            get { return "NO DISPLAY TEXT SPECIFIED"; }
        }

        public ImageSource Icon { get; protected set; }

        public bool IsEnabled
        {
            get { return true; }
        }

        public abstract void Invoke();
    }
}
