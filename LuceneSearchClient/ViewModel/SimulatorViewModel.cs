using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LuceneSearchClient.Model;
using PageRankCalculator.Model;
using PageRankCalculator.PageRankCalculation;
using QuickGraph;
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
        public const string NumberIterationsPropertyName = "PrNumberIterations";
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
        public const string AprNumberOfIterationPropertyName = "AprNumberOfIteration";
        public const string PrPrecisionPropertyName = "PrPrecision";
        public const string AprprecisionPropertyName = "Aprprecision";
        public const string ListAprPagesPropertyName = "ListAprPages";
        public const string GoogleMatrixIsCalculatedPropertyName = "GoogleMatrixIsCalculated";
        public const string GoogleMatrixPropertyName = "GoogleMatrix";
        public const string AutomaticIterationsAprPropertyName = "AutomaticIterationsApr";
        public const string ListMatricesLabelsPropertyName = "ListMatricesLabels";
        public const string SelectedMatriceEvPropertyName = "SelectedMatriceEv";
        public const string ListPrIteMatPropertyName = "ListPrIteMat";
        public const string ListAPrIteMatPropertyName = "ListAPrIteMat";
        public const string ListPrTimeMatPropertyName = "ListPrTimeMat";
        public const string ListAPrTimeMatPropertyName = "ListAPrTimeMat";
        public const string LayoutAlgorithmTypesPropertyName = "LayoutAlgorithmTypes";
        public const string SelectedLayoutAlgorithmeTtypePropertyName = "SelectedLayoutAlgorithmeTtype";
        public const string WebGraphPropertyName = "WebGraph";
        public const string BusyIndicatorPropertyName = "BusyIndicator";
        public const string InputOutputRatioIsCalculatedPropertyName = "InputOutputRatioIsCalculated";
        public const string InputOutputRatioPropertyName = "InputOutputRatio";
        public const string EdgeSourcePropertyName = "EdgeSource";
        public const string EdgeTargetPropertyName = "EdgeTarget";
        #endregion
        #region Fields
        private ulong _matrixSize;
        private ObservableCollection<KeyValuePair<float, float>> _listPrPages;
        private Matrix _adjacenteMatrix;
        private Matrix _transitionMatrix;
        private float _dampingFactor;
        private float _defaultInitialPageRank;
        private ulong _prNumberIterations;
        private Vector _initialPageRank;
        private bool _automaticIterations = false;
        private bool _automaticIterationsApr = false;
        private Vector _pageRank;
        private Matrix _teleportationMatrix;
        private ObservableCollection<string> _listWebPages = new ObservableCollection<string>();
        private string _selectedPage;
        private List<List<KeyValuePair<float, float>>> _dataSourceListDampPr = new List<List<KeyValuePair<float, float>>>();
        private ObservableCollection<KeyValuePair<float, float>> _listDampPr;
        private ObservableCollection<KeyValuePair<float, ulong>> _listDampIt;
        private ObservableCollection<KeyValuePair<ulong, ulong>> _listPrIteMat;
        private ObservableCollection<KeyValuePair<ulong, ulong>> _listAPrIteMat;
        private Visibility _transitionMatrixIsCalculated = Visibility.Hidden;
        private Vector _amelioratedPageRankVector;
        private Vector _eignValuesVector;
        private ulong _aprNumberOfIteration;
        private short _aprPrecesion = 5;
        private short _prPrecision = 5;
        private ObservableCollection<KeyValuePair<float, float>> _listAprPages;
        private Matrix _googleMatrix;
        private Visibility _googleMatrixIsCalculated = Visibility.Hidden;
        private Visibility _inputOutputRatioIsCalculated = Visibility.Hidden;
        private List<string> _listMatricesLabelsList = new List<string>()
        {
            "google Matrix",
            "Transition Matrix",
            "Adjacence Matrix"
        };
        private string _selectedMatriceEv = "google Matrix";
        private ObservableCollection<KeyValuePair<ulong, ulong>> _listPrTimeMat;
        private ObservableCollection<KeyValuePair<ulong, ulong>> _listAPrTimeMat;
        private List<string> _layoutAlgorithmTypes = new List<string>
        {
            "BoundedFR",
            "Circular",
            "CompoundFDP",
            "EfficientSugiyama",
            "FR",
            "ISOM",
            "KK",
            "LinLog",
            "Tree"
        };
        private string _selectedLayoutAlgorithmeType = "KK";
        private WebGraph _webGraph;
        private bool _busyIndicator = false;
        private Matrix _inputOutputRatio;
        private string _edgeSource;
        private string _edgeTarget;
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
                //Generate The WebGraph
                WebGraph = new WebGraph(false);
                foreach (var page in listpages)
                    WebGraph.AddVertex(new WebVertex() { Label = page });
                RaisePropertyChanged(WebGraphPropertyName);

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
        public ulong PrNumberIterations
        {
            get
            {
                return _prNumberIterations;
            }

            set
            {
                if (_prNumberIterations == value)
                {
                    return;
                }

                _prNumberIterations = value;
                RaisePropertyChanged(NumberIterationsPropertyName);
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
        public short Aprprecision
        {
            get
            {
                return _aprPrecesion;
            }

            set
            {
                if (_aprPrecesion == value)
                {
                    return;
                }

                _aprPrecesion = value;
                RaisePropertyChanged(AprprecisionPropertyName);
            }
        }
        public ulong AprNumberOfIteration
        {
            get
            {
                return _aprNumberOfIteration;
            }

            set
            {
                if (_aprNumberOfIteration == value)
                {
                    return;
                }

                _aprNumberOfIteration = value;
                RaisePropertyChanged(AprNumberOfIterationPropertyName);
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
        public bool AutomaticIterationsApr
        {
            get
            {
                return _automaticIterationsApr;
            }

            set
            {
                if (_automaticIterationsApr == value)
                {
                    return;
                }

                _automaticIterationsApr = value;
                RaisePropertyChanged(AutomaticIterationsAprPropertyName);
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
        public ObservableCollection<KeyValuePair<ulong, ulong>> ListPrIteMat
        {
            get
            {
                return _listPrIteMat;
            }

            set
            {
                if (_listPrIteMat == value)
                {
                    return;
                }

                _listPrIteMat = value;
                RaisePropertyChanged(ListPrIteMatPropertyName);
            }
        }
        public ObservableCollection<KeyValuePair<ulong, ulong>> ListAPrIteMat
        {
            get
            {
                return _listAPrIteMat;
            }

            set
            {
                if (_listAPrIteMat == value)
                {
                    return;
                }

                _listAPrIteMat = value;
                RaisePropertyChanged(ListAPrIteMatPropertyName);
            }
        }
        public ObservableCollection<KeyValuePair<ulong, ulong>> ListAPrTimeMat
        {
            get
            {
                return _listAPrTimeMat;
            }

            set
            {
                if (_listAPrTimeMat == value)
                {
                    return;
                }

                _listAPrTimeMat = value;
                RaisePropertyChanged(ListAPrTimeMatPropertyName);
            }
        }
        public ObservableCollection<KeyValuePair<ulong, ulong>> ListPrTimeMat
        {
            get
            {
                return _listPrTimeMat;
            }

            set
            {
                if (_listPrTimeMat == value)
                {
                    return;
                }

                _listPrTimeMat = value;
                RaisePropertyChanged(ListPrTimeMatPropertyName);
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
        public Matrix GoogleMatrix
        {
            get
            {
                return _googleMatrix;
            }

            set
            {
                if (_googleMatrix == value)
                {
                    return;
                }

                _googleMatrix = value;
                RaisePropertyChanged(GoogleMatrixPropertyName);
                GoogleMatrixIsCalculated = Visibility.Visible;
            }
        }
        public Matrix InputOutputRatio
        {
            get
            {
                return _inputOutputRatio;
            }

            set
            {
                if (_inputOutputRatio == value)
                {
                    return;
                }
                
                _inputOutputRatio = value;
                RaisePropertyChanged(InputOutputRatioPropertyName);
                InputOutputRatioIsCalculated = Visibility.Visible;
            }
        }
        public Visibility GoogleMatrixIsCalculated
        {
            get
            {
                return _googleMatrixIsCalculated;
            }

            set
            {
                if (_googleMatrixIsCalculated == value)
                {
                    return;
                }
                _googleMatrixIsCalculated = value;
                RaisePropertyChanged(GoogleMatrixIsCalculatedPropertyName);
            }
        }       
        public Visibility InputOutputRatioIsCalculated
        {
            get
            {
                return _inputOutputRatioIsCalculated;
            }

            set
            {
                if (_inputOutputRatioIsCalculated == value)
                {
                    return;
                }

                RaisePropertyChanging(InputOutputRatioIsCalculatedPropertyName);
                _inputOutputRatioIsCalculated = value;
                RaisePropertyChanged(InputOutputRatioIsCalculatedPropertyName);
            }
        }
        public List<string> ListMatricesLabels
        {
            get
            {
                return _listMatricesLabelsList;
            }

            set
            {
                if (_listMatricesLabelsList == value)
                {
                    return;
                }
                _listMatricesLabelsList = value;
                RaisePropertyChanged(ListMatricesLabelsPropertyName);
            }
        }
        public string SelectedMatriceEv
        {
            get
            {
                return _selectedMatriceEv;
            }

            set
            {
                if (_selectedMatriceEv == value)
                {
                    return;
                }

                _selectedMatriceEv = value;
                RaisePropertyChanged(SelectedMatriceEvPropertyName);
            }
        }
        public List<string> LayoutAlgorithmTypes
        {
            get
            {
                return _layoutAlgorithmTypes;
            }

            set
            {
                if (_layoutAlgorithmTypes == value)
                {
                    return;
                }

                _layoutAlgorithmTypes = value;
                RaisePropertyChanged(LayoutAlgorithmTypesPropertyName);
            }
        }
        public string SelectedLayoutAlgorithmeTtype
        {
            get
            {
                return _selectedLayoutAlgorithmeType;
            }

            set
            {
                if (_selectedLayoutAlgorithmeType == value)
                {
                    return;
                }
                _selectedLayoutAlgorithmeType = value;
                RaisePropertyChanged(SelectedLayoutAlgorithmeTtypePropertyName);
            }
        }
        public WebGraph WebGraph
        {
            get
            {
                return _webGraph;
            }

            set
            {
                if (_webGraph == value)
                {
                    return;
                }

                _webGraph = value;
                RaisePropertyChanged(WebGraphPropertyName);
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
        public string EdgeSource
        {
            get
            {
                return _edgeSource;
            }

            set
            {
                if (_edgeSource == value)
                {
                    return;
                }
                
                _edgeSource = value;
                RaisePropertyChanged(EdgeSourcePropertyName);
            }
        }
        public string EdgeTarget
        {
            get
            {
                return _edgeTarget;
            }

            set
            {
                if (_edgeTarget == value)
                {
                    return;
                }                
                _edgeTarget = value;
                RaisePropertyChanged(EdgeTargetPropertyName);
                AdjacenteMatrix[ulong.Parse(EdgeSource), ulong.Parse(EdgeTarget)] =1;                
                    AddNewGraphEdge(EdgeSource,EdgeTarget);
                    PageRankVector = null;
                    AmelioratedPageRankVector = null;
                RaisePropertyChanged(WebGraphPropertyName);
                RaisePropertyChanged(AdjacenceMatrixPropertyName);
                _edgeSource = "";
                _edgeTarget = "";
                ResetMatrices();
            }
        }
        #endregion
        #region Ctors and Methods
        public SimulatorViewModel()
        {
            DampingFactor = PageRank.DefaultDampingFactor;
            PrNumberIterations = 100;
            AprNumberOfIteration = 100;
            Messenger.Default.Register<string>(this, "removenode", (node) =>
            {
                _matrixSize = MatrixSize - 1;
                ListWebPages.Remove(ListWebPages[ListWebPages.Count-1]);
                SelectedPage = ListWebPages.FirstOrDefault();
                var newAdjMatrix = new Matrix(_matrixSize);
                InitialPageRank = Vector.e(VectorType.Row, _matrixSize);
                TelePortationMatrix = Matrix.E(_matrixSize);

                PageRankVector = null;
                AmelioratedPageRankVector = null;
                ulong nodeUlong = ulong.Parse(node);
                for (ulong i = 0; i < MatrixSize ; i++)
                {
                    for (ulong j = 0; j < MatrixSize ; j++)
                    {
                        var d = i >= nodeUlong ? i + 1 : i;
                        newAdjMatrix[i, j] = AdjacenteMatrix[i >= nodeUlong ? i + 1 : i, j >= nodeUlong?j+1:j];
                    }
                }
                AdjacenteMatrix = newAdjMatrix;
               WebGraph.RemoveVertex (WebGraph.Vertices.First(x => x.Label==node));
                
             
                //for (ulong label =nodeUlong+1 ; label < MatrixSize+1; label++)
                //{
                //    var lab = label.ToString(CultureInfo.InvariantCulture);
                //    WebGraph.Vertices.First(v => v.Label==lab).Label=(label-1).ToString(CultureInfo.InvariantCulture);
                //}
                RaisePropertyChanged(AdjacenceMatrixPropertyName);
                RaisePropertyChanged(WebGraphPropertyName);
                ResetMatrices(); 
               
            });
            Messenger.Default.Register<WebEdge>(this, "removeedge", (edge) =>
            {
                //Remove The edge From The Graph 
                //Remove The edge From The Matrix 
                //Reset all Pregenerated Matrices

            });
            Messenger.Default.Register<Matrix>(this, "exporttosimulator", (adjMatrix) =>
            {
                _matrixSize = adjMatrix.Size;
                AdjacenteMatrix = adjMatrix;
                InitialPageRank = Vector.e(VectorType.Row, _matrixSize);
                TelePortationMatrix = Matrix.E(_matrixSize);
                var listpages = new List<string>();
                for (ulong i = 0; i < _matrixSize; i++)
                {
                    listpages.Add(i.ToString(CultureInfo.InvariantCulture));
                }
                ListWebPages = new ObservableCollection<string>(listpages);
                SelectedPage = ListWebPages.First();
                //Generate The WebGraph
                WebGraph = new WebGraph(false);
                foreach (var page in listpages)
                    WebGraph.AddVertex(new WebVertex() { Label = page });

                for (ulong i = 0; i < MatrixSize; i++)
                {
                    for (ulong j = 0; j < MatrixSize; j++)
                    {                        
                        if (AdjacenteMatrix[i, j] != 0)
                            AddNewGraphEdge(i.ToString(CultureInfo.InvariantCulture), j.ToString(CultureInfo.InvariantCulture));
                    }
                }
                RaisePropertyChanged(AdjacenceMatrixPropertyName);                
                RaisePropertyChanged(WebGraphPropertyName);
            });
            Messenger.Default.Register<string>(this, "startedge", (se) => EdgeSource = se);
            Messenger.Default.Register<string>(this, "endedge", (ee) => EdgeTarget = ee);
            
        }

        private void ResetMatrices()
        {
            TransitionMatrix = null;
            InputOutputRatio = null;
            GoogleMatrix = null;
            TelePortationMatrix = Matrix.E(MatrixSize);
            GoogleMatrixIsCalculated = Visibility.Hidden;
            TransitionMatrixIsCalculated=Visibility.Hidden;
            InputOutputRatioIsCalculated = Visibility.Hidden;


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
                                              BusyIndicator = true;
                                              var rand = new Random();
                                              for (ulong i = 0; i < MatrixSize; i++)
                                              {
                                                  for (ulong j = 0; j < MatrixSize; j++)
                                                  {
                                                      AdjacenteMatrix[i, j] = rand.Next(2);
                                                      if (AdjacenteMatrix[i, j] != 0)
                                                          AddNewGraphEdge(i.ToString(CultureInfo.InvariantCulture), j.ToString(CultureInfo.InvariantCulture));
                                                  }
                                              }
                                              RaisePropertyChanged(AdjacenceMatrixPropertyName);
                                              RaisePropertyChanged(WebGraphPropertyName);
                                              BusyIndicator = false;
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
                                              WebGraph = new WebGraph(false);
                                              for (ulong page = 0; page < MatrixSize; page++)                                             
                                                  WebGraph.AddVertex(new WebVertex() { Label = page.ToString(CultureInfo.InvariantCulture) });
                                              RaisePropertyChanged(WebGraphPropertyName);
                                              ResetMatrices();
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
                                              ulong prNbIterations = 0;
                                              TransitionMatrix = new Matrix(AdjacenteMatrix);
                                              TransitionMatrix.ToProbablityMatrix();
                                              RaisePropertyChanged(TransitionMatrixPropertyName);
                                              var pageRank = new PageRank(TransitionMatrix, DampingFactor, TelePortationMatrix);
                                              GoogleMatrix = pageRank.GetGoogleMatrix();
                                              pageRank.TeleportationMatrix = TelePortationMatrix;
                                              var dateTime = DateTime.Now;
                                              PageRankVector = !AutomaticIterations ? pageRank.GetPageRankVector(InitialPageRank, PrPrecision, out prNbIterations) : pageRank.GetPageRankVector(InitialPageRank, (ulong)PrNumberIterations);
                                              Debug.WriteLine("PageRank Calculated In -> " + (DateTime.Now - dateTime).Milliseconds + " Miliseconds , Iterations ->" + PrNumberIterations);
                                              if (!AutomaticIterations)
                                                  PrNumberIterations = prNbIterations;
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
                                              ListAprPages = new ObservableCollection<KeyValuePair<float, float>>(listAPr);
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
                                              DataSourceListDampPr = new List<List<KeyValuePair<float, float>>>();
                                              ListDampPr = new ObservableCollection<KeyValuePair<float, float>>();
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
                                              ListDampIt = new ObservableCollection<KeyValuePair<float, ulong>>();
                                          }));
            }
        }
        private RelayCommand _resetChartsPrAprIteCommand;
        public RelayCommand ResetChartsPrAprIteCommand
        {
            get
            {
                return _resetChartsPrAprIteCommand
                    ?? (_resetChartsPrAprIteCommand = new RelayCommand(
                                          () =>
                                          {

                                              ListPrIteMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
                                              ListAPrIteMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
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
                                              {
                                                  var newval = float.Parse(editedTextbox.Text.Replace(",", "."),
                                                      CultureInfo.InvariantCulture);
                                                  AdjacenteMatrix[
                                                      (ulong)args.Row.GetIndex(), (ulong)args.Column.DisplayIndex] =
                                                      newval;
                                                  if (newval != 0)
                                                      AddNewGraphEdge(args.Row.GetIndex().ToString(),
                                                      args.Column.DisplayIndex.ToString());
                                                  else
                                                      RemoveGraphEdge(args.Row.GetIndex().ToString(),
                                                      args.Column.DisplayIndex.ToString());
                                                  RaisePropertyChanged(WebGraphPropertyName);
                                                  ResetMatrices();
                                              }
                                          }));
            }
        }
        private void AddNewGraphEdge(string from, string to)
        {
            var webEdge = new WebEdge(WebGraph.Vertices.Where(x => x.Label == from).First(), WebGraph.Vertices.Where(x => x.Label == to).First());
            _webGraph.AddEdge(webEdge);

        }
        private void RemoveGraphEdge(string from, string to)
        {
            var edge = _webGraph.Edges.FirstOrDefault(e => e.Source.Label == from && e.Target.Label == to);
            if (edge != null)
                _webGraph.RemoveEdge(edge);
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
                                              ulong aprNbIterations = 0;
                                              TransitionMatrix = new Matrix(AdjacenteMatrix);
                                              TransitionMatrix.ToProbablityMatrix();
                                              RaisePropertyChanged(TransitionMatrixPropertyName);
                                              var amPageRank = new PageRank(TransitionMatrix, DampingFactor, TelePortationMatrix);
                                              var dateTime = DateTime.Now;
                                              amPageRank.TeleportationMatrix = TelePortationMatrix;
                                              AmelioratedPageRankVector = !AutomaticIterationsApr ? amPageRank.GetAmelioratedPageRankVector(InitialPageRank, PrPrecision, out aprNbIterations) : amPageRank.GetAmelioratedPageRankVector(InitialPageRank, (ulong)aprNbIterations);
                                              Debug.WriteLine("Ameliorated PageRank Calculated In ->" + (DateTime.Now - dateTime).Milliseconds + " Miliseconds , Iterations ->" + PrNumberIterations);
                                              if (!AutomaticIterationsApr)
                                                  AprNumberOfIteration = aprNbIterations;

                                              InputOutputRatio = amPageRank.DampingFactorMatrix;
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
                                              List<float> listEv;
                                              switch (SelectedMatriceEv)
                                              {
                                                  case "google Matrix":
                                                      if (GoogleMatrix == null) return;
                                                      listEv = GoogleMatrix.Eigenvalues();
                                                      _eignValuesVector = new Vector(VectorType.Column, (ulong)listEv.Count);
                                                      for (int i = 0; i < listEv.Count; i++)
                                                      {
                                                          _eignValuesVector[(ulong)i] = listEv[i];
                                                      }
                                                      RaisePropertyChanged(EignValuesVectorPropertyName);
                                                      break;
                                                  case "Transition Matrix":
                                                      if (TransitionMatrix == null)
                                                      {
                                                          if (AdjacenteMatrix == null) return;
                                                          TransitionMatrix = new Matrix(AdjacenteMatrix);
                                                      }
                                                      listEv = TransitionMatrix.Eigenvalues();
                                                      _eignValuesVector = new Vector(VectorType.Column, (ulong)listEv.Count);
                                                      for (int i = 0; i < listEv.Count; i++)
                                                      {
                                                          _eignValuesVector[(ulong)i] = listEv[i];
                                                      }
                                                      RaisePropertyChanged(EignValuesVectorPropertyName);
                                                      break;
                                                  case "Adjacence Matrix":
                                                      if (AdjacenteMatrix == null) return;
                                                      listEv = AdjacenteMatrix.Eigenvalues();
                                                      _eignValuesVector = new Vector(VectorType.Column, (ulong)listEv.Count);
                                                      for (int i = 0; i < listEv.Count; i++)
                                                      {
                                                          _eignValuesVector[(ulong)i] = listEv[i];
                                                      }
                                                      RaisePropertyChanged(EignValuesVectorPropertyName);
                                                      break;
                                              }
                                          }));
            }
        }
        private RelayCommand _startSimulationPrAprIteCommand;
        public RelayCommand StartSimulationPrAprIteCommand
        {
            get
            {
                return _startSimulationPrAprIteCommand
                    ?? (_startSimulationPrAprIteCommand = new RelayCommand(
                                          () =>
                                          {
                                              
                                              var worker = new BackgroundWorker();
                                              worker.DoWork += DrawSimulationIteMatChart;
                                              worker.RunWorkerCompleted += DrawSimulationIteMatChartCompeleted;
                                              worker.RunWorkerAsync();
                                          }));
            }
        }
        private void DrawSimulationIteMatChartCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RaisePropertyChanged(ListPrIteMatPropertyName);
            RaisePropertyChanged(ListAPrIteMatPropertyName);
            
        }
        private void DrawSimulationIteMatChart(object sender, DoWorkEventArgs e)
        {
        //    BusyIndicator = true;
            _listPrIteMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
            _listAPrIteMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
            for (ulong matrixSize = 5; matrixSize <= 50; matrixSize += 5)
            {
                //Generate A Random Matrix 
                var adjacentMatrix = new Matrix(matrixSize);
                var rand = new Random();
                for (ulong i = 0; i < matrixSize; i++)
                {
                    for (ulong j = 0; j < matrixSize; j++)
                    {
                        adjacentMatrix[i, j] = rand.Next(2);
                    }
                }
                var transitionMatrix = new Matrix(adjacentMatrix);
                transitionMatrix.ToProbablityMatrix();
                //Calculate The Number Of Iteration Associated To Pr Calcul 
                ulong nbIterationsPr = 0;
                var teleportationMatrix = Matrix.E(matrixSize);
                var pageRank = new PageRank(transitionMatrix, PageRank.DefaultDampingFactor, teleportationMatrix);
                var initialVector = Vector.e(VectorType.Row, matrixSize);
                pageRank.GetPageRankVector(initialVector, 10, out nbIterationsPr);
                //Calculate The Number Of Iteration Associated To APr Calcul
                ulong nbIterationsAPr = 0;
                var aPageRank = new PageRank(transitionMatrix, PageRank.DefaultDampingFactor, teleportationMatrix);
                aPageRank.GetAmelioratedPageRankVector(initialVector, 10, out nbIterationsAPr);
                _listPrIteMat.Add(new KeyValuePair<ulong, ulong>(matrixSize, nbIterationsPr));
                _listAPrIteMat.Add(new KeyValuePair<ulong, ulong>(matrixSize, nbIterationsAPr));
            }
         //   BusyIndicator = false;
        }
        private RelayCommand _startSimulationPrAprTimeCommand;
        public RelayCommand StartSimulationPrAprTimeCommand
        {
            get
            {
                return _startSimulationPrAprTimeCommand
                    ?? (_startSimulationPrAprTimeCommand = new RelayCommand(
                                          () =>
                                          {                                              
                                              var worker = new BackgroundWorker();
                                              worker.DoWork += DrawSimulationTimeMatChart;
                                              worker.RunWorkerCompleted += DrawSimulationTimeMatChartCompeleted;
                                              worker.RunWorkerAsync();

                                          }));
            }
        }
        private RelayCommand _resetChartsPrAprTimeCommand;
        public RelayCommand ResetChartsPrAprTimeCommand
        {
            get
            {
                return _resetChartsPrAprTimeCommand
                    ?? (_resetChartsPrAprTimeCommand = new RelayCommand(
                                          () =>
                                          {
                                              ListPrTimeMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
                                              ListAPrTimeMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
                                          }));
            }
        }        
        private void DrawSimulationTimeMatChartCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RaisePropertyChanged(ListPrTimeMatPropertyName);
            RaisePropertyChanged(ListAPrTimeMatPropertyName);
            
        }
        private void DrawSimulationTimeMatChart(object sender, DoWorkEventArgs e)
        {
       //     BusyIndicator = true;
            _listPrTimeMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
            _listAPrTimeMat = new ObservableCollection<KeyValuePair<ulong, ulong>>();
            for (ulong matrixSize = 5; matrixSize <= 50; matrixSize += 5)
            {
                //Generate A Random Matrix 
                var adjacentMatrix = new Matrix(matrixSize);
                var rand = new Random();
                for (ulong i = 0; i < matrixSize; i++)
                {
                    for (ulong j = 0; j < matrixSize; j++)
                    {
                        adjacentMatrix[i, j] = rand.Next(2);
                    }
                }
                var transitionMatrix = new Matrix(adjacentMatrix);
                transitionMatrix.ToProbablityMatrix();
                //Calculate The Number Of Iteration Associated To Pr Calcul 
                ulong nbIterationsPr = 0;
                var teleportationMatrix = Matrix.E(matrixSize);
                var pageRank = new PageRank(transitionMatrix, PageRank.DefaultDampingFactor, teleportationMatrix);
                var initialVector = Vector.e(VectorType.Row, matrixSize);
                var time = DateTime.Now;
                pageRank.GetPageRankVector(initialVector, 10, out nbIterationsPr);
                var timePr = (DateTime.Now - time).Milliseconds;

                //Calculate The Number Of Iteration Associated To APr Calcul
                ulong nbIterationsAPr = 0;
                var aPageRank = new PageRank(transitionMatrix, PageRank.DefaultDampingFactor, teleportationMatrix);

                time = DateTime.Now;
                aPageRank.GetAmelioratedPageRankVector(initialVector, 10, out nbIterationsAPr);
                var timeAPr = (DateTime.Now - time).Milliseconds;

                _listPrTimeMat.Add(new KeyValuePair<ulong, ulong>(matrixSize, (ulong)timePr));
                _listAPrTimeMat.Add(new KeyValuePair<ulong, ulong>(matrixSize, (ulong)timeAPr));
            }
       //     BusyIndicator = false;

        }

        private RelayCommand _newNodeCommand;  
        public RelayCommand NewNodeCommand
        {
            get
            {
                return _newNodeCommand
                    ?? (_newNodeCommand = new RelayCommand(
                                          () =>
                                          {                                              
                                              _matrixSize = MatrixSize + 1;
                                              ListWebPages.Add((MatrixSize - 1).ToString(CultureInfo.InvariantCulture));
                                              SelectedPage = ListWebPages.First();
                                              var newAdjMatrix = new Matrix(_matrixSize);
                                              InitialPageRank = Vector.e(VectorType.Row, _matrixSize);
                                              TelePortationMatrix = Matrix.E(_matrixSize);
                                              
                                              PageRankVector = null;
                                              AmelioratedPageRankVector = null;

                                              for (ulong i = 0; i < MatrixSize-1; i++)
                                              {
                                                  for (ulong j = 0; j < MatrixSize-1; j++)
                                                  {
                                                      newAdjMatrix[i, j] = AdjacenteMatrix[i, j];                                                     
                                                  }
                                              }
                                              AdjacenteMatrix = newAdjMatrix;
                                              WebGraph.AddVertex(new WebVertex()
                                              {
                                                  Label = (MatrixSize - 1).ToString(CultureInfo.InvariantCulture)
                                              });
                                              RaisePropertyChanged(AdjacenceMatrixPropertyName);
                                              RaisePropertyChanged(WebGraphPropertyName);
                                              ResetMatrices();                                           
                                          }));
            }
        }
        #endregion
    }
}