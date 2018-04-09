namespace NtFreX.HtmlToRtfConverter
{
    public enum HtmlTokenType
    {
        ElementOpen,
        ElementClose,
        ElementInlineFinish,
        ElementFinish,
        
        Text,

        CommentStart,
        CommentEnd
    }
}
