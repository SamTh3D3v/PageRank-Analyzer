using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Interop;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LuceneSearchClient.Model;
using LuceneSearchLibrary;
using LuceneSearchLibrary.Model;
using PageRankCalculator.Model;
using PageRankCalculator.PageRankCalculation;

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
        public const string ARankingIsCalculatedPropertyName = "ARankingIsCalculated";
        public const string BusyIndicatorPropertyName = "BusyIndicator";
        public const string ListPagesPropertyName = "ListPages";
        #endregion
        #region Fields
        private ObservableCollection<DocumentHit> _listSearchResult;
        private ObservableCollection<DocumentHit> _listPages;
        private String _searchTerms;
        private HtmlIndexer _indexer;
        private WebSite _webSite;
        private Searcher _searcher;
        private bool _searchEnabled = false;
        private bool _amelioratedPrIsSelected;
        private bool _googlePrIsSelected;
        private bool _rankingIsCalculated = false;
        private bool _aRankingIsCalculated = false; 
        private Vector _pageRankVector;
        private Vector _aPageRankVector;
        private bool _busyIndicator = false;
        private WebGraphDataReader _webGraphDataReader;
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
        public bool ARankingIsCalculated
        {
            get
            {
                return _aRankingIsCalculated;
            }

            set
            {
                if (_aRankingIsCalculated == value)
                {
                    return;
                }
                
                _aRankingIsCalculated = value;
                RaisePropertyChanged(ARankingIsCalculatedPropertyName);
            }
        }
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
        public ObservableCollection<DocumentHit> ListPages
        {
            get
            {
                return _listPages;
            }

            set
            {
                if (_listPages == value)
                {
                    return;
                }
                _listPages = value;
                RaisePropertyChanged(ListPagesPropertyName);
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
            Messenger.Default.Register<Vector>(this, "Pr_Is_Calculated", (pr) =>
            {
                RankingIsCalculated = true;
                _pageRankVector = pr;
            });
            Messenger.Default.Register<Vector>(this, "APr_Is_Calculated", (pr) =>
            {
                ARankingIsCalculated = true;
                _aPageRankVector = pr;
            });
            
            Messenger.Default.Register<WebGraphDataReader>(this, "WebGraphDataReader", (wg) =>
            {
                _webGraphDataReader = wg;
            });
        }
        private void Indexing()
        {
            if (WebSite.NewIndex)
            {
                _indexer = new HtmlIndexer(_webSite.WebSiteIndex, _webSite.WebSiteUrl); //c:\index   http://blog.codinghorror.com/
                _indexer.AddDirectory(new DirectoryInfo(_webSite.WebSiteLocation), "*.htm*");   //\blog.codinghorror.com
                _indexer.Close(); 
            }
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
                                              if (RankingIsCalculated)
                                              {
                                                  foreach (var doc in ListSearchResult)
                                                  {                                                      
                                                      var index = _webGraphDataReader.Pages.Where(
                                                          (x) => x.UriString == doc.Link).Select((x) => x.Id);
                                                      if (index.Count()!=0)                                                                                          
                                                          doc.PageRank = _pageRankVector[index.FirstOrDefault()];
                                                  }
                                              }
                                              if (ARankingIsCalculated)
                                              {
                                                  foreach (var doc in ListSearchResult)
                                                  {
                                                      var index = _webGraphDataReader.Pages.Where(
                                                          (x) => x.UriString == doc.Link).Select((x) => x.Id);
                                                      if (index.Count() != 0)
                                                          doc.PageRankAmeliorated = _aPageRankVector[index.FirstOrDefault()];
                                                  }
                                              }
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
        private RelayCommand _onloadAllPagesCommand;
        public RelayCommand OnloadAllPagesCommand
        {
            get
            {
                return _onloadAllPagesCommand
                    ?? (_onloadAllPagesCommand = new RelayCommand(
                                          () =>
                                          {
                                              BusyIndicator = true;
                                              if(_indexer==null) return;
                                              var listAllPages = _indexer.ListIndexedDocs;
                                              //Update The PageRank Values
                                              if (RankingIsCalculated)
                                              {

                                                  foreach (var doc in listAllPages)
                                                  {                                                      
                                                      var index = _webGraphDataReader.Pages.Where(
                                                          (x) => x.UriString == doc.Link).Select((x) => x.Id).FirstOrDefault();                                                                                                       
                                                          doc.PageRank = _pageRankVector[index];
                                                  }
                                              }
                                              if (ARankingIsCalculated)
                                              {

                                                  foreach (var doc in listAllPages)
                                                  {
                                                      var index = _webGraphDataReader.Pages.Where(
                                                          (x) => x.UriString == doc.Link).Select((x) => x.Id).FirstOrDefault();
                                                      doc.PageRankAmeliorated = _aPageRankVector[index];
                                                  }
                                              }                                                                                         
                                                  ListPages = new ObservableCollection<DocumentHit>(listAllPages); 
                                              //Update The PageRank And The AmelioratedPageRank Value If Calculated
                                              BusyIndicator = false;

                                          }));
            }
        }
        #endregion
    }
}