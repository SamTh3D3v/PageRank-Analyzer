using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;


namespace PageRankCalculator.Model
{
    public class Matrix
    {
        #region Properties
        public ulong Length
        {
            get
            {
                return _length;
            }
        }

        #endregion

        #region Indexers

        public Vector this[VectorType vectorType, ulong index]
        {
            get
            {
                var vector = new Vector(vectorType,_length);
                if (vectorType == VectorType.Column)
	            {
                    for (ulong i = 0; i < _length; i++)
                    {
                        vector[i] = _data[i, index];
                    }	 
	            }
                else if (vectorType == VectorType.Row)
                {
                    for (ulong j = 0; j < _length; j++)
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
                    for (ulong i = 0; i < _length; i++)
                    {
                        _data[i, index] = value[i];
                    }
                }
                else if (vectorType == VectorType.Row)
                {
                    for (ulong j = 0; j < _length; j++)
                    {
                        _data[index,j] = value[j];
                    }
                }
            }
        }

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

        public Matrix(ulong length)
        {
            _length = length;
            _data = new float[_length, _length];
        }

        public Matrix(Matrix matrix)
        {
            _length = matrix.Length;
            _data = new float[_length, _length];
            for (ulong i = 0; i < _length ; i++)
            {
                for (ulong j = 0; j < _length; j++)
                {
                    _data[i,j] = matrix[i, j];
                }
            }
        }

        public Matrix(float[,] data)
        {
            _length = (ulong)data.Length;
            _data = new float[_length, _length];
            for (ulong i = 0; i < _length; i++)
            {
                for (ulong j = 0; j < _length; j++)
                {
                    _data[i, j] = data[i, j];
                }
            }
        }

        #endregion

        #region Operators

        public static Matrix operator +(Matrix x, Matrix y)
        {
            Matrix additionResultMatrix = null;
            if (x.Length == y.Length)
            {
                additionResultMatrix = new Matrix(x.Length);
                for (ulong i = 0; i < x.Length; i++)
                {
                    for (ulong j = 0; j < x.Length; j++)
                    {
                        additionResultMatrix[i, j] = x[i, j] + y[i, j];
                    }    
                }
            }
            else
            {
                throw new ArithmeticException("Unhandled Arithmetic Exception, Additionning two matrices of different length is not allowed");
            }
            return additionResultMatrix;
        }

