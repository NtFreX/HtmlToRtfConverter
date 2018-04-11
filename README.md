# HtmlToRtfConverter

This is for educational purposes and not fully functional

    var rtfGenerator = new RtfGeneratorBuilder()
        .Element(HtmlElementType.Pre, builder =>
            builder.FontSize(12.0f)
                .ForegroundColor(Color.Blue)
                .BackgroundColor(Color.AliceBlue))
        .Element(HtmlElementType.Blockquote, builder =>
            builder.FontSize(8f)
                .ForegroundColor(Color.Black)
                .BackgroundColor(Color.LightSlateGray)
                .HorizontalAligment(HorizontalAligment.Center))
        .Build();
            
    var value = rtfGenerator.Generate(html);
    
Supported html elements: `div`, `p`, `ul`, `ol`, `li`, `strong`, `em`, `del`, `pre`, `blockquote`, `br`

Supported styles: `color`, `background-color`
