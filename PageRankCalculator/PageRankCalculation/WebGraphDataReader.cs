using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PageRankCalculator.Model;
using WebGraphMaker.businessLogic;
using WebGraphMaker.ExcelDataCovertion;
using WebGraphMaker.Model;

namespace PageRankCalculator.BusinessModel
{
    public class WebGraphDataReader
    {
        #region Properties

        /// <summary>
        /// Gets List of pages generated after reading WebGraph
        /// </summary>
        public List<Page> Pages
        {
            get
            {
                return _pages;
            }
        }

        /// <summary>
        /// Gets list of links generated after reading WebGraph
        /// </summary>
        public List<Link> Links
        {
            get
            {
                return _links;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method that extracts pages and links from WebGraph files 
        /// </summary>
        /// <param name="grapheEntity">Determines whether it is about pages or links</param>
        /// <param name="fileName"> the suggested file to extract data from</param>
        public void ExtractDataFromWebGraph(GraphEntities grapheEntity, string fileName)
        {
            using (Stream reader = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                if (grapheEntity == GraphEntities.Pages)
                {
                    var pagesSerializer = new XmlSerializer(typeof(List<Page>));
                    _pages = (List<Page>)pagesSerializer.Deserialize(reader);
                }
                else if (grapheEntity == GraphEntities.Links)
                {
                    var pagesSerializer = new XmlSerializer(typeof(List<Link>));
                    _links = (List<Link>)pagesSerializer.Deserialize(reader);
                }
            }
        }

        #endregion

        #region Fields

        private List<Page> _pages;
        private List<Link> _links;

        #endregion
    }
}
