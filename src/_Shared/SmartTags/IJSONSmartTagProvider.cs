using System;
using System.Collections.Generic;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JSON_Intellisense
{
    interface IJSONSmartTagProvider
    {
        Type ItemType { get; }
        IEnumerable<ISmartTagAction> GetSmartTagActions(JSONParseItem item, int caretPosition, ITrackingSpan itemTrackingSpan, ITextView view);
    }
}
