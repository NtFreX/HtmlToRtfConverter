ideas:
 - fluent api to configure style for specific elements
   (new RtfGenerator()
		.Element("em", x => 
			x.Color("gray")
			 .FontSize(12)
			 .Aligment(Aligment.Center)
		))
	(usefull for html editors)
 - 