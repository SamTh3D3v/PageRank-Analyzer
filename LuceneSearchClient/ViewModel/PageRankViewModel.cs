using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Office.Interop.Excel;
using PageRankCalculator.Model;
using PageRankCalculator.PageRankCalculation;
using WebGraphMaker.ExcelDataCovertion;
using Xceed.Wpf.Toolkit;

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
        public const string AmelioratedPageRankVectorPropertyName = "AmelioratedPageRankVector";
        public const string GenerateButtonIsEnabledPropertyName = "GenerateButtonIsEnabled";
        public const string GetTrMatButIsEnabledPropertyName = "GetTrMatButIsEnabled";
        public const string BusyIndicatorPropertyName = "BusyIndicator";
        public const string AprPrecisionPropertyName = "AprPrecision";
        public const string PrPrecisionPropertyName = "PrPrecision";
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
        private Vector _amelioratedPageRankVector;
        private WebGraphDataReader _webGraphDataReader;
        private BackgroundWorker _worker;
        private ExcelDataConverter _excelDataConverter;
        private bool _generateButtonIsEnabled = false;
        private bool _getTrMatButIsEnabled = false;
        private bool _busyIndicator = false;
        private short _aprPrecision = 5;
        private short _prPrecision = 5;     
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
        public Vector AmelioratedPageRankVector
        {
            get
            {
                return _amelioratedPageRankVector;
            }

            set
            {
                if (_amelioratedPageRankVector == value)
                {
                    return;
                }

                _amelioratedPageRankVector = value;
                RaisePropertyChanged(AmelioratedPageRankVectorPropertyName);
            }
        }
        public bool GenerateButtonIsEnabled
        {
            get
            {
                return _generateButtonIsEnabled;
            }

            set
            {
                if (_generateButtonIsEnabled == value)
                {
                    return;
                }

                _generateButtonIsEnabled = value;
                RaisePropertyChanged(GenerateButtonIsEnabledPropertyName);
            }
        }
        public bool GetTrMatButIsEnabled
        {
            get
            {
                return _getTrMatButIsEnabled;
            }

            set
            {
                if (_getTrMatButIsEnabled == value)
                {
                    return;
                }

                _getTrMatButIsEnabled = value;
                RaisePropertyChanged(GetTrMatButIsEnabledPropertyName);
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
        public short AprPrecision
        {
            get
            {
                return _aprPrecision;
            }

            set
            {
                if (_aprPrecision == value)
                {
                    return;
                }


                _aprPrecision = value;
                RaisePropertyChanged(AprPrecisionPropertyName);
            }
        }                      
        public short PrPrecision
        {
            get
            {
                return _prPrecision;
            }

            set
            {
                if (_prPrecision == value)
                {
                    return;
                }

                _prPrecision = value;
                RaisePropertyChanged(PrPrecisionPropertyName);
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
                                              try
                                              {
                                                  BusyIndicator = true;
                                                  _webGraphDataReader = new WebGraphDataReader();
                                                  var indexingThread = new Thread(new ThreadStart(GetTransitionMat));
                                                  indexingThread.Start();
                                              }
                                              catch (TargetInvocationException tiEx)
                                              {
                                                  Debug.WriteLine(tiEx.InnerException);
                                              }

                                          }));
            }
        }
        private void GetTransitionMat()
        {
            try
            {
                _webGraphDataReader.ExtractDataFromWebGraph(GraphEntities.Pages, PagesXmlFile);
                _webGraphDataReader.ExtractDataFromWebGraph(GraphEntities.Links, LinksXmlFile);
                WebGraphDataConverter.SetTransitionMatrix(_webGraphDataReader.Pages, _webGraphDataReader.Links);
                Debug.WriteLine("Transition Matrix Generated");
                TransitionMatrix = WebGraphDataConverter.TransitionMatrix;
                //Update The Busy Indicator  
                //ind.IsBusy = false;
                //Setting The Transportation Matrix 
                TeleportationMatrix = Matrix.E(TransitionMatrix.Size);
                InitialPageRankVector = Vector.e(VectorType.Row, TransitionMatrix.Size);
                Messenger.Default.Send<WebGraphDataReader>(_webGraphDataReader, "WebGraphDataReader");
                BusyIndicator = false;
            }
            catch (TargetInvocationException tiEx)
            {
                System.Windows.MessageBox.Show(tiEx.InnerException.ToString());

            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message);
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
                                              BusyIndicator = true;
                                              var worker = new BackgroundWorker();
                                              worker.DoWork += CalculatePageRank;
                                              worker.RunWorkerCompleted += CalculatePRCompeleted;
                                              worker.RunWorkerAsync();
                                          }));
            }
        }
        private void CalculatePRCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {

            PageRankVector = (Vector)e.Result;
            Messenger.Default.Send<Vector>(PageRankVector, "Pr_Is_Calculated");
            //Update The Busy Indicator  
            BusyIndicator = false;
        }

        private void CalculatePageRank(object sender, DoWorkEventArgs e)
        {
            ulong nbIterations;
            var pageRank = new PageRank(TransitionMatrix, PageRank.DefaultDampingFactor);
            e.Result = pageRank.GetPageRankVector(InitialPageRankVector,
               PrPrecision, out nbIterations);
        }

        private RelayCommand _calculateAmelioratedPageRankCommand;
        public RelayCommand CalculateAmelioratedPageRankCommand
        {
            get
            {
                return _calculateAmelioratedPageRankCommand
                    ?? (_calculateAmelioratedPageRankCommand = new RelayCommand(
                                          () =>
                                          {

                                              BusyIndicator = true;
                                              var worker = new BackgroundWorker();
                                              worker.DoWork += CalculateAmeliPageRank;
                                              worker.RunWorkerCompleted += CalculateAPRCompeleted;
                                              worker.RunWorkerAsync();

                                          }));
            }
        }
        private void CalculateAPRCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {

            AmelioratedPageRankVector = (Vector)e.Result;
            Messenger.Default.Send<Vector>(AmelioratedPageRankVector, "APr_Is_Calculated");
            //Update The Busy Indicator  
            BusyIndicator = false;
        }

        private void CalculateAmeliPageRank(object sender, DoWorkEventArgs e)
        {
            ulong nbIterations;
            var pageRank = new PageRank(TransitionMatrix, PageRank.DefaultDampingFactor);
            e.Result = pageRank.GetAmelioratedPageRankVector(InitialPageRankVector,
               AprPrecision, out nbIterations);
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
                                              var openFileDialog = new OpenFileDialog
                                                        {
                                                            Filter = "Excel Files (.xlsx)|*.xlsx",
                                                            FilterIndex = 1,
                                                            Multiselect = false
                                                        };
                                              var dialogResult = openFileDialog.ShowDialog();
                                              if (dialogResult != DialogResult.OK) return;
                                              GenerateButtonIsEnabled = false;
                                              WebGraphExcelFile = openFileDialog.FileName;
                                              //This Operation Need To Be Calculated In a Worker

                                              Debug.WriteLine("Generate Rang From Excel Started");
                                              var time = DateTime.Now;
                                              //Start The Busy Indicator 
                                              //ind.IsBusy = true;
                                              try
                                              {
                                                  BusyIndicator = true;
                                                  var worker = new BackgroundWorker();
                                                  worker.DoWork += GenerateRang;
                                                  worker.RunWorkerCompleted += GenerateRangCompleted;
                                                  worker.RunWorkerAsync();
                                              }
                                              catch (TargetInvocationException tiEx)
                                              {
                                                  Debug.WriteLine(tiEx.InnerException);
                                              }
                                              Debug.WriteLine("Genertate Excel Frome Excel Terminated, Elapsed Time Is : " + (DateTime.Now - time) + " Miliseconds");
                                          }));
            }
        }
        private void GenerateRangCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.WriteLine("Excel Generation Compeleted");
            GenerateButtonIsEnabled = true;
            //Update The Busy Indicator  
            BusyIndicator = false;
        }
        private void GenerateRang(object sender, DoWorkEventArgs e)
        {
            _range = ExcelDataReader.ReadData(_webGraphExcelFile);
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
                                              _excelDataConverter = new ExcelDataConverter(_range);
                                              var saveFileDialog = new SaveFileDialog
                                                     {
                                                         Filter = "txt files (*.xml)|*.xml",
                                                         FilterIndex = 2,
                                                         RestoreDirectory = true
                                                     };
                                              if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                                              GetTrMatButIsEnabled = false;
                                              PagesXmlFile = saveFileDialog.FileName;
                                              if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                                              LinksXmlFile = saveFileDialog.FileName;

                                              Debug.WriteLine("Operation Started");
                                              var time = DateTime.Now;
                                              //Start THe Busy Indicator 
                                              //ind.IsBusy = true;
                                              try
                                              {
                                                  BusyIndicator = true;
                                                  var worker = new BackgroundWorker();
                                                  worker.DoWork += GenerateXmlFiles;
                                                  worker.RunWorkerCompleted += GenerateXmlFilesCompeleted;
                                                  worker.RunWorkerAsync();
                                              }
                                              catch (TargetInvocationException tiEx)
                                              {
                                                  Debug.WriteLine(tiEx.InnerException);
                                              }
                                              Debug.WriteLine("Operation Terminated Elapsed Time Is : " + (DateTime.Now - time) + " Miliseconds"); //About 500 Ms For a WebGraph Of 300 Nodes                                               


                                          }));
            }
        }
        private void GenerateXmlFilesCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.WriteLine("Xmls Generation Compeleted");
            GetTrMatButIsEnabled = true;
            //Update The Busy Indicator  
            BusyIndicator = false;
        }

        private void GenerateXmlFiles(object sender, DoWorkEventArgs e)
        {
            _excelDataConverter.ConvertExelData(_pagesXmlFile, _linksXmlFile);
        }
        #endregion
    }
}