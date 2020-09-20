using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Arctic.Finder.Models
{
    [XmlRoot(ElementName = "toplevel", Namespace = "")]
    public class GoogleResponse : List<CompleteSuggestion>
    {
        //public CompleteSuggestion[] toplevel { get; set; }
    }

    //[Xml(ElementName = "CompleteSuggestion")]
    public class CompleteSuggestion
    {
        //[XmlElement(DataType = "xs:string", ElementName = "suggestion")]
        public string suggestion { get; set; }
    }
}
