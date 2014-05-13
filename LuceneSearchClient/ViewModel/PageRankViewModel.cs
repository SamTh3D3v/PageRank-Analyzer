using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Office.Interop.Excel;
using PageRankCalculator.Model;
using WebGraphMaker.businessLogic;

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
        #endregion
        #region Fields
        private string _linksXmlFile = "";
        private string _pagesXmlFile = "";
        private string _webGraphExcelFile = "";
        private Vector _initialPageRankVector;
        private Matrix _transitionMatrix;
        private Matrix _teleportationMatrix;
        private Range _range;
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
                                              _range = ExcelDataReader.ReadData();
                                              WebGraphExcelFile = "File Has Been Selected";
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
                                              excelDataConverter.ConvertExelData();
                                              PagesXmlFile = "Pages Xml File Has Been Setted";
                                              LinksXmlFile = "Links Xml File Has Been Setted";


                                          }));
            }
        }
        #endregion

       
    }
}