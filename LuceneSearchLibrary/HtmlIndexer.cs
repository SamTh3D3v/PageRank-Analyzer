using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using LuceneSearchLibrary.Model;

namespace LuceneSearchLibrary
{
    public class HtmlIndexer
    {
        #region Consts
        public const string ListIndexedDocsPropertyName = "ListIndexedDocs";
        #endregion
        #region Fields  
        private readonly IndexWriter _writer;
        private string _webSiteDirectory;
        private string _pattern;
        private string _webSiteLink;
        private List<DocumentHit> _listIndexedDocs = new List<DocumentHit>();
        #endregion
        #region Properties        
        public List<DocumentHit> ListIndexedDocs
        {
            get
            {
                return _listIndexedDocs;
            }

            set
            {
                if (_listIndexedDocs != null && _listIndexedDocs == value)
                {
                    return;
                }

                _listIndexedDocs = value;                
            }
        }
        #endregion       
        #region Constructors
        public  HtmlIndexer(string indexDirectory,string webSiteLink)
		{

            try
            {
                _writer = new IndexWriter(FSDirectory.Open(indexDirectory), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.LIMITED);
            }
            catch (Exception)
            {
                
                throw;
            }
			_writer.UseCompoundFile = true;
            _webSiteLink = webSiteLink;
            
		}
        #endregion
        #region Methods
        public void AddDirectory(DirectoryInfo webSiteDirectory, string pattern)
        {
            _webSiteDirectory = webSiteDirectory.FullName;
            _pattern = pattern;            
            AddSubDirectory(webSiteDirectory);
        }
        private void AddSubDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo fi in directory.GetFiles(_pattern))
            {
                AddHtmlDocument(fi.FullName);
            }
            foreach (DirectoryInfo di in directory.GetDirectories())
            {
                AddSubDirectory(di);
            }
        }
        public void AddHtmlDocument(string path)
        {
            var doc = new Document();

            string html;
            using (var sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                html = sr.ReadToEnd();
            }
            int relativePathStartingPosition = this._webSiteDirectory.EndsWith("\\") ? this._webSiteDirectory.Length : this._webSiteDirectory.Length + 1;
            string relativePath = path.Substring(relativePathStartingPosition);
            doc.Add(new Field("text", ParseHtml(html), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("path", relativePath, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("link", _webSiteLink + relativePath.Replace("\\","/"), Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("title", GetPageTitle(html), Field.Store.YES, Field.Index.ANALYZED));
            _writer.AddDocument(doc); 
            _listIndexedDocs.Add(new DocumentHit()
            {
                Link = doc.Get("link"),            
               Path = doc.Get("path") ,
               Title = doc.Get("title"), 
               PageRank = 0 ,
               PageRankAmeliorated = 0           
            });
        }
        private static string ParseHtml(string html)
        {
            string temp = Regex.Replace(html, "<[^>]*>", "");
            return temp.Replace("&nbsp;", " ");
        }
        private static string GetPageTitle(string html)
        {
            Match m = Regex.Match(html, "<title>(.*)</title>");
            if (m.Groups.Count == 2)
                return m.Groups[1].Value;
            return "(unknown)";
        }
        public void Close()
        {

            try
            {
                _writer.Optimize();   //Make The Inverted Index Much Faster To Read 
                _writer.Dispose();
            }
            catch (Exception)
            {
                
                throw new Exception("Exception while closing the indexWriter");
            }
            
        }
        #endregion
        #region Events
        #endregion
    }
}