        public static Matrix operator *(Matrix x, Matrix y)
        {
            Matrix multiplicationResultMatrix = null;
            if (x.Length == y.Length)
            {
                multiplicationResultMatrix = new Matrix(x.Length);

                Parallel.For((long)0, (long)x.Length, i =>
                {
                    for (ulong j = 0; j < x.Length; j++)
                    {
                        for (ulong k = 0; k < x.Length; k++)
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
                throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplying two matrices of different length is not allowed");
            }
            return multiplicationResultMatrix;
        }

        public static Vector operator *(Vector x, Matrix y)
        {
            Vector multiplicationResultVector = null;
            if (x.VectorType == VectorType.Row)
            {
                if (x.Length == y.Length)
                {
                    multiplicationResultVector = new Vector(VectorType.Row, x.Length);

                    Parallel.For((long)0, (long)x.Length, j =>
                            {
                                for (ulong i = 0; i < x.Length; i++)
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
                    throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplication of a vector and matrix of different length is not allowed");                    
                }
            }
            else
            {
                throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplication not allowed");
            }
            return multiplicationResultVector;
        }

        public static Vector operator *(Matrix y, Vector x)
        {
            Vector multiplicationResultVector = null;
            if (x.VectorType == VectorType.Row)
            {
                if (x.Length == y.Length)
                {
                    multiplicationResultVector = new Vector(VectorType.Row, x.Length);
                    for (ulong i = 0; i < x.Length; i++)
                    {
                        for (ulong j = 0; j < x.Length; j++)
                        {
                            multiplicationResultVector[i] += y[i, j] *x[j];
                        }
                    }
                }
                else
                {
                    throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplication of a vector and matrix of different length is not allowed");
                }
            }
            else
            {
                throw new ArithmeticException("Unhandled Arithmetic Exception, Multiplication not allowed");
            }
            return multiplicationResultVector;
        }

        public static Matrix operator *(float x, Matrix y)
        {
            var multiplicationResultMatrix = new Matrix(y.Length);
                for (ulong i = 0; i < y.Length; i++)
                {
                    for (ulong j = 0; j < y.Length; j++)
                    {
                        multiplicationResultMatrix[i, j] = y[i, j] * x;
                    }
                }
            return multiplicationResultMatrix;
        }

        public static Matrix operator ^(Matrix x,ulong power)
        {
            Matrix powerResultMatrix = null;
            if (power==0)
            {
                powerResultMatrix = Matrix.I(x.Length);
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
            if (x.Length != y.Length) isEqual = false;
            else
            {
                if (x.Length == y.Length)
                {
                    for (ulong i = 0; i < x.Length; i++)
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

        public static bool operator !=(Matrix x, Matrix y)
        {
            return !(x == y);
        }

        public static bool operator >(Matrix x, float y)
        {
            bool isGreater =true;
            for (ulong i = 0; i < x.Length; i++)
            {
                for (ulong j = 0; j < x.Length; j++)
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

        public static bool operator >=(Matrix x, float y)
        {            
            for (ulong i = 0; i < x.Length; i++)
            {
                for (ulong j = 0; j < x.Length; j++)
                {
                    if (x[i, j] < y)
                    {                        
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool operator <(Matrix x, float y)
        {
            return !(x >= y);
        }

        public static bool operator <=(Matrix x, float y)
        {
            return !(x > y);
        }

        #endregion

        #region Method

        //Unity Matrix, this matrix has 1 when i==j, 0 else
        public static Matrix I(ulong length)
        {
            var unityMatrix = new Matrix(length);
            for (ulong i = 0; i < length; i++)
            {
                for (ulong j = 0; j < length; j++)
                {
                    if (i == j) unityMatrix[i, j] = 1;
                }
            }
            return unityMatrix;
        }
        //Unity Matrix, this matrix has 1/N everywhere
        public static Matrix E(ulong length)
        {
            var matrixE = new Matrix(length);
            for (ulong i = 0; i < length; i++)
            {
                for (ulong j = 0; j < length; j++)
                {
                    matrixE[i, j] = 1f/length;
                }                
            }
            return matrixE;
        }
        public float SumRow(ulong iIndex)
            {
                return this[VectorType.Row, iIndex].Sum();
            }
        public bool IsSochasitc()
            {
                bool isStochastic = true;
                for (ulong i = 0; i < _length; i++)
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
        public void ToProbablityMatrix()
            {
                for (ulong i = 0; i < _length; i++)
                {
                    var rowSum = SumRow(i);
                    for (ulong j = 0; (j < _length); j++)
                    {
                        if ((Math.Abs(_data[i, j] - 1) < float.Epsilon)) _data[i, j] /= rowSum;   
                    }
                }
            }
        public bool IsIrreducible()
        {
            bool isIrreducible = true;
            Matrix poweredMatrix = this;
            for (ulong i = 1; i < Length-1; i++)
            {
                if ((this^i)<=0)
                {
                    isIrreducible = false;
                    break;
                }                
            }
            return isIrreducible;
        }
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
        public List<float> Eigenvalues()
        {
            DenseMatrix m = DenseMatrix.OfArray(_data);
            var eigenValues = m.Evd().EigenValues().ToList();
            List<float> sortedEigenvalues = (from eigenValue in eigenValues
                                    orderby eigenValue.Magnitude
                                    select (float)eigenValue.Magnitude).ToList();
            return sortedEigenvalues;
        }

        #endregion

        #region Fields

            private const float Epsilon = 0.0009f;
            private readonly ulong _length;
            private float[,] _data;

        #endregion
    }
}
