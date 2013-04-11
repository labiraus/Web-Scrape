using System.Collections.Generic;
using System.Diagnostics;

namespace Web_Scrape
{
    [DebuggerDisplay("{TagType}, {Name}")]
    public class GenericElement
    {
        public string Text { get; set; }

        public string TagType { get; set; }

        public string Name
        {
            get
            {
                if (Attributes.ContainsKey("name") && Attributes["name"] != "")
                    return Attributes["name"];
                if (Attributes.ContainsKey("id") && Attributes["id"] != "")
                    return Attributes["id"];
                return "Unnamed Element";
            }
        }

        public Dictionary<string, string> Attributes { get; set; }

        public GenericElement(string tagType)
        {
            this.TagType = tagType;
            this.Attributes = new Dictionary<string, string>();
        }
    }
}
