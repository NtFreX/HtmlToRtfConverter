using System.Linq;
using Xunit;

namespace NtFreX.HtmlToRtfConverter.Tests
{
    public class HtmlTokenizerTests
    {
        [Fact]
        public void Can_Tokenize_SingleElement()
        {
            var html = "<div/>";

            var tokenizer = new HtmlTokenizer();
            var result = tokenizer.Tokenize(html)
                                  .ToList();

            Assert.Equal(3, result.Count);

            Assert.Equal(HtmlTokenType.ElementOpen, result[0].TokenType);

            Assert.Equal(HtmlTokenType.Text, result[1].TokenType);
            Assert.Equal("div", result[1].Value);

            Assert.Equal(HtmlTokenType.ElementInlineFinish, result[2].TokenType);
        }
    }
}
