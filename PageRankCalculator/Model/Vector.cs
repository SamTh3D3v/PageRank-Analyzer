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
        /// <summary>
        /// Gets the size of current Vector 
        /// </summary>
        public ulong Size
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Gets the current Vector's Type
        /// </summary>
        public VectorType VectorType
        {
            get
            {
                return _vectorType;
            }
        }

        #endregion

        #region Indexers
        /// <summary>
        /// Gets the Vector's element at the index index
        /// </summary>
        /// <param name="index">The index at which the element will be retrieved</param>
        /// <returns></returns>
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
        /// Initialize a new instance of the <b>Vector</b> class using the Vector Type and an array of floats
        /// </summary>
        /// <param name="vectorType">The vector type as a row or a column vector </param>
        /// <param name="data">The data which the Vector will contain</param>
        public Vector(VectorType vectorType, float[] data)
        {
            _data = new float[_size = (ulong)data.Length];
            for (ulong i = 0; i < _size; i++)
            {
                _data[i] = data[i];
            }
            _vectorType = vectorType;
        }

        /// <summary>
        /// Initialize a new instance of the <b>Vector</b> class using the Vector type and the Size of the Vector
        /// </summary>
        /// <param name="vectorType">The suggested vector type as a row or a column vector </param>
        /// <param name="size">The suggested size of the Vector</param>
        public Vector(VectorType vectorType, ulong size)
        {
            _data = new float[_size = size];
            _vectorType = vectorType;
        }

        /// <summary>
        /// Initialize a new instance of the <b>Vector</b> class using the Vector type and an existing Vector
        /// </summary>
        /// <param name="vectorType">The suggested vector type as a row or a column vector </param>
        /// <param name="vector">The suggested Vector from which to create another Vector</param>
        public Vector(VectorType vectorType, Vector vector)
        {
            _data = new float[_size = vector.Size];
            for (ulong i = 0; i < _size; i++)
            {
                _data[i] = vector[i];
            }
            _vectorType = vectorType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a 1/Size all Vector, using a Vector type and a specified Vector size
        /// </summary>
        /// <param name="vectorType">The suggested VectorType</param>
        /// <param name="size">The suggested size of the Vector</param>
        /// <returns>A vector with 1/Size every where</returns>
        public static Vector e(VectorType vectorType, ulong size)
        {
            var data = new float[size];
            for (ulong i = 0; i < size; i++)
            {
                data[i] = (float)1 / size;
            }
            var vectorE = new Vector(vectorType, data);
            return vectorE;
        }

        /// <summary>
        /// Calculates the sum of all the current Vector's elements
        /// </summary>
        /// <returns>The sum of all the element as a float</returns>
        public float Sum()
        {
            return _data.Sum();
        }

        /// <summary>
        /// Checks whether the current Vector is Stochastic or not
        /// </summary>
        /// <returns>True if stochastic, false otherwise</returns>
        public bool CheckIfStochasitc()
        {
            return Math.Abs(Sum() - 1) < float.Epsilon;
        }

        /// <summary>
        /// Normalizes the current vector
        /// </summary>
        public void Normalize()
        {
            var sum = Sum();
            for (ulong i = 0; i < _size; i++)
            {
                _data[i] /= sum;
            }
        }

        public IEnumerable<float> Where(Func<float, bool> predicate)
        {
            return _data.Where(predicate);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Additionning tow Vectors of the same size and same type
        /// </summary>
        /// <param name="x">The suggested left Vector</param>
        /// <param name="y">The suggested right Vector</param>
        /// <exception cref="ArithmeticException">Throw wheb the vectors have different sizes</exception>
        /// <exception cref="InvalidOperationException">Thrown when the vectors are of different type</exception>
        /// <returns>A vector of the same type and the same size</returns>
        public static Vector operator +(Vector x, Vector y)
        {
            if (x.VectorType != y.VectorType)
            {
                throw new InvalidOperationException("Both vectors must be of the same type");
            }
            if (x.Size != y.Size)
            {
                throw new ArithmeticException("Both vectors must have the same size");
            }
            var additionResultVector = new Vector(x.VectorType, x.Size);
            for (ulong i = 0; i < x.Size; i++)
            {
                additionResultVector[i] = x[i] + y[i];
            }
            return additionResultVector;
        }

        /// <summary>
        /// Multiply two Vectors of different types and of the same Size
        /// </summary>
        /// <param name="x">The suggested left Vector</param>
        /// <param name="y">The suggested right Vector</param>
        /// <exception cref="InvalidOperationException">Thrown when the vectors are of the same type</exception>
        /// <exception cref="ArithmeticException">Thrown when the vectors have different sizes</exception>
        /// <returns>A square matrix of the same size, or of a Size = 1</returns>
        public static Matrix operator *(Vector x, Vector y)
        {
            Matrix multiplicationResultMatrix = null;
            if (x.VectorType == y.VectorType)
            {
                throw new InvalidOperationException("Both vectors must be of the same type");
            }
            if (x.Size != y.Size)
            {
                throw new ArithmeticException("Both vectors must have the same size");
            }
            if ((x.VectorType == VectorType.Row) && (y.VectorType == VectorType.Column))
            {
                multiplicationResultMatrix = new Matrix(1);
                for (ulong i = 0; i < x.Size; i++)
                {
                    multiplicationResultMatrix[(ulong)0, 0] += x[i] * y[i];
                }
            }
            else
            {
                if ((x.VectorType == VectorType.Column) && (y.VectorType == VectorType.Row))
                {
                    multiplicationResultMatrix = new Matrix(x.Size);
                    for (ulong j = 0; j < x.Size; j++)
                    {
                        for (ulong i = 0; i < x.Size; i++)
                        {
                            multiplicationResultMatrix[i, j] += x[i] * y[j];
                        }
                    }
                }
            }

            return multiplicationResultMatrix;
        }

        /// <summary>
        /// Multiply a vector by a scalar of type float
        /// </summary>
        /// <param name="x">The suggested scalar</param>
        /// <param name="y">The suggested Vector</param>
        /// <returns></returns>
        public static Vector operator *(float x, Vector y)
        {
            var multiplicationResultVector = new Vector(y.VectorType, y.Size);
            for (ulong i = 0; i < y.Size; i++)
            {
                multiplicationResultVector[i] = y[i] * x;
            }
            return multiplicationResultVector;
        }

        /// <summary>
        /// Compares two vectors and check whether are equal
        /// </summary>
        /// <param name="x">The suggested left Vector</param>
        /// <param name="y">The suggested right Vector</param>
        /// <returns>true if equal, false otherwise</returns>
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
                if (x.Size != y.Size) isEqual = false;
                else
                {
                    if (x.Size == y.Size)
                    {
                        for (ulong i = 0; i < x.Size; i++)
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

        /// <summary>
        /// Compares two vectors and check whether are not equal
        /// </summary>
        /// <param name="x">The suggested left Vector</param>
        /// <param name="y">The suggested right Vector</param>
        /// <returns>true if equal, false otherwise</returns>
        public static bool operator !=(Vector x, Vector y)
        {
            return !(x == y);
        }

        #endregion

        #region Fields

        /// <summary>
        /// The size of the current Vector
        /// </summary>
        private ulong _size;

        /// <summary>
        /// The type of the current Vector
        /// </summary>
        private readonly VectorType _vectorType;

        /// <summary>
        /// The data contained within the Vector
        /// </summary>
        private float[] _data;

        #endregion
    }

    /// <summary>
    /// Represents the type of the Vector, can be row or column
    /// </summary>
    public enum VectorType
    {
        Row, Column
    }
}
