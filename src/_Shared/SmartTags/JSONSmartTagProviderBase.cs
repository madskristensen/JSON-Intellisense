using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JSON_Intellisense
{
    abstract class JSONSmartTagProviderBase : IJSONSmartTagProvider
    {
        public Type ItemType
        {
            get { return typeof(JSONMember); }
        }

        public abstract string SupportedFileName { get; }

        public virtual IEnumerable<ISmartTagAction> GetSmartTagActions(JSONParseItem item, int caretPosition, ITrackingSpan itemTrackingSpan, ITextView view)
        {
            string fileName = itemTrackingSpan.TextBuffer.GetFileName();

            if (string.IsNullOrEmpty(fileName) || Path.GetFileName(fileName) != SupportedFileName)
                return Enumerable.Empty<ISmartTagAction>();

            JSONMember member = (JSONMember)item;

            if (!member.Name.ContainsRange(caretPosition, 1))
                return Enumerable.Empty<ISmartTagAction>();

            JSONMember parent = member.Parent.FindType<JSONMember>();

            if (parent == null || !parent.UnquotedNameText.EndsWith("dependencies", StringComparison.OrdinalIgnoreCase))
                return Enumerable.Empty<ISmartTagAction>();

            return GetSmartTagActions(member, itemTrackingSpan.TextBuffer);
        }

        public abstract IEnumerable<ISmartTagAction> GetSmartTagActions(JSONMember item, ITextBuffer buffer);
    }
}
