using System.Linq;
using Microsoft.CSS.Core;
using Microsoft.JSON.Core.Parser;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JSON_Intellisense
{
    public static class JSONExtensions
    {
        public static string GetMemberName(this JSONDocument doc, ICompletionSession session)
        {
            if (session == null)
                return null;

            JSONParseItem member = doc.ItemBeforePosition(session.TextView.Caret.Position.BufferPosition.Position);

            if (member != null)
                return member.Text.Trim('"');

            return null;
        }

        public static string SelectItemText(this JSONBlockItem block, string selector)
        {
            JSONParseItem item = SelectItem(block, selector);

            if (item != null)
            {
                string text = item.Text.Replace("\\\"", "\"");
                return text.Substring(1, text.Length - 2);
            }
            return string.Empty;
        }

        public static JSONParseItem SelectItem(this JSONBlockItem block, string selector)
        {
            string[] paths = selector.Split('/');
            string path = paths[0];

            var items = from b in block.BlockItemChildren
                        let m = b as JSONMember
                        where m != null && m.UnquotedNameText == path
                        select m.Value;

            JSONParseItem item = items.ElementAtOrDefault(0);

            JSONObject obj = item as JSONObject;

            if (obj != null)
                return SelectItem(obj, string.Join("/", paths.Skip(1)));

            return item;
        }
    }
}
