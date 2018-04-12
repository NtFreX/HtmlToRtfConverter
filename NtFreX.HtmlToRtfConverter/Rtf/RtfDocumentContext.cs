using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NtFreX.HtmlToRtfConverter.Rtf
{
    internal class RtfDocumentContext
    {
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<string> _fonts = new List<string>();

        //TODO: can we move this two properties to another place?
        public int ListLevel { get; set; }= -1;
        public bool IsOrderedList { get; set; }

        public int GetFontNumber(string font)
        {
            var index = _fonts.FindIndex(x => x == font);
            if (index >= 0)
            {
                return index + 1;
            }

            _fonts.Add(font);
            return _fonts.Count;
        }
        public int GetColorNumber(Color color)
        {
            var index = _colors.FindIndex(x => x.A == color.A && x.B == color.B && x.G == color.G && x.R == color.R);
            if (index >= 0)
            {
                return index + 1;
            }

            _colors.Add(color);
            return _colors.Count;
        }

        public string GetFontTable(string defaultFont = null)
        {
            var value = new StringBuilder();
            value.Append(@"{\fonttbl;");
            if (!string.IsNullOrWhiteSpace(defaultFont))
                value.Append(@"{\f0 " + defaultFont + ";}");
            for (int i = 0; i < _fonts.Count; i++)
                value.Append(@"{\f" + (i + 1) + " " + _fonts[i] + ";}");
            value.Append("}");
            return value.ToString();

        }
        public string GetColorTable()
        {
            var value = new StringBuilder();
            value.Append(@"{\colortbl;");
            foreach (var color in _colors)
                value.Append(@"\red" + color.R + @"\green" + color.G + @"\blue" + color.B + @";");
            value.Append("}");
            return value.ToString();
        }
    }
}