using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebGraphMaker.Model
{
    public class Page
    {   
        [XmlIgnore]
        public Uri Url { get; set; }
        
        
        [XmlAttribute("uri")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string UriString
        {
            get { return Url == null ? null : Url.ToString(); }
            set { Url = (value == null) ? null : new Uri(value); }
        }


        [XmlElement("Id")] 
        public short Id { get; set; }

        [XmlElement("Description")] 
        public String Description { get; set; }
    }
}
