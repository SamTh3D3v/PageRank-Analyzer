using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LuceneSearchLibrary;
using LuceneSearchLibrary.Model;


namespace LuceneSearchClient.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //Update
        #region Consts
        public const string ListSearchResultPropertyName = "ListSearchResult";
        public const string SearchTermsPropertyName = "SearchTerms";
        public const string SearchEnabledPropertyName = "SearchEnabled";
        #endregion
        #region Fields
        private ObservableCollection<DocumentHit> _listSearchResult;
        private String _searchTerms;
        private HtmlIndexer _indexer;
        private Searcher _searcher;
        private bool _searchEnabled = false;
        private Thread _indexingThread;
        #endregion
        #region Properties                         
        public ObservableCollection<DocumentHit> ListSearchResult
        {
            get
            {
                return _listSearchResult;
            }

            set
            {
                if (_listSearchResult == value)
                {
                    return;
                }
                
                _listSearchResult = value;
                RaisePropertyChanged(ListSearchResultPropertyName);
            }
        }                        
        public bool SearchEnabled
        {
            get
            {
                return _searchEnabled;
            }

            set
            {
                if (_searchEnabled == value)
                {
                    return;
                }                
                _searchEnabled = value;
                RaisePropertyChanged(SearchEnabledPropertyName);
            }
        }
        public String SearchTerms
        {
            get
            {
                return _searchTerms;
            }

            set
            {
                if (_searchTerms == value)
                {
                    return;
                }                
                _searchTerms = value;
                RaisePropertyChanged(SearchTermsPropertyName);
            }
        }
        #endregion
        #region Ctors and Methods
        public MainViewModel()
        {            
        _indexingThread=new Thread(new ThreadStart(Indexing));
        _indexingThread.Start();

        }

        private void Indexing()
        {
            _indexer = new HtmlIndexer(@"c:\index", "http://blog.codinghorror.com/");
            _indexer.AddDirectory(new DirectoryInfo(@"\blog.codinghorror.com"), "*.htm*");
            _indexer.Close();
            _searcher = new Searcher(@"\index");
            SearchEnabled = true;

        }
        #endregion
        #region Commands
        private RelayCommand _searchCommand;         
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand
                    ?? (_searchCommand = new RelayCommand(
                                          () =>
                                          {                                              
                                             ListSearchResult=new ObservableCollection<DocumentHit>(_searcher.Search(SearchTerms)); 
                                          }));
            }
        }
        #endregion            
    }
}