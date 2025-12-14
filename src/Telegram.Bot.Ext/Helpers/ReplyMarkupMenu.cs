using System.Diagnostics;
using Telegram.Bot.Ext.Features.CallbackQueries;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Ext.Helpers;

public class ReplyMarkupMenu : IRootNode
{
    [DebuggerDisplay("{Title} (Id={Id})")]
    private class Node : INode
    {
        private const char SegmentSeparator = '.';

        private readonly CallbackDataId _callbackDataId;
        private readonly Node? _parent;
        private readonly MenuSelectHandler? _handler;

        public Node(string id, string title, CallbackDataId callbackDataId, MenuSelectHandler? handler,
            Node? parent)
        {
            _handler = handler;
            _callbackDataId = callbackDataId;
            _parent = parent;
            Id = id;
            Title = title;
        }

        public string Id { get; }
        public string Title { get; }
        public bool IsLeaf => Children.Count == 0;
        public List<Node> Children { get; } = [];

        public Task ExecuteHandlerAsync(CallbackQuery query, IHandleContext ctx, CancellationToken token)
            => _handler?.Invoke(query, ctx, this, token) ?? Task.CompletedTask;

        public IReadOnlyCollection<INode> GetNodesPath()
        {
            var segments = new Stack<INode>();
            var current = this;
            while (current != null)
            {
                segments.Push(current);
                current = current._parent;
            }
            segments.Pop();
            return segments;
        }

        public InlineKeyboardMarkup BuildInlineMarkup()
        {
            var markup = new InlineMarkup(maxItemsInRow: 1);
            foreach (var child in Children)
                markup.WithItem(child.Title, _callbackDataId.Build(child.Id));
            return markup.BuildMarkup();
        }

        public INode? Find(string id)
        {
            var segments = id.Split(SegmentSeparator);
            if (segments.Length == 0)
                return null;
            if (segments[0] != RootId)
                return null;

            var node = this;
            for (var i=1; i<segments.Length; i++)
            {
                if (!int.TryParse(segments[i], out var ci))
                    return null;
                if (ci < 0 || ci >= node.Children.Count)
                    return null;
                node = node.Children[ci];
            }

            return node;
        }

        // ReSharper disable once ParameterHidesPrimaryConstructorParameter
        public INode AddChild(string title, MenuSelectHandler? handler, Action<INode>? setupChildren = null)
        {
            var childId = $"{Id}{SegmentSeparator}{Children.Count}";
            var childNode = new Node(childId, title, _callbackDataId, handler, this);
            Children.Add(childNode);
            setupChildren?.Invoke(childNode);
            return this;
        }
    }

    private const string RootId = "";
    private readonly Node _root;

    public ReplyMarkupMenu(ICallbackQueryManager callbackQueryManager)
    {
        var menuCallbackDataId = callbackQueryManager.RegisterHandler(HandleMenuSelectAsync);
        _root = new Node(RootId, "Menu", menuCallbackDataId, null, null);
    }

    public INode AddChild(string title, MenuSelectHandler? handler, Action<INode>? setupChildren = null)
        => _root.AddChild(title, handler, setupChildren);

    public InlineKeyboardMarkup BuildInlineMarkup()
        => _root.BuildInlineMarkup();

    public INode? Find(string id)
        => _root.Find(id);

    private async Task<(bool handled, bool removeMarkup)> HandleMenuSelectAsync(CallbackQuery query, string? data,
        IHandleContext ctx, CancellationToken token)
    {
        if (Find(data!) is not Node node)
            return (handled: false, removeMarkup: false);

        await node.ExecuteHandlerAsync(query, ctx, token);
        if (node.Children.Count == 0)
            return (handled: true, removeMarkup: true);

        await ctx.Bot.EditMessageReplyMarkupAsync(ctx.ChatId, query.Message!.MessageId,
            replyMarkup: node.BuildInlineMarkup(), token);
        return (handled: true, removeMarkup: false);
    }
}

public delegate Task MenuSelectHandler(CallbackQuery query, IHandleContext ctx, INode node, CancellationToken token);

public interface IRootNode
{
    INode AddChild(string title, MenuSelectHandler? handler = null, Action<INode>? setupChildren = null);
    InlineKeyboardMarkup BuildInlineMarkup();
    INode? Find(string id);
}

public interface INode : IRootNode
{
    public string Title { get; }
    public bool IsLeaf { get; }
    Task ExecuteHandlerAsync(CallbackQuery query, IHandleContext ctx, CancellationToken token);
    IReadOnlyCollection<INode> GetNodesPath();
}
