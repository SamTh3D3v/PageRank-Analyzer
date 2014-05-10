using GalaSoft.MvvmLight;

namespace LuceneSearchClient.ViewModel
{

    public class SimulatorViewModel : ViewModelBase
    {
        #region Consts
        public const string MatrixSizePropertyName = "MatrixSize";
        public const string TransitionMatrixPropertyName = "TransitionMatrix";
        #endregion
        #region Fields
        private int _matrixSize;
        private short[,] _transitionMatrix;
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

                RaisePropertyChanging(TransitionMatrixPropertyName);
                _transitionMatrix = value;
                RaisePropertyChanged(TransitionMatrixPropertyName);
            }
        }
        #endregion
        #region Ctors and Methods
        public SimulatorViewModel()
        {
        }
        #endregion
        #region Commands

        #endregion
       
    }
}