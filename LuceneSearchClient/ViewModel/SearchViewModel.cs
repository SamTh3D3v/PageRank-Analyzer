using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows.Interop;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LuceneSearchClient.Model;
using LuceneSearchLibrary;
using LuceneSearchLibrary.Model;
using PageRankCalculator.Model;

namespace LuceneSearchClient.ViewModel
{  
    public class SearchViewModel : ViewModelBase
    {
        #region Consts
         public const string ListSearchResultPropertyName = "ListSearchResult";
        public const string SearchTermsPropertyName = "SearchTerms";
        public const string SearchEnabledPropertyName = "SearchEnabled";
        public const string WebSitePropertyName = "WebSite";
        public const string GooglePrIsSelectedPropertyName = "GooglePrIsSelected";
        public const string AmelioratedPrIsSelectedPropertyName = "AmelioratedPrIsSelected";
        public const string RankingIsCalculatedPropertyName = "RankingIsCalculated";
        #endregion
        #region Fields
        private ObservableCollection<DocumentHit> _listSearchResult;
        private String _searchTerms;
        private HtmlIndexer _indexer;
        private WebSite _webSite;
        private Searcher _searcher;
        private bool _searchEnabled =false;
        private bool _amelioratedPrIsSelected;
        private bool _googlePrIsSelected;
        private bool _rankingIsCalculated =false;
        private Vector _pageRankVector;
        #endregion
        #region Properties
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
        public bool GooglePrIsSelected
        {
            get
            {
                return _googlePrIsSelected;
            }

            set
            {
                if (_googlePrIsSelected == value)
                {
                    return;
                }
                
                _googlePrIsSelected = value;
                RaisePropertyChanged(GooglePrIsSelectedPropertyName);
            }
        }
        public bool AmelioratedPrIsSelected
        {
            get
            {
                return _amelioratedPrIsSelected;
            }

            set
            {
                if (_amelioratedPrIsSelected == value)
                {
                    return;
                }
                
                _amelioratedPrIsSelected = value;
                RaisePropertyChanged(AmelioratedPrIsSelectedPropertyName);
            }
        }        
        public bool RankingIsCalculated
        {
            get
            {
                return _rankingIsCalculated;
            }

            set
            {
                if (_rankingIsCalculated == value)
                {
                    return;
                }                
                _rankingIsCalculated = value;
                RaisePropertyChanged(RankingIsCalculatedPropertyName);
            }
        }

        public const string BusyIndicatorPropertyName = "BusyIndicator";

        private bool _busyIndicator = false;

        public bool BusyIndicator
        {
            get
            {
                return _busyIndicator;
            }

            set
            {
                if (_busyIndicator == value)
                {
                    return;
                }
                
                _busyIndicator = value;
                RaisePropertyChanged(BusyIndicatorPropertyName);
            }
        }
        #endregion
        #region Commands

        #endregion
        #region Ctors and Methods
        public SearchViewModel()
        {
            //Registre To Receive The WebSite Settings
            Messenger.Default.Register<WebSite>(this, "savesettings", (website) =>
            {
                WebSite = website;
                BusyIndicator = true;
                var indexingThread = new Thread(new ThreadStart(Indexing));
                indexingThread.Start();
            });
            Messenger.Default.Register<Vector>(this,"Pr_Is_Calculated", (pr) =>
            {               
                    RankingIsCalculated = true;
                    _pageRankVector = pr;
            });
        }
        private void Indexing()
        {
            _indexer = new HtmlIndexer(_webSite.WebSiteIndex, _webSite.WebSiteUrl); //c:\index   http://blog.codinghorror.com/
            _indexer.AddDirectory(new DirectoryInfo(_webSite.WebSiteLocation), "*.htm*");   //\blog.codinghorror.com
            _indexer.Close();
            _searcher = new Searcher(_webSite.WebSiteIndex);
            SearchEnabled = true;
            BusyIndicator = false;

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
                                              ListSearchResult = new ObservableCollection<DocumentHit>(_searcher.Search(SearchTerms));
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