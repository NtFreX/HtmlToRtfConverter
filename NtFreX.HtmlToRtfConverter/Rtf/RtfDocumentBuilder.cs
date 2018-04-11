using System.Drawing;
using System.Text;
using NtFreX.HtmlToRtfConverter.Html;

namespace NtFreX.HtmlToRtfConverter.Rtf
{
    internal class RtfDocumentBuilder
    {
        private readonly StringBuilder _document = new StringBuilder();
        private readonly RtfGeneratorSubject _subject;

        public RtfDocumentContext Context { get; } = new RtfDocumentContext();

        public RtfDocumentBuilder(RtfGeneratorSubject subject)
        {
            _subject = subject;
        }

        public RtfDocumentBuilder BackgroundColor(string htmlColor)
            => BackgroundColor(HtmlColorHelper.GetColor(htmlColor));
        public RtfDocumentBuilder BackgroundColor(Color color)
        {
            var colorNumber = Context.GetColorNumber(color);
            _document.Append($@"\chshdng10000\chcbpat{colorNumber}\chcfpat{colorNumber}\cb{colorNumber}");
            return this;
        }
        public RtfDocumentBuilder ForegroundColor(string htmlColor)
            => ForegroundColor(HtmlColorHelper.GetColor(htmlColor));
        public RtfDocumentBuilder ForegroundColor(Color color)
        {
            var colorNumber = Context.GetColorNumber(color);
            _document.Append($@"\cf{colorNumber}");
            return this;
        }
        public RtfDocumentBuilder Paragraph()
        {
            _document.Append(@"\par");
            return this;
        }
        public RtfDocumentBuilder FontSize(float fontSize)
        {
            _document.Append($@"\fs{fontSize}");
            return this;
        }
        public RtfDocumentBuilder HorizontalAligment(HorizontalAligment aligment)
        {
            if (aligment == HtmlToRtfConverter.Rtf.HorizontalAligment.Left)
                _document.Append(@"\ql");
            else if (aligment == HtmlToRtfConverter.Rtf.HorizontalAligment.Right)
                _document.Append(@"\qr");
            else if (aligment == HtmlToRtfConverter.Rtf.HorizontalAligment.Center)
                _document.Append(@"\qc");
            return this;
        }
        public RtfDocumentBuilder FontStyle(string font)
        {
            var fontNumber = Context.GetFontNumber(font);
            _document.Append($@"\f{fontNumber}");
            return this;
        }
        public RtfDocumentBuilder Bold()
        {
            _document.Append(@"\b");
            return this;
        }
        public RtfDocumentBuilder Italic()
        {
            _document.Append(@"\i");
            return this;
        }
        public RtfDocumentBuilder Striketrough()
        {
            _document.Append(@"\strike");
            return this;
        }
        public RtfDocumentBuilder NewLine()
        {
            _document.Append(@"\line");
            return this;
        }
        public RtfDocumentBuilder Text(string text)
        {
            _document.Append("{" + text + "}");
            return this;
        }
        public RtfDocumentBuilder OpenContext()
        {
            _document.Append("{");
            return this;
        }
        public RtfDocumentBuilder CloseContext()
        {
            _document.Append("}");
            return this;
        }
        public RtfDocumentBuilder Rtf(string rtf)
        {
            _document.Append(rtf);
            return this;
        }

        public string Build()
        {
            var fontTable = Context.GetFontTable(_subject.FontStyle);
            var colorTable = Context.GetColorTable();

            var value = new StringBuilder();
            value.AppendLine(@"{\rtf1\ansi\deff0 " + fontTable + colorTable);
            value.AppendLine(_document.ToString());
            value.AppendLine("}");
            return value.ToString();
        }
    }
}