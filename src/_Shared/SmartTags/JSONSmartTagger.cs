using System;
using System.Collections.Generic;
using Microsoft.CSS.Core;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.Web.Editor;
using Microsoft.Web.Editor.EditorHelpers;

namespace JSON_Intellisense
{
    internal class JSONSmartTagger : ITagger<JSONSmartTag>, IDisposable
    {
        private ITextView _textView;
        private ITextBuffer _textBuffer;
        private JSONDocument _tree;
        private bool _pendingUpdate;
        private ItemHandlerRegistry<IJSONSmartTagProvider> _smartTagProviders;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public JSONSmartTagger(ITextView textView, ITextBuffer textBuffer)
        {
            _textView = textView;
            _textBuffer = textBuffer;
            _pendingUpdate = true;

            _textView.Caret.PositionChanged += OnCaretPositionChanged;
            // [Mads] I've added this so the smart tags refreshes when buffer is manipulated
            //_textBuffer.ChangedLowPriority += BufferChanged;

            _smartTagProviders = new ItemHandlerRegistry<IJSONSmartTagProvider>();
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            EnsureInitialized();

            _pendingUpdate = true;
        }

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs eventArgs)
        {
            EnsureInitialized();

            _pendingUpdate = true;
        }

        /// <summary>
        /// This must be delayed so that the TextViewConnectionListener
        /// has a chance to initialize the WebEditor host.
        /// </summary>
        public bool EnsureInitialized()
        {
            if (_tree == null && WebEditor.Host != null)
            {
                try
                {
                    JSONEditorDocument document = JSONEditorDocument.FromTextBuffer(_textBuffer);
                    _tree = document.JSONDocument;

                    RegisterSmartTagProviders();

                    WebEditor.OnIdle += OnIdle;
                }
                catch (Exception)
                {
                }
            }

            return _tree != null;
        }

        private void RegisterSmartTagProviders()
        {
            IEnumerable<Lazy<IJSONSmartTagProvider>> providers = ComponentLocatorWithOrdering<IJSONSmartTagProvider>.ImportMany();

            foreach (Lazy<IJSONSmartTagProvider> provider in providers)
            {
                _smartTagProviders.RegisterHandler(provider.Value.ItemType, provider.Value);
            }
        }

        private void OnIdle(object sender, EventArgs eventArgs)
        {
            Update();
        }

        private void Update()
        {
            if (_pendingUpdate)
            {
                if (TagsChanged != null)
                {
                    // Tell the editor that the tags in the whole buffer changed. It will call back into GetTags().

                    SnapshotSpan span = new SnapshotSpan(_textBuffer.CurrentSnapshot, new Span(0, _textBuffer.CurrentSnapshot.Length));
                    TagsChanged(this, new SnapshotSpanEventArgs(span));
                }

                _pendingUpdate = false;
            }
        }

        public IEnumerable<ITagSpan<JSONSmartTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            List<ITagSpan<JSONSmartTag>> tags = new List<ITagSpan<JSONSmartTag>>();

            if (!EnsureInitialized())
                return tags;

            // Map view caret to the CSS buffer
            ProjectionSelectionHelper selectionHelpers = new ProjectionSelectionHelper(_textView, new[] { _textBuffer });
            SnapshotPoint? bufferPoint = selectionHelpers.MapFromViewToBuffer(_textView.Caret.Position.BufferPosition);

            if (!bufferPoint.HasValue)
                return tags;

            JSONParseItem currentItem = GetContextItem(_tree, bufferPoint.Value.Position);
            if (currentItem == null || currentItem.Parent == null)
                return tags;

            JSONParseItem parent = currentItem.Parent;

            IEnumerable<IJSONSmartTagProvider> providers = _smartTagProviders.GetHandlers(parent.GetType());
            List<ISmartTagAction> actions = new List<ISmartTagAction>();

            if (providers != null && _textBuffer.CurrentSnapshot.Length >= currentItem.AfterEnd)
            {
                ITrackingSpan trackingSpan = _textBuffer.CurrentSnapshot.CreateTrackingSpan(currentItem.Start, currentItem.Length, SpanTrackingMode.EdgeInclusive);

                foreach (IJSONSmartTagProvider provider in providers)
                {
                    IEnumerable<ISmartTagAction> newActions = provider.GetSmartTagActions(parent, bufferPoint.Value.Position, trackingSpan, _textView);

                    if (newActions != null)
                    {
                        actions.AddRange(newActions);
                    }
                }
            }

            if (actions.Count > 0)
            {
                SmartTagActionSet actionSet = new SmartTagActionSet(actions.AsReadOnly());
                List<SmartTagActionSet> actionSets = new List<SmartTagActionSet>();
                actionSets.Add(actionSet);

                SnapshotSpan itemSpan = new SnapshotSpan(_textBuffer.CurrentSnapshot, currentItem.Start, currentItem.Length);
                JSONSmartTag smartTag = new JSONSmartTag(actionSets.AsReadOnly());

                tags.Add(new TagSpan<JSONSmartTag>(itemSpan, smartTag));
            }

            return tags;
        }

        /// <summary>
        /// This code was copied from CompletionEngine.cs in the CSS code. If this class gets
        /// copied into the CSS code, reuse that other function (CompletionEngine.GetCompletionContextLeafItem)
        /// </summary>
        private static JSONParseItem GetContextItem(JSONDocument styleSheet, int position)
        {
            // Look on both sides of the cursor for a context item.

            JSONParseItem prevItem = styleSheet.ItemBeforePosition(position) ?? styleSheet;
            JSONParseItem nextItem = styleSheet.ItemAfterPosition(position);

            if (position > prevItem.AfterEnd)
            {
                // Not touching the previous item, check its parents

                for (; prevItem != null; prevItem = prevItem.Parent)
                {
                    if (prevItem.IsUnclosed || prevItem.AfterEnd >= position)
                    {
                        break;
                    }
                }
            }

            // Only use the next item if the cursor is touching it, and it's not a comment
            if (nextItem != null && (position < nextItem.Start))// || nextItem.FindType<Comment>() != null))
            {
                nextItem = null;
            }

            // When two things touch the cursor inside of a selector, always prefer the previous item.
            // If this logic gets larger, consider a better design to choose between two items.
            if (nextItem != null &&
                prevItem != null &&
                prevItem.AfterEnd == position)
            {
                nextItem = null;
            }

            return nextItem ?? prevItem;
        }

        public void Dispose()
        {
            _tree = null;
            if (_textBuffer != null)
            {
                _textBuffer.ChangedLowPriority -= BufferChanged;
                _textBuffer = null;
            }

            if (_textView != null)
            {
                _textView.Caret.PositionChanged -= OnCaretPositionChanged;
                _textView = null;
            }
        }
    }
}
