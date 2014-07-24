using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JSON_Intellisense
{
    class JSONSmartTag : SmartTag
    {
        public JSONSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets)
            : base(SmartTagType.Factoid, actionSets)
        {
        }
    }
}
