using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WebGraphMaker.Model
{
    public class Link
    {
        [XmlElement("From")] 
        public short TailPageId { get; set; }

        [XmlElement("To")] 
        public short HeadPageId { get; set; }
    }
}
