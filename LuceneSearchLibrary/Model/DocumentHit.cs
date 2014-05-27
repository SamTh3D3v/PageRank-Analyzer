namespace LuceneSearchLibrary.Model
{
    public class DocumentHit
    {
        #region Properties
        public string Title
        {
            get;
            set;
        }
        public string Path
        {
            get;
            set;
        }
        public string Link
        {
            get;
            set;
        }
        //How ManyTimes The Term apears In This Document
        public float Score
        {
            get;
            set;
        }        
        public float PageRank
        {
            get;
            set;
        }
        public float PageRankAmeliorated
        {
            get;
            set;
        }

        #endregion      
    }
}
