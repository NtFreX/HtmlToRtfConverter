# HtmlToRtfConverter

This is for educational purposes and not fully functional

    var rtfConverter = new RtfConverterBuilder()
        .Element(HtmlElementType.Pre, builder =>
            builder.FontSize(12.0f)
                .ForegroundColor(Color.Blue)
                .BackgroundColor(Color.AliceBlue))
        .Element(HtmlElementType.Blockquote, builder =>
            builder.FontSize(8f)
                .ForegroundColor(Color.Black)
                .BackgroundColor(Color.LightSlateGray)
                .HorizontalAligment(HorizontalAligment.Center))
        .BuildConverter();
            
    var value = rtfConverter.Convert(html);
    
Supported html elements: `div`, `p`, `ul`, `ol`, `li`, `strong`, `em`, `del`, `pre`, `blockquote`, `br`

Supported styles: `color`, `background-color`
