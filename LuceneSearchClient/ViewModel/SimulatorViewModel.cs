using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace LuceneSearchClient.ViewModel
{

    public class SimulatorViewModel : ViewModelBase
    {
        #region Consts
        public const string MatrixSizePropertyName = "MatrixSize";
        public const string TransitionMatrixPropertyName = "TransitionMatrix";
        public const string InitialPageRankPropertyName = "InitialPageRank";
        public const string DampingFactorPropertyName = "DampingFactor";
        public const string TransportationValuePropertyName = "TransportationValue";
        public const string NumberIterationsPropertyName = "NumberIterations";
        #endregion
        #region Fields
        private int _matrixSize;
        private short[,] _transitionMatrix;
        private float _dampingFactor;
        private float _transportationValue;
        private float _initialPageRank;
        private int _numberIterations;
        #endregion
        #region Properties
        public int MatrixSize
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
                TransitionMatrix = new short[_matrixSize, _matrixSize];
                TransportationValue = 1f/_matrixSize;
                InitialPageRank = 1f / _matrixSize;
                RaisePropertyChanged(MatrixSizePropertyName);
            }
        }
        public short[,] TransitionMatrix
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
        public float TransportationValue
        {
            get
            {
                return _transportationValue;
            }

            set
            {
                if (Equals(_transportationValue, value))
                {
                    return;
                }
                
                _transportationValue = value;
                RaisePropertyChanged(TransportationValuePropertyName);
            }
        }        
        public float InitialPageRank
        {
            get
            {
                return _initialPageRank;
            }

            set
            {
                if (Equals(_initialPageRank, value))
                {
                    return;
                }                
                _initialPageRank = value;
                RaisePropertyChanged(InitialPageRankPropertyName);
            }
        }           
        public int NumberIterations
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
        #endregion
        #region Ctors and Methods
        public SimulatorViewModel()
        {
            DampingFactor = 0.85f;
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
                                              var rand=new Random();
                                              for (int i = 0; i < MatrixSize; i++)
                                              {
                                                  for (int j = 0; j < MatrixSize; j++)
                                                  {
                                                      TransitionMatrix[i, j] = (short) rand.Next(2);
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
                                              TransitionMatrix = new short[_matrixSize, _matrixSize];
                                          }));
            }
        }
        #endregion
       
    }
}