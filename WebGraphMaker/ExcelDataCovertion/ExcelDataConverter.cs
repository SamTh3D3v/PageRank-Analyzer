using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Office.Interop.Excel;
using WebGraphMaker.Model;
using System.Diagnostics;

namespace WebGraphMaker.businessLogic
{
    public class ExcelDataConverter
    {
        #region Constructors
        
        /// <summary>
        /// Initilaize new instance of the ExcelDataConverter Class, using a specified range
        /// </summary>
        /// <param name="range">The suggested range from which to convert WebGraph data</param>
        public ExcelDataConverter(Range range)
        {
            _worksheetRange = range;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates The list of pages from the current ExcelDataReader instance's Range object
        /// </summary>
                private void GeneratePagesList()
                {
                     _pages = new List<Model.Page>();

                     int rowCount = _worksheetRange.Rows.Count;
                     int colCount = _worksheetRange.Columns.Count;

                     ulong idCount = 0;
                     for (int i = 1; i <= rowCount; i++)
                     {
                         for (int j = 1; j <= colCount; j++)
                         {
                             //Thx god Uri has the == operator

                             if (_pages.Count == 0)
                             {
                                 Debug.WriteLine(idCount);
                                 _pages.Add(new Model.Page()
                                 {

                                     Url = new Uri(_worksheetRange.Cells[i, j].Value2.ToString(), UriKind.RelativeOrAbsolute),
                                     Id = idCount
                                 });
                                 idCount++;
                             }
                             else
                             {
                                 var uri = new Uri(_worksheetRange.Cells[i, j].Value2.ToString(), UriKind.RelativeOrAbsolute);
                                 if (!_pages.Contains(_pages.Find(p => p.Url == uri)))
                                 {
                                     Debug.WriteLine(idCount);
                                     _pages.Add(new Model.Page()
                                     {
                                         Url = new Uri(_worksheetRange.Cells[i, j].Value2.ToString(), UriKind.RelativeOrAbsolute),
                                         Id = idCount
                                     });
                                     idCount++;
                                 }
                             }
                         }
                     }

                }
        /// <summary>
        /// Generates The list of links from the current ExcelDataReader instance's Range object
        /// </summary>
                private void GenerateLinksList()
                {
                    _links = new List<Model.Link>();

                    int rowCount = _worksheetRange.Rows.Count;
                    
                    for (int i = 1; i <= rowCount; i++)
                    {
                        var tailUri = new Uri(_worksheetRange.Cells[i, 1].Value2.ToString(), UriKind.RelativeOrAbsolute);
                        var headUri = new Uri(_worksheetRange.Cells[i, 2].Value2.ToString(), UriKind.RelativeOrAbsolute);
                        ulong tailId = _pages.Find(p => p.Url == tailUri).Id;
                        ulong headId = _pages.Find(p => p.Url == headUri).Id;
                        _links.Add(new Link()
                        {
                            TailPageId = tailId,
                            HeadPageId = headId
                        });
                    } 
                }

        /// <summary>
        /// Writes Pages or Links lists into XML files, ouptuts the file name of the created XML file
        /// </summary>
        /// <param name="grapheEntity">The suggested entity to persist can be Page or Link</param>
        /// <param name="fileName">The full name of the generated XML file</param>
        private void PersistGraphEntitiesToXml(GraphEntities grapheEntity, out string fileName)
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "txt files (*.xml)|*.xml",
                        FilterIndex = 2,
                        RestoreDirectory = true
                    };

                    
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                fileName = saveFileDialog.FileName;
                using (TextWriter writer = new StreamWriter(fileName))
                        {
                            if (grapheEntity == GraphEntities.Pages)
                            {
                                var pagesSerializer = new XmlSerializer(typeof(List<Model.Page>));
                                pagesSerializer.Serialize(writer, _pages);
                            }
                            else if (grapheEntity == GraphEntities.Links)
                            {
                                var linksSerializer = new XmlSerializer(typeof(List<Link>));
                                linksSerializer.Serialize(writer, _links);      
                            }
                        }
            }
            else
            {
                fileName = null;
                    }
                }

        /// <summary>
        /// Converts Excel data into a WebGraph by generating XML files, outputs the names of the both created XML files 
        /// </summary>
        /// <param name="pagesFileName">The full file name of the created pages XML file</param>
        /// <param name="linksFileName">The full file name of the created links XML file</param>
        public void ConvertExelData(out string pagesFileName, out string linksFileName)
                {
                    GeneratePagesList();
                    GenerateLinksList();

            PersistGraphEntitiesToXml(GraphEntities.Pages, out pagesFileName);
            PersistGraphEntitiesToXml(GraphEntities.Links, out linksFileName);
                }
        #endregion

        #region Fields

            private List<Model.Page> _pages;
            private List<Model.Link> _links;

        private readonly Range _worksheetRange;

        #endregion
    }

    public enum GraphEntities
    {
        Pages, Links
    }
}
