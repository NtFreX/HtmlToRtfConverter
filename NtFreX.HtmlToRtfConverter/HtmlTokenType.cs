namespace NtFreX.HtmlToRtfConverter
{
    public enum HtmlTokenType
    {
        ElementOpen,
        ElementClose,
        ElementInlineFinish,
        ElementFinish,
        
        Text,
        NewLine,

        CommentStart,
        CommentEnd
    }
}
