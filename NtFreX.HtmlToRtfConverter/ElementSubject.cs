using System.Drawing;

namespace NtFreX.HtmlToRtfConverter
{
    internal class ElementSubject
    {
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
        public float FontSize { get; set; }
        public string FontStyle { get; set; }
        public HorizontalAligment HorizontalAligment { get; set; }
    }
}