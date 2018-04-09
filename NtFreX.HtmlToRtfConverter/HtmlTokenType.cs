namespace NtFreX.HtmlToRtfConverter
{
    public enum HtmlTokenType
    {
        ElementOpen,
        ElementClose,
        ElementInlineFinish,
        ElementFinish,

        Attribute,
        AttributeValueSeperator,
        AttributeValueFinish,
        Text,
        Spacing
    }
}
