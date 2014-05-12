using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PageRankCalculator.Model
{
    public class Vector
    {
        #region Properties
        public ulong Length
        {
            get
            {
                return _length;
            }
        }
        public VectorType VectorType
        {
            get
            {
                return _vectorType;
            }
        }

        #endregion

        #region Indexers
        public float this[ulong index]
        {
            get
            {
                return _data[index];
            }
            set
            {
                _data[index] = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <b>Vector</b> class using a Length and a Vector Type
        /// </summary>
        /// <param name="vectorType">The vector type as a row or a column vector </param>
        /// <param name="data"></param>
        public Vector(VectorType vectorType, float[] data)
        {
            _data = new float[_length = (ulong)data.Length];
            for (ulong i = 0; i < _length; i++)
            {
                _data[i] = data[i];
            }
            _vectorType = vectorType;
        }

        public Vector(VectorType vectorType, ulong length)
        {
            _data = new float[_length = length];
            _vectorType = vectorType;
        }

        public Vector(VectorType vectorType, Vector vector)
        {
            _data = new float[_length = vector.Length];
            for (ulong i = 0; i < _length; i++)
            {
                _data[i] = vector[i];
            }
            _vectorType = vectorType;
        }

        #endregion

        #region Methods
            //Unity Matrix, this vector has 1/N everywhere
            public static Vector e(VectorType vectorType, ulong length)
            {
                var data = new float[length];
                for (ulong i = 0; i < length; i++)
                {
                    data[i] = (float)1 / length;
                }
                var vectorE = new Vector(vectorType, data); 
                return vectorE;
            }

            public float Sum()
            {
                return _data.Sum();
            }

            public bool CheckIfSochasitc()
            {
                return Math.Abs(Sum() - 1) < float.Epsilon;
            }

            public void Normalize()
            {
                var sum = Sum();
                for (ulong i = 0; i < _length; i++)
                {
                    _data[i] /= sum;
                }
            }
          
            public IEnumerable<float> Where(Func<float,bool> predicate)
            {
                return _data.Where(predicate);
            }

        #endregion

        #region Operators
            public static Vector operator +(Vector x, Vector y)
            {
                if (x.VectorType != y.VectorType)
                {
                    throw new ArithmeticException("Both vectors must be of the same type");   
                }
                if (x.Length != y.Length)
                {
                    throw new ArithmeticException("Both vectors must have the same length");                        
                }
                var additionResultVector = new Vector(x.VectorType,x.Length);
                for (ulong i = 0; i < x.Length; i++)
                {
                    additionResultVector[i] = x[i] + y[i];
                } 
                return additionResultVector;
            }

            public static Matrix operator *(Vector x, Vector y)
            {
                Matrix multiplicationResultMatrix = null;
                if (x.VectorType == y.VectorType)
                {
                    throw new ArithmeticException("Both vectors must be of the same type");
                }
                if (x.Length != y.Length)
                {
                    throw new ArithmeticException("Both vectors must have the same length");
                }
                if ((x.VectorType == VectorType.Row) && (y.VectorType == VectorType.Column))
                {
                    multiplicationResultMatrix = new Matrix(1);
                    for (ulong i = 0; i < x.Length; i++)
                    {
                        multiplicationResultMatrix[(ulong)0,0] += x[i] * y[i];
                    }
                }
                else
                {
                    if ((x.VectorType == VectorType.Column) && (y.VectorType == VectorType.Row))
                    {
                        multiplicationResultMatrix = new Matrix(x.Length);
                        for (ulong j = 0; j < x.Length; j++)
                        {
                            for (ulong i = 0; i < x.Length; i++)
                            {
                                multiplicationResultMatrix[i, j] += x[i] * y[j];
                            }
                        }
                    }
                }
               
                return multiplicationResultMatrix;
            }

            public static Vector operator *(float x, Vector y)
            {
                var multiplicationResultVector = new Vector(y.VectorType,y.Length);
                for (ulong i = 0; i < y.Length; i++)
                {
                    multiplicationResultVector[i] = y[i]*x;
                }
                return multiplicationResultVector;
            }

            public static bool operator ==(Vector x, Vector y)
            {
                bool isEqual = true;
                var xo = x as object;
                var yo = y as object;
                if (xo == null && yo == null)
                {
                    return true;
                }
                else if (xo == null || yo == null)
                {
                    return false;   
                }
                if (x.VectorType != y.VectorType) isEqual = false;
                else
                {
                    if (x.Length != y.Length) isEqual = false;
                    else
                    {
                        if (x.Length == y.Length)
                        {
                            for (ulong i = 0; i < x.Length; i++)
                            {
                                if (Math.Abs(x[i] - y[i]) > float.Epsilon)
                                {
                                    isEqual = false;                                  
                                    break;
                                }
                            }
                        }
                    }
                }
                return isEqual;
            }

            public static bool operator !=(Vector x, Vector y)
            {
                return ! (x == y);
            }

        #endregion

        #region Fields

            private  ulong _length;
            private readonly VectorType _vectorType;
            private float[] _data;

        #endregion
    }

    public enum VectorType
    {
        Row,Column        
    }
}
