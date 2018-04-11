using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NtFreX.HtmlToRtfConverter
{
    //TODO: ¨multilevel lists (ordered, unordered); paragraphs
    public class RtfGenerator
    {
        private readonly RtfGeneratorSubject _subject;

        private const string StyleAttributeName = "style";

        private const string ColorStyleName = "color";
        private const string BackgrounColorStyleName = "background-color";
        
        private readonly HtmlElementType[] _clearingElements =
        {
            HtmlElementType.P,
            HtmlElementType.Div,
            HtmlElementType.Blockquote,
            HtmlElementType.Pre,
            HtmlElementType.Li,
            HtmlElementType.Ol,
            HtmlElementType.Ul
        };
        
        private int _listLevel = -1;
        private bool _isOrderedList;

        private List<Color> _colors;
        private List<string> _fonts;

        internal RtfGenerator(RtfGeneratorSubject subject)
        {
            _subject = subject;
        }

        public string Generate(string html)
        {
            _colors = new List<Color>();
            _fonts = new List<string>();

            var dom = HtmlParser.Parse(html);
            var rtf = GenerateInternal(dom, true);

            var value = new StringBuilder();
            value.AppendLine(@"{\rtf1\ansi\deff0 " + GenerateFontTable() + GenerateColorTable());
            value.AppendLine(rtf);
            value.AppendLine("}");
            return value.ToString();
        }

        private string GenerateInternal(IEnumerable<HtmlDomEntity> dom, bool isCleared)
        {
            var value = new StringBuilder();
            foreach (var obj in dom)
            {
                if (obj is HtmlElement element)
                {
                    var name = new string(element.Name.Select((x, i) => i == 0 ? Char.ToUpper(x) : Char.ToLower(x)).ToArray());
                    if (Enum.TryParse(name, out HtmlElementType elementType))
                    {
                        if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                            _listLevel++;

                        var openingModifiers = string.Join(string.Empty, GetElementOpeningModifiers(elementType, isCleared));
                        var closingModifiers = string.Join(string.Empty, GetElementClosingModifiers(elementType));
                        var attributeModifiers = string.Join(string.Empty, GetAttributeModifiers(element));
                        var configurationModifiers = string.Join(string.Empty, GetConfigurationModifiers(elementType));
                        var innerRtf = GenerateInternal(element.Children, false);

                        value.AppendLine("{" + $"{openingModifiers}{attributeModifiers}" +
                                         "{" + $"{configurationModifiers}{innerRtf}" + "}" + 
                                         closingModifiers + @"}");

                        if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                            _listLevel--;

                        isCleared = true;
                    }
                }
                else if (obj is HtmlText text)
                {
                    var textValue = RemoveSpacing(text.Text);
                    if(!string.IsNullOrWhiteSpace(textValue))
                        value.AppendLine("{" + textValue + "}");
                }
            }

            return value.ToString();
        }

        private string RemoveSpacing(string text)
        {
            text = text
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");

            while(text.Contains("  "))
                text = text.Replace("  ", " ");
            while (text.StartsWith(" "))
                text = text.Substring(1);
            while (text.EndsWith(" "))
                text = text.Substring(0, text.Length - 1);

            return text;
        }

        private IEnumerable<string> GetConfigurationModifiers(HtmlElementType elementType)
        {
            if (_subject.ElementSubjects.ContainsKey(elementType))
            {
                var configuration = _subject.ElementSubjects[elementType];

                if (!configuration.ForegroundColor.IsEmpty)
                    yield return @"\cf" + GetColorNumber(configuration.ForegroundColor);
                if (!configuration.BackgroundColor.IsEmpty)
                {
                    var colorNumber = GetColorNumber(configuration.BackgroundColor);
                    yield return $@"\chshdng10000\chcbpat{colorNumber}\chcfpat{colorNumber}\cb{colorNumber}";
                }
                if (Math.Abs(configuration.FontSize - default(float)) < 0)
                    yield return $"fs{configuration.FontSize}";
                if (configuration.HorizontalAligment == HorizontalAligment.Left)
                    yield return @"\ql";
                if (configuration.HorizontalAligment == HorizontalAligment.Right)
                    yield return @"\qr";
                if (configuration.HorizontalAligment == HorizontalAligment.Center)
                    yield return @"\qc";
                if (!string.IsNullOrEmpty(configuration.FontStyle))
                    yield return @"\f" + GetFontNumber(configuration.FontStyle);
            }
        }
        private IEnumerable<string> GetElementClosingModifiers(HtmlElementType elementType)
        {
            if (_clearingElements.Contains(elementType))
                yield return @"\par";
        }
        private IEnumerable<string> GetElementOpeningModifiers(HtmlElementType elementType, bool isCleared)
        {
            if (!isCleared && _clearingElements.Contains(elementType))
                yield return @"\par";
            if (elementType == HtmlElementType.Strong)
                yield return @"\b";
            if (elementType == HtmlElementType.Em)
                yield return @"\i";
            if (elementType == HtmlElementType.Del)
                yield return @"\strike";
            if (elementType == HtmlElementType.Ol)
                _isOrderedList = true;
            if (elementType == HtmlElementType.Ul)
                _isOrderedList = false;
            if (elementType == HtmlElementType.Li)
            {
                var indent = _listLevel * 400;
                var symbolType = _isOrderedList ? @"\pnlvl3" : @"\pnlvlblt\pntxtb\u8226?\tab";
                yield return @"{\li" + indent + @"\pntext\pn" + symbolType + @"}";
            }
            if (elementType == HtmlElementType.Blockquote)
                yield return $@"\cf{GetColorNumber(Color.LightSlateGray)}\qc";
            if (elementType == HtmlElementType.Pre)
                yield return @"\brdrt\brdrs\brdrb\brdrs\brdrl\brdrs\brdrr\brdrs\brdrw10\brsp20\brdrcf0";
            if (elementType == HtmlElementType.Br)
                yield return @"\line";
        }
        private IEnumerable<string> GetAttributeModifiers(HtmlElement element)
        {
            var style = element.Attributes.FirstOrDefault(x => x.Name.ToLower() == StyleAttributeName);
            if (style != null)
            {
                var styleParts = style.Value.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var stylePart in styleParts)
                {
                    var keyValue = stylePart.Split(":".ToCharArray());
                    var key = keyValue[0].ToLower();
                    var value = keyValue[1];
                    if (key == ColorStyleName)
                        yield return @"\cf" + GetColorNumber(HtmlColorHelper.GetColor(value));
                    if (key == BackgrounColorStyleName)
                    {
                        var colorNumber = GetColorNumber(HtmlColorHelper.GetColor(value));
                        yield return $@"\chshdng10000\chcbpat{colorNumber}\chcfpat{colorNumber}\cb{colorNumber}";
                    }
                }
            }
        }

        private int GetFontNumber(string font)
        {
            var index = _fonts.FindIndex(x => x == font);
            if (index >= 0)
            {
                return index + 1;
            }

            _fonts.Add(font);
            return _fonts.Count;
        }
        private int GetColorNumber(Color color)
        {
            var index = _colors.FindIndex(x => x.A == color.A && x.B == color.B && x.G == color.G && x.R == color.R);
            if (index >= 0)
            {
                return index + 1;
            }

            _colors.Add(color);
            return _colors.Count;
        }

        private string GenerateFontTable()
        {
            var value = new StringBuilder();
            value.Append(@"{\fonttbl;");
            if (!string.IsNullOrWhiteSpace(_subject.FontStyle))
                value.Append(@"{\f0 " + _subject.FontStyle + ";}");
            for (int i = 0; i < _fonts.Count; i++)
                value.Append(@"{\f" + (i + 1) + " " + _fonts[i] + ";}");
            value.Append("}");
            return value.ToString();

        }
        private string GenerateColorTable()
        {
            var value = new StringBuilder();
            value.Append(@"{\colortbl;");
            foreach(var color in _colors)
                value.Append(@"\red" + color.R + @"\green" + color.G + @"\blue" + color.B + @";");
            value.Append("}");
            return value.ToString();
        }
    }
}
