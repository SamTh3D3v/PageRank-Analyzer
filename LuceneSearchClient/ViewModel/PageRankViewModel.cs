using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Office.Interop.Excel;
using PageRankCalculator.Model;
using PageRankCalculator.PageRankCalculation;
using WebGraphMaker.ExcelDataCovertion;

namespace LuceneSearchClient.ViewModel
{

    public class PageRankViewModel : ViewModelBase
    {
        #region Consts
        public const string PagesXmlFilePropertyName = "PagesXmlFile";
        public const string WebGraphExcelFilePropertyName = "WebGraphExcelFile";
        public const string LinksXmlFilePropertyName = "LinksXmlFile";
        public const string InitialPageRankVectorPropertyName = "InitialPageRankVector";
        public const string TeleportationMatrixPropertyName = "TeleportationMatrix";
        public const string TransitionMatrixPropertyName = "TransitionMatrix";
        public const string PageRankVectorPropertyName = "PageRankVector";
        #endregion
        #region Fields
        private string _linksXmlFile = "";
        private string _pagesXmlFile = "";
        private string _webGraphExcelFile = "";
        private Vector _initialPageRankVector;
        private Matrix _transitionMatrix;
        private Matrix _teleportationMatrix;
        private Range _range;
        private Vector _pageRankVector;
        private WebGraphDataReader _webGraphDataReader;
        #endregion
        #region Properties 
        public string WebGraphExcelFile
        {
            get
            {
                return _webGraphExcelFile;
            }

            set
            {
                if (_webGraphExcelFile == value)
                {
                    return;
                }
                
                _webGraphExcelFile = value;
                RaisePropertyChanged(WebGraphExcelFilePropertyName);
            }
        }   
        public string PagesXmlFile
        {
            get
            {
                return _pagesXmlFile;
            }

            set
            {
                if (_pagesXmlFile == value)
                {
                    return;
                }                
                _pagesXmlFile = value;
                RaisePropertyChanged(PagesXmlFilePropertyName);
            }
        }
        public string LinksXmlFile
        {
            get
            {
                return _linksXmlFile;
            }

            set
            {
                if (_linksXmlFile == value)
                {
                    return;
                }
                _linksXmlFile = value;
                RaisePropertyChanged(LinksXmlFilePropertyName);
            }
        }  
        public Vector InitialPageRankVector
        {
            get
            {
                return _initialPageRankVector;
            }

            set
            {
                if (_initialPageRankVector == value)
                {
                    return;
                }                
                _initialPageRankVector = value;
                RaisePropertyChanged(InitialPageRankVectorPropertyName);
            }
        }                    
        public Matrix TransitionMatrix
        {
            get
            {
                return _transitionMatrix;
            }

            set
            {
                if (_transitionMatrix == value)
                {
                    return;
                }
                
                _transitionMatrix = value;
                RaisePropertyChanged(TransitionMatrixPropertyName);
            }
        }                   
        public Matrix TeleportationMatrix
        {
            get
            {
                return _teleportationMatrix;
            }

            set
            {
                if (_teleportationMatrix == value)
                {
                    return;
                }                
                _teleportationMatrix = value;
                RaisePropertyChanged(TeleportationMatrixPropertyName);
            }
        }
        public Vector PageRankVector
        {
            get
            {
                return _pageRankVector;
            }

            set
            {
                if (_pageRankVector == value)
                {
                    return;
                }                
                _pageRankVector = value;
                RaisePropertyChanged(PageRankVectorPropertyName);
            }
        }
        #endregion
        #region Ctos and Methods
        public PageRankViewModel()
        {

        }
        #endregion
        #region Commands
        private RelayCommand _getTransitionMatrixCommand;
        public RelayCommand GetTransitionMatrixCommand
        {
            get
            {
                return _getTransitionMatrixCommand
                    ?? (_getTransitionMatrixCommand = new RelayCommand(
                                          () =>
                                          {
                                              _webGraphDataReader=new WebGraphDataReader();
                                              _webGraphDataReader.ExtractDataFromWebGraph(GraphEntities.Pages,PagesXmlFile);
                                              _webGraphDataReader.ExtractDataFromWebGraph(GraphEntities.Links, LinksXmlFile);
                                              WebGraphDataConverter.SetTransitionMatrix(_webGraphDataReader.Pages, _webGraphDataReader.Links);
                                             TransitionMatrix=WebGraphDataConverter.TransitionMatrix;                                               
                                              //Setting The Transportation Matrix 
                                              TeleportationMatrix=Matrix.E(TransitionMatrix.Size);                                              
                                              InitialPageRankVector = Vector.e(VectorType.Row, TransitionMatrix.Size);
                                              Messenger.Default.Send<WebGraphDataReader>(_webGraphDataReader,"WebGraphDataReader");
                                              
                                          }));
            }
        }
        private RelayCommand _setTeleportationCommand;
        public RelayCommand SetTeleportationCommand
        {
            get
            {
                return _setTeleportationCommand
                    ?? (_setTeleportationCommand = new RelayCommand(
                                          () =>
                                          {
                                              
                                          }));
            }
        }
        private RelayCommand _setInitialPageRankCommand;      
        public RelayCommand SetInitialPageRankCommand
        {
            get
            {
                return _setInitialPageRankCommand
                    ?? (_setInitialPageRankCommand = new RelayCommand(
                                          () =>
                                          {
                                              InitialPageRankVector = Vector.e(VectorType.Row, TransitionMatrix.Size);
                                          }));
            }
        }
        private RelayCommand _calculatePageRankCommand;
        public RelayCommand CalculatePageRankCommand
        {
            get
            {
                return _calculatePageRankCommand
                    ?? (_calculatePageRankCommand = new RelayCommand(
                                          () =>
                                          {

                                              ulong nbIterations;
                                              var pageRank = new PageRank(TransitionMatrix, PageRank.DefaultDampingFactor);
                                              PageRankVector = pageRank.GetPageRankVector(InitialPageRankVector,
                                                  5, out nbIterations);       
                                              Messenger.Default.Send<Vector>(PageRankVector,"Pr_Is_Calculated"); 
                                              
                                          }));
            }
        }
        private RelayCommand _browseCommand;
        public RelayCommand BrowseCommand
        {
            get
            {
                return _browseCommand
                    ?? (_browseCommand = new RelayCommand(
                                          () =>
                                          {
                                              string excelfile;
                                              _range = ExcelDataReader.ReadData(out _webGraphExcelFile);
                                              RaisePropertyChanged(WebGraphExcelFilePropertyName);
                                              
                                          }));
            }
        }
        private RelayCommand _generateXmLsCommand;
        public RelayCommand GenerateXmlsCommands
        {
            get
            {
                return _generateXmLsCommand
                    ?? (_generateXmLsCommand = new RelayCommand(
                                          () =>
                                          {
                                              var excelDataConverter = new ExcelDataConverter(_range);
                                              excelDataConverter.ConvertExelData(out _pagesXmlFile,out _linksXmlFile);
                                              RaisePropertyChanged(PagesXmlFilePropertyName);
                                              RaisePropertyChanged(LinksXmlFilePropertyName);  
                                              
                                          }));
            }
        }
        #endregion

       
    }
}