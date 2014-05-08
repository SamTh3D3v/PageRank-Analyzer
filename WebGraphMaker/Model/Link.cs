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
        public ulong TailPageId { get; set; }

        [XmlElement("To")] 
        public ulong HeadPageId { get; set; }
    }
}
