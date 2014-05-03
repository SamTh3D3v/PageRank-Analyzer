using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneSearchLibrary.Model;

namespace LuceneSearchLibrary
{
    public class Searcher
    {
        #region Consts
        public const int MaximumHits=200;
        #endregion
        #region Fields
        private IndexSearcher _searcher;
        #endregion
        #region Properties

        #endregion
        #region Constructors
        public Searcher(String indexDirectory)
        {
            Directory directory = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(indexDirectory));
            _searcher = new IndexSearcher(IndexReader.Open(directory, true));  //read Only
        }
        #endregion
        #region Methods
        //Changes
        public IEnumerable<DocumentHit> Search(String term)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", analyzer);
            var query = parser.Parse(term);                      
            var collector = TopScoreDocCollector.Create(MaximumHits, true); //true: Sort By The Number Of Terme Occurence 
            _searcher.Search(query, collector);
            ScoreDoc[] hits = collector.TopDocs().ScoreDocs; 
            var listHits=new List<DocumentHit>();
            for (int i = 0; i < hits.Length; i++)
            {
                Document doc = _searcher.Doc(hits[i].Doc);                  
                listHits.Add(new DocumentHit()
                {    
                    Title = doc.Get("title"),
                    Path=doc.Get("path"),
                    Link = doc.Get("link")
                });              
            }
            return listHits;
        }
        #endregion
    }
}
