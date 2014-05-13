using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PageRankCalculator.BusinessModel;
using PageRankCalculator.Model;
using Vector = PageRankCalculator.Model.Vector;

namespace LuceneSearchClient.ViewModel
{

    public class SimulatorViewModel : ViewModelBase
    {
        #region Consts
        public const string MatrixSizePropertyName = "MatrixSize";
        public const string TransitionMatrixPropertyName = "AdjacenteMatrix";
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
        #endregion
        #region Fields
        private ulong _matrixSize;
        private Matrix _adjacenteMatrix;
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
        private List<KeyValuePair<float, float>> _listDampPr;

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
                RaisePropertyChanged(TransitionMatrixPropertyName);
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
                if (DataSourceListDampPr == null || _selectedPage==null) return;
                if (DataSourceListDampPr.Count == 0) return;
                
                ListDampPr = DataSourceListDampPr[int.Parse(SelectedPage.Trim())];
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
        public List<KeyValuePair<float, float>> ListDampPr
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
                                              RaisePropertyChanged(TransitionMatrixPropertyName);
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
                                              var transitionMatrix = new Matrix(AdjacenteMatrix);
                                              transitionMatrix.ToProbablityMatrix();
                                              var pageRank = new PageRank(transitionMatrix, DampingFactor, TelePortationMatrix);
                                              PageRankVector = !AutomaticIterations ? pageRank.GetPageRankVector(InitialPageRank, 5, out nbIterations) : pageRank.GetPageRankVector(InitialPageRank, (ulong)NumberIterations);
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
        private RelayCommand _startSimulationCommand;
        public RelayCommand StartSimulationCommand
        {
            get
            {
                return _startSimulationCommand
                    ?? (_startSimulationCommand = new RelayCommand(
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
                                              //ulong nbIterations = 0;
                                              //AdjacenteMatrix.ToProbablityMatrix();
                                              //RaisePropertyChanged(TransitionMatrixPropertyName);
                                              //var pageRank = new PageRank(AdjacenteMatrix, DampingFactor);
                                              //// pageRank.GoogleMatrix();
                                              //PageRankVector = !AutomaticIterations ? pageRank.GetPageRankVector(InitialPageRank, (short)5, out nbIterations) : pageRank.GetPageRankVector(InitialPageRank, (ulong)NumberIterations);
                                              //RaisePropertyChanged(PageRankVectorPropertyName);
                                              //NumberIterations = (int)nbIterations;

                                          }));
            }
        }
        private RelayCommand _resetChartsCommand;
        public RelayCommand ResetChartsCommand
        {
            get
            {
                return _resetChartsCommand
                    ?? (_resetChartsCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
            }
        }
        private RelayCommand _exportChartCommand;
        public RelayCommand ExportChartCommand
        {
            get
            {
                return _exportChartCommand
                    ?? (_exportChartCommand = new RelayCommand(
                                          () =>
                                          {

                                          }));
            }
        }
        #endregion


    }
}