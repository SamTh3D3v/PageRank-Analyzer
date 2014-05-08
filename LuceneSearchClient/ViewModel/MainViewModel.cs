using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LuceneSearchClient.Model;
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
        public const string WebSitePropertyName = "WebSite";
        #endregion
        #region Fields
        private ObservableCollection<DocumentHit> _listSearchResult;
        private String _searchTerms;
        private HtmlIndexer _indexer;
        private WebSite _webSite;
        private Searcher _searcher;
        private bool _searchEnabled = false;             
        public WebSite WebSite
        {
            get
            {
                return _webSite;
            }

            set
            {
                if (_webSite == value)
                {
                    return;
                }
                
                _webSite = value;
                RaisePropertyChanged(WebSitePropertyName);
            }
        }
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
            //Registre To Receive The WebSite Settings
            Messenger.Default.Register<WebSite>(this, "savesettings", (website) =>
            {
                WebSite = website;
                var indexingThread = new Thread(new ThreadStart(Indexing));
                indexingThread.Start();
            });                      

        }

        private void Indexing()
        {
            _indexer = new HtmlIndexer(_webSite.WebSiteIndex, _webSite.WebSiteUrl); //c:\index   http://blog.codinghorror.com/
            _indexer.AddDirectory(new DirectoryInfo(_webSite.WebSiteLocation), "*.htm*");   //\blog.codinghorror.com
            _indexer.Close();
            _searcher = new Searcher(_webSite.WebSiteIndex);
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
        private RelayCommand _onloadCommand;         
        public RelayCommand OnloadCommand
        {
            get
            {
                return _onloadCommand
                    ?? (_onloadCommand = new RelayCommand(
                                          () => Messenger.Default.Send<NotificationMessage>(new NotificationMessage("opensettingswindow"))));
            }
        }
        #endregion            
    }
}