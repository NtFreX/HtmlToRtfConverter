using System.Drawing;

namespace NtFreX.HtmlToRtfConverter
{
    public class ElementBuilder
    {
        private readonly ElementSubject _subject = new ElementSubject();

        public ElementBuilder ForegroundColor(Color color)
        {
            _subject.ForegroundColor = color;
            return this;
        }
        public ElementBuilder BackgroundColor(Color color)
        {
            _subject.BackgroundColor = color;
            return this;
        }
        public ElementBuilder FontSize(float fontSize)
        {
            _subject.FontSize = fontSize;
            return this;
        }
        public ElementBuilder FontStyle(string fontStyle)
        {
            _subject.FontStyle = fontStyle;
            return this;
        }
        public ElementBuilder HorizontalAligment(HorizontalAligment horizontalAligment)
        {
            _subject.HorizontalAligment = horizontalAligment;
            return this;
        }

        internal ElementSubject Build()
            => _subject;
    }
}