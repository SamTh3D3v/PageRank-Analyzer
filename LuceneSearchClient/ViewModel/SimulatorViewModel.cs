using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PageRankCalculator.Model;
using PageRankCalculator.PageRankCalculation;
using TextBox = System.Windows.Controls.TextBox;
using Vector = PageRankCalculator.Model.Vector;

namespace LuceneSearchClient.ViewModel
{

    public class SimulatorViewModel : ViewModelBase
    {
        #region Consts
        public const string MatrixSizePropertyName = "MatrixSize";
        public const string AdjacenceMatrixPropertyName = "AdjacenteMatrix";
        public const string DefaultInitialPageRankPropertyName = "DefaultInitialPageRank";
        public const string DampingFactorPropertyName = "DampingFactor";
        public const string NumberIterationsPropertyName = "NumberIterations";
        public const string InitialPageRankPropertyName = "InitialPageRank";
        public const string AutomaticIterationsPropertyName = "AAutomaticIterations";
        public const string PageRankVectorPropertyName = "PageRankVector";
        public const string TelePortationMatrixPropertyName = "TelePortationMatrix";
        public const string ListWebPagesPropertyName = "ListWebPages";
        public const string SelectedPagePropertyName = "SelectedPage";
        public const string DataSourceListDampPrPropertyName = "DataSourceListDampPr";
        public const string ListDampPrPropertyName = "ListDampPr";
        public const string ListDampItPropertyName = "ListDampIt";
        public const string TransitionMatrixPropertyName = "TransitionMatrix";
        public const string TransitionMatrixIsCalculatedPropertyName = "TransitionMatrixIsCalculated";
        public const string AmelioratedPageRankVectorPropertyName = "AmelioratedPageRankVector";
        public const string EignValuesVectorPropertyName = "EignValuesVector";
        public const string ListPrPagesPropertyName = "ListPrPages";
        #endregion
        #region Fields
        private ulong _matrixSize;
        private ObservableCollection<KeyValuePair<float, float>> _listPrPages;
        private Matrix _adjacenteMatrix;
        private Matrix _transitionMatrix;
        private float _dampingFactor;
        private float _defaultInitialPageRank;
        private ulong _numberIterations;
        private Vector _initialPageRank;
        private bool _automaticIterations = true;
        private Vector _pageRank;
        private Matrix _teleportationMatrix;
        private ObservableCollection<string> _listWebPages = new ObservableCollection<string>();
        private string _selectedPage;
        private List<List<KeyValuePair<float, float>>> _dataSourceListDampPr = new List<List<KeyValuePair<float, float>>>();
        private ObservableCollection<KeyValuePair<float, float>> _listDampPr;
        private ObservableCollection<KeyValuePair<float, ulong>> _listDampIt;
        private Visibility _transitionMatrixIsCalculated = Visibility.Hidden;
        private Vector _amelioratedPageRankVector;
        private Vector _eignValuesVector;
        #endregion
        #region Properties
        public ulong MatrixSize
        {
            get
            {
                return _matrixSize;
            }
            set
            {
                if (_matrixSize == value)
                {
                    return;
                }
                _matrixSize = value;
                AdjacenteMatrix = new Matrix(_matrixSize);
                InitialPageRank = Vector.e(VectorType.Row, _matrixSize);
                TelePortationMatrix = Matrix.E(_matrixSize);
                var listpages = new List<string>();
                for (ulong i = 0; i < _matrixSize; i++)
                {
                    listpages.Add(i.ToString(CultureInfo.InvariantCulture));
                }
                ListWebPages = new ObservableCollection<string>(listpages);
                SelectedPage = ListWebPages.First();
            }
        }
        public Matrix AdjacenteMatrix
        {
            get
            {
                return _adjacenteMatrix;
            }

            set
            {
                if (_adjacenteMatrix == value)
                {
                    return;
                }

                _adjacenteMatrix = value;
                RaisePropertyChanged(AdjacenceMatrixPropertyName);
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
                TransitionMatrixIsCalculated = Visibility.Visible;
            }
        }
        public Visibility TransitionMatrixIsCalculated
        {
            get
            {
                return _transitionMatrixIsCalculated;
            }

            set
            {
                if (_transitionMatrixIsCalculated == value)
                {
                    return;
                }


                _transitionMatrixIsCalculated = value;
                RaisePropertyChanged(TransitionMatrixIsCalculatedPropertyName);
            }
        }
        public float DampingFactor
        {
            get
            {
                return _dampingFactor;
            }

            set
            {
                if (Equals(_dampingFactor, value))
                {
                    return;
                }

                _dampingFactor = value;
                RaisePropertyChanged(DampingFactorPropertyName);
            }
        }
        public float DefaultInitialPageRank
        {
            get
            {
                return _defaultInitialPageRank;
            }

            set
            {
                if (Equals(_defaultInitialPageRank, value))
                {
                    return;
                }
                _defaultInitialPageRank = value;
                RaisePropertyChanged(DefaultInitialPageRankPropertyName);
            }
        }
        public ulong NumberIterations
        {
            get
            {
                return _numberIterations;
            }

            set
            {
                if (_numberIterations == value)
                {
                    return;
                }

                _numberIterations = value;
                RaisePropertyChanged(NumberIterationsPropertyName);
            }
        }
        public Vector InitialPageRank
        {
            get
            {
                return _initialPageRank;
            }

            set
            {
                if (_initialPageRank == value)
                {
                    return;
                }
                _initialPageRank = value;
                RaisePropertyChanged(InitialPageRankPropertyName);
            }
        }
        public bool AutomaticIterations
        {
            get
            {
                return _automaticIterations;
            }

            set
            {
                if (_automaticIterations == value)
                {
                    return;
                }
                _automaticIterations = value;
                RaisePropertyChanged(AutomaticIterationsPropertyName);
            }
        }
        public Vector PageRankVector
        {
            get
            {
                return _pageRank;
            }

            set
            {
                if (_pageRank == value)
                {
                    return;
                }
                _pageRank = value;
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
        public Matrix TelePortationMatrix
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
                RaisePropertyChanged(TelePortationMatrixPropertyName);
            }
        }
        public ObservableCollection<string> ListWebPages
        {
            get
            {
                return _listWebPages;
            }

            set
            {
                if (_listWebPages == value)
                {
                    return;
                }
                _listWebPages = value;
                RaisePropertyChanged(ListWebPagesPropertyName);
            }
        }
        public string SelectedPage
        {
            get
            {
                return _selectedPage;
            }

            set
            {
                if (_selectedPage == value)
                {
                    return;
                }
                _selectedPage = value;
                RaisePropertyChanged(SelectedPagePropertyName);
                if (DataSourceListDampPr == null || _selectedPage == null) return;
                if (DataSourceListDampPr.Count == 0) return;

                ListDampPr = new ObservableCollection<KeyValuePair<float, float>>(DataSourceListDampPr[int.Parse(SelectedPage.Trim())]);
            }
        }
        public List<List<KeyValuePair<float, float>>> DataSourceListDampPr
        {
            get
            {
                return _dataSourceListDampPr;
            }

            set
            {
                if (_dataSourceListDampPr == value)
                {
                    return;
                }

                _dataSourceListDampPr = value;
                RaisePropertyChanged(DataSourceListDampPrPropertyName);
            }
        }
        public ObservableCollection<KeyValuePair<float, float>> ListDampPr
        {
            get
            {
                return _listDampPr;
            }

            set
            {
                if (_listDampPr == value)
                {
                    return;
                }

                _listDampPr = value;
                RaisePropertyChanged(ListDampPrPropertyName);
            }
        }
        public ObservableCollection<KeyValuePair<float, ulong>> ListDampIt
        {
            get
            {
                return _listDampIt;
            }

            set
            {
                if (_listDampIt == value)
                {
                    return;
                }

                _listDampIt = value;
                RaisePropertyChanged(ListDampItPropertyName);
            }
        }

        

        


