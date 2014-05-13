using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;


namespace PageRankCalculator.Model
{
    public class Matrix
    {
        #region Properties

        /// <summary>
        /// Gets the size of the current instance of the Matrix
        /// </summary>
        public ulong Size
        {
            get
            {
                return _size;
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Gets the row or the column Vector at the specified row or column index resp.
        /// </summary>
        /// <param name="vectorType">The suggested Vector Type to be retrieved</param>
        /// <param name="index">The row or column index at which The desired Vector will be retrieved</param>
        /// <returns>Object of type Vector</returns>
        public Vector this[VectorType vectorType, ulong index]
        {
            get
            {
                var vector = new Vector(vectorType,_size);
                if (vectorType == VectorType.Column)
	            {
                    for (ulong i = 0; i < _size; i++)
                    {
                        vector[i] = _data[i, index];
                    }	 
	            }
                else if (vectorType == VectorType.Row)
                {
                    for (ulong j = 0; j < _size; j++)
                    {
                        vector[j] = _data[index, j];
                    }           
                }
                return vector;     
            }
            set
            {
                if (vectorType == VectorType.Column)
                {
                    for (ulong i = 0; i < _size; i++)
                    {
                        _data[i, index] = value[i];
                    }
                }
                else if (vectorType == VectorType.Row)
                {
                    for (ulong j = 0; j < _size; j++)
                    {
                        _data[index,j] = value[j];
                    }
                }
            }
        }

        /// <summary>
        /// Gets the element at the specified indexes
        /// </summary>
        /// <param name="iIndex">The suggested row index</param>
        /// <param name="jIndex">The specified column index</param>
        /// <returns>Object of type float</returns>
        public float this[ulong iIndex, ulong jIndex]
        {
            get
            {
                return _data[iIndex, jIndex];
            }
            set
            {
                _data[iIndex, jIndex]=value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of the <b>Matrix</b> Class using the specified size
        /// </summary>
        /// <param name="size">The suggested size to initialize the Matrix</param>
        public Matrix(ulong size)
        {
            _size = size;
            _data = new float[_size, _size];
        }
        
        /// <summary>
        /// Initialize a new instance of the <b>Matrix</b> Class using a existing Matrix object
        /// </summary>
        /// <param name="matrix">The suggested Matrix object used to instanciate a new Matrix</param>
        public Matrix(Matrix matrix)
        {
            _size = matrix.Size;
            _data = new float[_size, _size];
            for (ulong i = 0; i < _size ; i++)
            {
                for (ulong j = 0; j < _size; j++)
                {
                    _data[i,j] = matrix[i, j];
                }
            }
        }

        /// <summary>
        /// Initialize a new instance of the <b>Matrix</b> Class using an array of floats
        /// </summary>
        /// <param name="data">The suggested array used to initialize the Matrix</param>
        public Matrix(float[,] data)
        {
            _size = (ulong)data.Length;
            _data = new float[_size, _size];
            for (ulong i = 0; i < _size; i++)
            {
                for (ulong j = 0; j < _size; j++)
                {
                    _data[i, j] = data[i, j];
                }
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Additioning two Square matrices of the same size
        /// </summary>
        /// <param name="x">The left Matrix</param>
        /// <param name="y">The right Matrix</param>
        /// <returns>The addition result, a square Matrix of the same size</returns>
        /// <exception cref="System.ArithmeticException">Thrown when the suggested matrices are of different size</exception>
        public static Matrix operator +(Matrix x, Matrix y)
        {
            Matrix additionResultMatrix = null;
            if (x.Size == y.Size)
            {
                additionResultMatrix = new Matrix(x.Size);
                for (ulong i = 0; i < x.Size; i++)
                {
                    for (ulong j = 0; j < x.Size; j++)
                    {
                        additionResultMatrix[i, j] = x[i, j] + y[i, j];
                    }    
                }
            }
            else
            {
                throw new ArithmeticException("Unhandled Arithmetic Exception, Additionning two matrices of different size is not allowed");
            }
            return additionResultMatrix;
        }

        /// <summary>
        /// Multplicating two Square matrices of the same size
        /// </summary>
        /// <param name="x">The left Matrix</param>
        /// <param name="y">The right Matrix</param>
        /// <returns>The multiplication result, a square Matrix of the same size</returns>
        /// <exception cref="System.ArithmeticException">Thrown when the suggested matrices are of different size</exception>
        public static Matrix operator *(Matrix x, Matrix y)
        {
            Matrix multiplicationResultMatrix = null;
            if (x.Size == y.Size)
            {
                multiplicationResultMatrix = new Matrix(x.Size);

                Parallel.For((long)0, (long)x.Size, i =>
                {
                    for (ulong j = 0; j < x.Size; j++)
                    {
                        for (ulong k = 0; k < x.Size; k++)
                        {
                            if ((Math.Abs(x[(ulong)i, k]) < float.Epsilon) || (Math.Abs(y[k, j]) < float.Epsilon))
                            {
                                multiplicationResultMatrix[(ulong) i, j] += 0;
                            }
                            else
                            {
                                multiplicationResultMatrix[(ulong)i, j] += x[(ulong)i, k] * y[k, j];   
                            }
                        }
                    }
                });
            }
            else
            {
                throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplying two matrices of different size is not allowed");
            }
            return multiplicationResultMatrix;
        }

        /// <summary>
        /// Multiplicating a row Vector and a square Matrix of the same size
        /// </summary>
        /// <param name="x">The suggest row Vector</param>
        /// <param name="y">The Suggest square Matrix</param>
        /// <returns>The muliplication result, a row Vector of the same size</returns>
        /// <exception cref="System.ArithmeticException">Thrown when the suggested Vector and Matrix are of different size</exception>
        /// <exception cref="InvalidOperationException">Thrown when the suggested Vector is not a row vector </exception>
        public static Vector operator *(Vector x, Matrix y)
        {
            Vector multiplicationResultVector = null;
            if (x.VectorType == VectorType.Row)
            {
                if (x.Size == y.Size)
                {
                    multiplicationResultVector = new Vector(VectorType.Row, x.Size);

                    Parallel.For((long)0, (long)x.Size, j =>
                            {
                                for (ulong i = 0; i < x.Size; i++)
                                {
                                    if ((Math.Abs(x[i]) < float.Epsilon) || (Math.Abs(y[i, (ulong)j]) < float.Epsilon))
                                    {
                                        multiplicationResultVector[(ulong)j] += 0;
                                    }
                                    else
                                    {
                                        multiplicationResultVector[(ulong)j] += x[i] * y[i, (ulong)j];
                                    }
                                }
                            });
                }
                else
                {
                    throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplication of a vector and matrix of different size is not allowed");                    
                }
            }
            else
            {
                throw new InvalidOperationException("Unhandled Arithmetic Exception, Multiplication not allowed");
            }
            return multiplicationResultVector;
        }

        /// <summary>
        /// Multiplicating a square Matrix and a column Vector of the same size
        /// </summary>
        /// <param name="x">The suggest square Matrix</param>
        /// <param name="y">The Suggest column Vector</param>
        /// <returns>The muliplication result, a column Vector of the same size</returns>
        /// <exception cref="System.ArithmeticException">Thrown when the suggested matrix and vector are of different size</exception>
        /// <exception cref="InvalidOperationException">Thrown when the suggested Vector is not a column vector </exception>
        public static Vector operator *(Matrix y, Vector x)
        {
            Vector multiplicationResultVector = null;
            if (x.VectorType != VectorType.Column)
            {
                throw new InvalidOperationException("Unhandled Arithmetic Exception, Multiplication not allowed");                    
            }
            if (x.Size != y.Size)
            {
                throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplication of a vector and matrix of different size is not allowed");   
            }
            multiplicationResultVector = new Vector(VectorType.Column, x.Size);
            for (ulong i = 0; i < x.Size; i++)
            {
                for (ulong j = 0; j < x.Size; j++)
                {
                    multiplicationResultVector[i] += y[i, j] * x[j];
                }
            }
            return multiplicationResultVector;
        }

        /// <summary>
        /// Multiplicating a float number and a square Matrix
        /// </summary>
        /// <param name="x">The suggest float</param>
        /// <param name="y">The Suggest square Matrix</param>
        /// <returns>The muliplication result, a square Matrix of the same size</returns>
        public static Matrix operator *(float x, Matrix y)
        {
            var multiplicationResultMatrix = new Matrix(y.Size);
                for (ulong i = 0; i < y.Size; i++)
                {
                    for (ulong j = 0; j < y.Size; j++)
                    {
                        multiplicationResultMatrix[i, j] = y[i, j] * x;
                    }
                }
            return multiplicationResultMatrix;
        }

        /// <summary>
        /// Calculates a square matrix raised to the power of power
        /// </summary>
        /// <param name="x">The suggested square Matrix</param>
        /// <param name="power">The suggested power number</param>
        /// <returns>A square matrix of the same size</returns>
        public static Matrix operator ^(Matrix x,ulong power)
        {
            Matrix powerResultMatrix = null;
            if (power==0)
            {
                powerResultMatrix = Matrix.I(x.Size);
            }
            else
            {
                powerResultMatrix = x;
                for (ulong i = 1; i < power; i++)
                {
                    powerResultMatrix *= x;
                }   
            }

            return powerResultMatrix;
        }

        /// <summary>
        /// Compares two square matrices of the same size
        /// </summary>
        /// <param name="x">The suggest left matrix</param>
        /// <param name="y">The suggest right matrix</param>
        /// <returns>true if equals, false otherwise</returns>
        public static bool operator ==(Matrix x, Matrix y)
        {
            bool isEqual = true;
            var xo = x as object;
            var yo = y as object;
            if (xo==null && yo==null)
            {
                return true;
            }else if (xo ==null || yo==null)
            {
                return false;
            }
            if (x.Size != y.Size) isEqual = false;
            else
            {
                if (x.Size == y.Size)
                {
                    for (ulong i = 0; i < x.Size; i++)
                    {
                        if (x[VectorType.Row, i] != y[VectorType.Row, i])
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }
            }
            return isEqual;            
        }

        /// <summary>
        /// Compares two square matrices of the same size
        /// </summary>
        /// <param name="x">The suggest left matrix</param>
        /// <param name="y">The suggest right matrix</param>
        /// <returns>true if not equal, false otherwise</returns>
        public static bool operator !=(Matrix x, Matrix y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Compares two square matrices of the same size
        /// </summary>
        /// <param name="x">The suggest left matrix</param>
        /// <param name="y">The suggest right matrix</param>
        /// <returns>true if x is greater than y, false otherwise</returns>
        public static bool operator >(Matrix x, float y)
        {
            bool isGreater =true;
            for (ulong i = 0; i < x.Size; i++)
            {
                for (ulong j = 0; j < x.Size; j++)
                {
                    if (x[i,j]<=y)
                    {
                        isGreater = false;
                        break;
                    }                    
                }                
            }
            return isGreater;
        }

        /// <summary>
        /// Compares two square matrices of the same size
        /// </summary>
        /// <param name="x">The suggest left matrix</param>
        /// <param name="y">The suggest right matrix</param>
        /// <returns>true if x is greater than or equals y, false otherwise</returns>
        public static bool operator >=(Matrix x, float y)
        {            
            for (ulong i = 0; i < x.Size; i++)
            {
                for (ulong j = 0; j < x.Size; j++)
                {
                    if (x[i, j] < y)
                    {                        
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Compares two square matrices of the same size
        /// </summary>
        /// <param name="x">The suggest left matrix</param>
        /// <param name="y">The suggest right matrix</param>
        /// <returns>true if x is less than y, false otherwise</returns>
        public static bool operator <(Matrix x, float y)
        {
            return !(x >= y);
        }

        /// <summary>
        /// Compares two square matrices of the same size
        /// </summary>
        /// <param name="x">The suggest left matrix</param>
        /// <param name="y">The suggest right matrix</param>
        /// <returns>true if x is less than or equals y, false otherwise</returns>
        public static bool operator <=(Matrix x, float y)
        {
            return !(x > y);
        }

        #endregion

        #region Method

        /// <summary>
        /// Gets the unity matrix, a diagonal matrix with the value 1 on the diagonal
        /// </summary>
        /// <param name="size">The suggest size of the matrix</param>
        /// <returns>A square unity matrix</returns>
        public static Matrix I(ulong size)
        {
            var unityMatrix = new Matrix(size);
            for (ulong i = 0; i < size; i++)
            {
                for (ulong j = 0; j < size; j++)
                {
                    if (i == j) unityMatrix[i, j] = 1;
                }
            }
            return unityMatrix;
        }

        /// <summary>
        /// Gets a sparse matrix with the value 1/Size every where
        /// </summary>
        /// <param name="size">The suggest size of the matrix</param>
        /// <returns>A 1/Size all suqare matrix</returns>
        public static Matrix E(ulong size)
        {
            var matrixE = new Matrix(size);
            for (ulong i = 0; i < size; i++)
            {
                for (ulong j = 0; j < size; j++)
                {
                    matrixE[i, j] = 1f/size;
                }                
            }
            return matrixE;
        }
        public float SumRow(ulong iIndex)
            {
                return this[VectorType.Row, iIndex].Sum();
            }

        /// <summary>
        /// Checks whether the current instance of the class Matrix is stochastic 
        /// </summary>
        /// <returns>True if stochastic, fase otherwise</returns>
        public bool IsSochasitc()
            {
                bool isStochastic = true;
                for (ulong i = 0; i < _size; i++)
                {
                    Debug.WriteLine(SumRow(i));
                    if (Math.Abs(SumRow(i) - 1) > Epsilon)
                    {
                        isStochastic = false;
                        break;
                    }
                }
                return isStochastic;
            }

        /// <summary>
        /// Transforms the current instance of the Matrix class to a probability matrix
        /// </summary>
        public void ToProbablityMatrix()
            {
                for (ulong i = 0; i < _size; i++)
                {
                    var rowSum = SumRow(i);
                    for (ulong j = 0; (j < _size); j++)
                    {
                        if ((Math.Abs(_data[i, j] - 1) < float.Epsilon)) _data[i, j] /= rowSum;   
                    }
                }
            }
        
        /// <summary>
        /// Checks whether the current instance of the Matrix class is irreducible 
        /// </summary>
        /// <returns>True if irreducible, false otherwise</returns>
        public bool IsIrreducible()
        {
            bool isIrreducible = true;
            Matrix poweredMatrix = this;
            for (ulong i = 1; i < Size-1; i++)
            {
                if ((this^i)<=0)
                {
                    isIrreducible = false;
                    break;
                }                
            }
            return isIrreducible;
        }

        /// <summary>
        /// Checks whether the current instance of the Matrix class is perodic
        /// This method is limitted to 100 itterations
        /// </summary>
        /// <param name="period">returns the period of the matrix, 0 if aperiodic</param>
        /// <returns>True if periodic, false otherwise</returns>
        public bool IsPeriodic(out ulong period)
        {
            bool isPeriodic = false;
            Matrix powerMatrix = this;
            ulong power = 1;
            do
            {
                isPeriodic = ((powerMatrix *= this) == this);
                //Max itterations supporterd
                if (++power == 100) break;

            } while (!isPeriodic);
            if (!isPeriodic) power = 1;
            period = power - 1;
            return isPeriodic;
        }
        
        /// <summary>
        /// Gets the list of Eigenvalues of the current instance of Matrix class
        /// </summary>
        /// <returns>List of float objects representing the magnitude of the complex eigenvalues</returns>
        public List<float> Eigenvalues()
        {
            DenseMatrix m = DenseMatrix.OfArray(_data);
            var eigenValues = m.Evd().EigenValues().ToList();
            List<float> sortedEigenvalues = (from eigenValue in eigenValues
                                    orderby eigenValue.Magnitude descending 
                                    select (float)eigenValue.Magnitude).ToList();
            return sortedEigenvalues;
        }

        #endregion

        #region Fields

        private const float Epsilon = 0.0009f;
        /// <summary>
        /// The size of the current instance of the Matrix class
        /// </summary>
        private readonly ulong _size;
        /// <summary>
        /// The data contained within the current instance of the Matrix class
        /// </summary>
        private float[,] _data;

        #endregion
    }
}
