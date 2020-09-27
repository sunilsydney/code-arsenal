using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Arctic.Finder.Models
{
	/// <summary>
	/// WIP, not in use now
	/// </summary>
    [XmlRoot(ElementName = "toplevel", Namespace = "")]
    public class GoogleResponse : List<CompleteSuggestion>
    {        
    }

    public class CompleteSuggestion
    {
        [XmlElement(DataType = "string", ElementName = "suggestion")]
        public string suggestion { get; set; }
    }
}
/* Example Response
<? xml version="1.0"?>
<toplevel>
	<CompleteSuggestion>
		<suggestion data = "carsales" />
	</ CompleteSuggestion >
	< CompleteSuggestion >
		< suggestion data="cars for sale"/>
	</CompleteSuggestion>
	<CompleteSuggestion>
		<suggestion data = "cars" />
	</ CompleteSuggestion >
	< CompleteSuggestion >
		< suggestion data="carsguide"/>
	</CompleteSuggestion>
</toplevel>
*/