        public ObservableCollection<KeyValuePair<float, float>> ListPrPages
        {
            get
            {
                return _listPrPages;
            }

            set
            {
                if (_listPrPages == value)
                {
                    return;
                }
                _listPrPages = value;
                RaisePropertyChanged(ListPrPagesPropertyName);
            }
        }

        public const string ListAprPagesPropertyName = "ListAprPages";

        private ObservableCollection<KeyValuePair<float, float>> _listAprPages;


        public ObservableCollection<KeyValuePair<float, float>> ListAprPages
        {
            get
            {
                return _listAprPages;
            }

            set
            {
                if (_listAprPages == value)
                {
                    return;
                }

                _listAprPages = value;
                RaisePropertyChanged(ListAprPagesPropertyName);
            }
        }
        public Vector EignValuesVector
        {
            get
            {
                return _eignValuesVector;
            }

            set
            {
                if (_eignValuesVector == value)
                {
                    return;
                }

                _eignValuesVector = value;
                RaisePropertyChanged(EignValuesVectorPropertyName);
            }
        }
        #endregion
        #region Ctors and Methods
        public SimulatorViewModel()
        {
            DampingFactor = PageRank.DefaultDampingFactor;
            NumberIterations = 100;
        }
        #endregion
        #region Commands
        private RelayCommand _generateMatrixCommand;
        public RelayCommand GenerateMatrixCommand
        {
            get
            {
                return _generateMatrixCommand
                    ?? (_generateMatrixCommand = new RelayCommand(
                                          () =>
                                          {
                                              var rand = new Random();
                                              for (ulong i = 0; i < MatrixSize; i++)
                                              {
                                                  for (ulong j = 0; j < MatrixSize; j++)
                                                  {
                                                      AdjacenteMatrix[i, j] = rand.Next(2);
                                                  }

                                              }
                                              RaisePropertyChanged(AdjacenceMatrixPropertyName);
                                          }));
            }
        }
        private RelayCommand _resetMatrixCommand;
        public RelayCommand ResetMatrixCommand
        {
            get
            {
                return _resetMatrixCommand
                    ?? (_resetMatrixCommand = new RelayCommand(
                                          () =>
                                          {
                                              AdjacenteMatrix = new Matrix(MatrixSize);
                                          }));
            }
        }
        private RelayCommand _resetInitialPageRankCommand;
        public RelayCommand ResetInitialPageRankCommand
        {
            get
            {
                return _resetInitialPageRankCommand
                    ?? (_resetInitialPageRankCommand = new RelayCommand(
                                          () =>
                                          {
                                              InitialPageRank = Vector.e(VectorType.Row, _matrixSize);
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
                                              ulong nbIterations = 0;
                                              TransitionMatrix = new Matrix(AdjacenteMatrix);
                                              TransitionMatrix.ToProbablityMatrix();
                                              RaisePropertyChanged(TransitionMatrixPropertyName);
                                              var pageRank = new PageRank(TransitionMatrix, DampingFactor, TelePortationMatrix);
                                              var dateTime = DateTime.Now;
                                              PageRankVector = !AutomaticIterations ? pageRank.GetPageRankVector(InitialPageRank, 10, out nbIterations) : pageRank.GetPageRankVector(InitialPageRank, (ulong)NumberIterations);
                                              Debug.WriteLine("PageRank Calculated In -> " + (DateTime.Now - dateTime).Milliseconds + " Miliseconds , Iterations ->" + NumberIterations);
                                              if (!AutomaticIterations)
                                                  NumberIterations = nbIterations;
                                          }));
            }
        }
        private RelayCommand _resetPageRankVector;
        public RelayCommand ResetPageRankVector
        {
            get
            {
                return _resetPageRankVector
                    ?? (_resetPageRankVector = new RelayCommand(
                                          () =>
                                          {
                                              PageRankVector = InitialPageRank;
                                          }));
            }
        }
        private RelayCommand _resetTelePortationCommand;
        public RelayCommand ResetTelePortationCommand
        {
            get
            {
                return _resetTelePortationCommand
                    ?? (_resetTelePortationCommand = new RelayCommand(
                                          () =>
                                          {
                                              TelePortationMatrix = Matrix.E(_matrixSize);
                                          }));
            }
        }
        private RelayCommand _startSimulationPrCommand;
        public RelayCommand StartSimulationPrCommand
        {
            get
            {
                return _startSimulationPrCommand
                    ?? (_startSimulationPrCommand = new RelayCommand(
                                          () =>
                                          {
                                              DataSourceListDampPr = new List<List<KeyValuePair<float, float>>>();
                                              for (ulong i = 0; i < MatrixSize; i++)
                                              {
                                                  DataSourceListDampPr.Add(new List<KeyValuePair<float, float>>());
                                              }
                                              for (float dampFactor = 0; dampFactor <= 1; dampFactor += 0.1f)
                                              {


                                                  ulong nbIterations = 0;
                                                  var transitionMatrix = new Matrix(AdjacenteMatrix);
                                                  transitionMatrix.ToProbablityMatrix();
                                                  var pageRank = new PageRank(transitionMatrix, dampFactor, TelePortationMatrix);
                                                  var prVector = pageRank.GetPageRankVector(InitialPageRank, 5,
                                                      out nbIterations);
                                                  for (ulong i = 0; i < prVector.Size; i++)
                                                  {
                                                      DataSourceListDampPr[(int)i].Add(new KeyValuePair<float, float>(dampFactor, prVector[i]));
                                                  }
                                              }
                                              ListDampPr = new ObservableCollection<KeyValuePair<float, float>>(DataSourceListDampPr[int.Parse(SelectedPage.Trim())]);
                                              //ulong nbIterations = 0;
                                              //AdjacenteMatrix.ToProbablityMatrix();
                                              //RaisePropertyChanged(AdjacenceMatrixPropertyName);
                                              //var pageRank = new PageRank(AdjacenteMatrix, DampingFactor);
                                              //// pageRank.GoogleMatrix();
                                              //PageRankVector = !AutomaticIterations ? pageRank.GetPageRankVector(InitialPageRank, (short)5, out nbIterations) : pageRank.GetPageRankVector(InitialPageRank, (ulong)NumberIterations);
                                              //RaisePropertyChanged(PageRankVectorPropertyName);
                                              //NumberIterations = (int)nbIterations;

                                          }));
            }
        }
        private RelayCommand _resetChartsPrCommand;
        private RelayCommand _startSimulationPrAprCommand;
        public RelayCommand StartSimulationPrAprCommand
        {
            get
            {
                return _startSimulationPrAprCommand
                    ?? (_startSimulationPrAprCommand = new RelayCommand(
                                          () =>
                                          {
                                              var listPr = new List<KeyValuePair<float, float>>();
                                              var listAPr = new List<KeyValuePair<float, float>>();
                                              for (float page = 0; page < TransitionMatrix.Size; page++)
                                              {                                                  
                                                  listPr.Add(new KeyValuePair<float, float>(page, PageRankVector[(ulong)page]));
                                                  listAPr.Add(new KeyValuePair<float, float>(page, AmelioratedPageRankVector[(ulong)page]));
                                              }
                                              ListPrPages = new ObservableCollection<KeyValuePair<float, float>>(listPr);
                                              ListAprPages= new ObservableCollection<KeyValuePair<float, float>>(listAPr);
                                          }));
            }
        }
        public RelayCommand ResetChartsPrCommand
        {
            get
            {
                return _resetChartsPrCommand
                    ?? (_resetChartsPrCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
            }
        }
        private RelayCommand _exportChartPrCommand;
        public RelayCommand ExportChartPrCommand
        {
            get
            {
                return _exportChartPrCommand
                    ?? (_exportChartPrCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
            }
        }
        private RelayCommand _startSimulationItCommand;
        public RelayCommand StartSimulationItCommand
        {
            get
            {
                return _startSimulationItCommand
                    ?? (_startSimulationItCommand = new RelayCommand(
                                          () =>
                                          {
                                              ListDampIt = new ObservableCollection<KeyValuePair<float, ulong>>();
                                              for (float dampFactor = 0; dampFactor <= 1; dampFactor += 0.1f)
                                              {


                                                  ulong nbIterations = 0;
                                                  var transitionMatrix = new Matrix(AdjacenteMatrix);
                                                  transitionMatrix.ToProbablityMatrix();
                                                  var pageRank = new PageRank(transitionMatrix, dampFactor, TelePortationMatrix);
                                                  pageRank.GetPageRankVector(InitialPageRank, 5,
                                                      out nbIterations);
                                                  ListDampIt.Add(new KeyValuePair<float, ulong>(dampFactor, nbIterations));
                                              }
                                          }));
            }
        }
        private RelayCommand _resetChartsItCommand;
        public RelayCommand ResetChartsItCommand
        {
            get
            {
                return _resetChartsItCommand
                    ?? (_resetChartsItCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
            }
        }
        private RelayCommand _exportChartItCommand;
        public RelayCommand ExportChartItCommand
        {
            get
            {
                return _exportChartItCommand
                    ?? (_exportChartItCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
            }
        }

        private RelayCommand<DataGridCellEditEndingEventArgs> _initialVecCellEditEndingCommand;
        public RelayCommand<DataGridCellEditEndingEventArgs> InitialVecCellEditEndingCommand
        {
            get
            {
                return _initialVecCellEditEndingCommand
                       ?? (_initialVecCellEditEndingCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(
                           (args) =>
                           {
                               var editedTextbox = args.EditingElement as TextBox;
                               if (editedTextbox != null)
                                   InitialPageRank[(ulong)args.Column.DisplayIndex] = float.Parse(editedTextbox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                           }));
            }
        }

        private RelayCommand<DataGridCellEditEndingEventArgs> _adjMatCellEditEndingCommand;
        public RelayCommand<DataGridCellEditEndingEventArgs> AdjMatCellEditEndingCommand
        {
            get
            {
                return _adjMatCellEditEndingCommand
                    ?? (_adjMatCellEditEndingCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(
                                          (args) =>
                                          {
                                              var editedTextbox = args.EditingElement as TextBox;
                                              if (editedTextbox != null)
                                                  AdjacenteMatrix[(ulong)args.Row.GetIndex(), (ulong)args.Column.DisplayIndex] = float.Parse(editedTextbox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                                          }));
            }
        }
        private RelayCommand<DataGridCellEditEndingEventArgs> _telMatCellEditEndingCommand;
        public RelayCommand<DataGridCellEditEndingEventArgs> TelMatCellEditEndingCommand
        {
            get
            {
                return _telMatCellEditEndingCommand
                    ?? (_telMatCellEditEndingCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(
                                          (args) =>
                                          {
                                              var editedTextbox = args.EditingElement as TextBox;
                                              if (editedTextbox != null)
                                                  TelePortationMatrix[(ulong)args.Row.GetIndex(), (ulong)args.Column.DisplayIndex] = float.Parse(editedTextbox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                                          }));
            }
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
                                              ulong nbIterations = 0;
                                              TransitionMatrix = new Matrix(AdjacenteMatrix);
                                              TransitionMatrix.ToProbablityMatrix();
                                              RaisePropertyChanged(TransitionMatrixPropertyName);
                                              var amPageRank = new PageRank(TransitionMatrix, DampingFactor, TelePortationMatrix);
                                              var dateTime = DateTime.Now;
                                              AmelioratedPageRankVector = !AutomaticIterations ? amPageRank.GetAmelioratedPageRankVector(InitialPageRank, 10, out nbIterations) : amPageRank.GetAmelioratedPageRankVector(InitialPageRank, (ulong)NumberIterations);
                                              Debug.WriteLine("Ameliorated PageRank Calculated In ->" + (DateTime.Now - dateTime).Milliseconds + " Miliseconds , Iterations ->" + NumberIterations);
                                              if (!AutomaticIterations)
                                                  NumberIterations = nbIterations;
                                          }));
            }
        }
        private RelayCommand _resetAmelioratedPageRankVector;
        public RelayCommand ResetAmelioratedPageRankVector
        {
            get
            {
                return _resetAmelioratedPageRankVector
                    ?? (_resetAmelioratedPageRankVector = new RelayCommand(
                                          () =>
                                          {
                                              AmelioratedPageRankVector = InitialPageRank;
                                          }));
            }
        }
        private RelayCommand _calculateEignValuesCommand;
        public RelayCommand CalculateEignValuesCommand
        {
            get
            {
                return _calculateEignValuesCommand
                    ?? (_calculateEignValuesCommand = new RelayCommand(
                                          () =>
                                          {
                                              var listEv = TransitionMatrix.Eigenvalues();
                                              EignValuesVector = new Vector(VectorType.Row, (ulong)listEv.Count);
                                              for (int i = 0; i < listEv.Count; i++)
                                              {
                                                  EignValuesVector[(ulong)i] = listEv[i];
                                              }
                                              RaisePropertyChanged(EignValuesVectorPropertyName);
                                          }));
            }
        }
        #endregion
    }
}