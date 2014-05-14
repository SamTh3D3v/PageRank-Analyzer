using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PageRankCalculator.Model;
using WebGraphMaker.Model;

namespace PageRankCalculator.BusinessModel
{
    public class PageRank
    {
        #region consts
        /// <summary>
        /// The Defualt value of the Google Dmaping Factor used to calculate the Google Matrix
        /// </summary>
        public const float DefaultDampingFactor = 0.85f;
        private const float Epsilon = 0.0001f;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the value of the Damping Factor d : factor used to calculate the Google matrix, amortize between the transition matrix and the teleportation matrix
            /// </summary>
            /// <value>Value less or equal than 1, greater or equal than 0</value>
            public float DampingFactor
        {
            get
            {
                return _dampinFactor;
            }
        }

            /// <summary>
        /// Gets the value of the Teleportation matrix : matrix used to calculate the Google matrix, eliminates reducibility and periodicity of the Transition matrix
            /// </summary>
            public Matrix TeleportationMatrix
        {
            get
            {
                return _teleportationMatrix;
            }
        }
            
            /// <summary>
        /// Gets the value of the Transition matrix, represents transition probabilities from web graph pages
            /// </summary>
            public Matrix TransitionMatrix
        {
            get
            {
                return _transitionMatrix;
            } 
        }
        #endregion

        #region Constructors

            /// <summary>
            /// Initialize a new instance of the <b>PageRank</b> Class, using the Transition matrix and the DampingFactor
            /// </summary>
            /// <param name="transitionMatrix">The suggested Transition matrix that represents the web graph</param>
            /// <param name="dampingFactor">The suggested DampingFactor, value should be between 0 and 1</param>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given <b>DampingFactor</b> is not in the correct range</exception>
        public PageRank(Matrix transitionMatrix, float dampingFactor)
        {
            if ((dampingFactor > 1) || (dampingFactor < 0))
            {
                throw new ArgumentOutOfRangeException("dampingFactor", dampingFactor,
                    "the DampingFactor should be between 0 and 1");
            }
            _transitionMatrix = transitionMatrix;
            _teleportationMatrix = Matrix.E(transitionMatrix.Size);
            _dampinFactor = dampingFactor;
        }

            /// <summary>
            /// Initialize a new instance of the <b>PageRank</b> Class, using the Transition matrix and the DampingFactor and the specific Teleportation matrix
            /// </summary>
            /// <param name="transitionMatrix">The suggested Transition matrix that represents the web graph</param>
            /// <param name="dampingFactor">The suggested DampingFactor, value should be between 0 and 1</param>
            /// <param name="teleportationMatrix">The suggested Teleportation matrix to calculate the Google matrix</param>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given <b>DampingFactor</b> is not in the correct range</exception>
            /// <exception cref="ArithmeticException">Thrown when the given <b>teleportationMatrix</b> and the <b>transitionMatrix</b> don't have the same size</exception>
        public PageRank(Matrix transitionMatrix, float dampingFactor, Matrix teleportationMatrix)
        {
            if (transitionMatrix.Size != teleportationMatrix.Size)
            {
                throw new ArithmeticException("Can't calculate PageRank with a transition matrix and a teleportation matrix of different lengths");                                                
            }
            if ((dampingFactor > 1) || (dampingFactor < 0))
            {
                throw new ArgumentOutOfRangeException("dampingFactor", dampingFactor,
                    "the DampingFactor should be between 0 and 1");
            }
            _transitionMatrix = transitionMatrix;
             _teleportationMatrix = teleportationMatrix;
             _dampinFactor = dampingFactor;
        }   
            
        /// <summary>
        /// Initialize a new instance of the <b>PageRank</b> Class, using the Transition matrix, the specific Teleportation matrix
        /// and the DefaultDampingFactor <value>0.85</value>
        /// </summary>
        /// <param name="transitionMatrix">The suggested Transition matrix that represents the web graph</param>
        /// <param name="teleportationMatrix">The suggested Teleportation matrix to calculate the Google matrix</param>
        /// <exception cref="ArithmeticException">Thrown when the given <b>teleportationMatrix</b> and the <b>transitionMatrix</b> don't have the same size</exception>
        public PageRank(Matrix transitionMatrix, Matrix teleportationMatrix)
        {
            if (transitionMatrix.Size != teleportationMatrix.Size)
            {
                throw new ArithmeticException("Can't calculate PageRank with a transition matrix and a teleportation matrix of different lengths");
            }
            _transitionMatrix = transitionMatrix;
            _teleportationMatrix = teleportationMatrix;
            _dampinFactor = DefaultDampingFactor;
        }

        #endregion

        #region Methods

            /// <summary>
            /// Calculate the PageRank of all pages represented by the Transition matrix, using the initialVector and the number of itterations
            /// </summary>
            /// <param name="initialVector">The suggested intial vector used to proceed the itterations power method that calculates the PageRank vector</param>
            /// <param name="nbItterations">The suggested number of itterations for the itterations power method</param>
            /// <returns>A row Vector object</returns>
            public Vector GetPageRankVector(Vector initialVector, ulong nbItterations)
            {
                //pi*G = d*pi*A + (1-d)*pi*Q = d*pi*A + (1-d)*e
                DeleteDanglingNodes();

                var pageRankVector = initialVector;
                for (ulong i = 0; i < nbItterations; i++)
                {
                    var temp1 = _dampinFactor * (pageRankVector * _transitionMatrix);
                var temp2 = (1 - _dampinFactor) * Vector.e(VectorType.Row, _transitionMatrix.Size);
                    pageRankVector = temp1 + temp2;
                }
                return pageRankVector;
            }

            /// <summary>
            /// Calculate the PageRank of all pages represented by the Transition matrix, using the initialVector and the degree of convergence
            /// </summary>
            /// <param name="initialVector">The suggested intial vector used to proceed the itterations power method that calculates the PageRank vector</param>
            /// <param name="convergenceDegree">The degree of convergence at which the itterations power method stops calulcation, <example>0.0001</example> </param>
            /// <param name="nbItterations">Returns the number of itterations made by the power method</param>
            /// <returns>A row Vector object</returns>
            public Vector GetPageRankVector(Vector initialVector, short convergenceDegree, out ulong nbItterations)
            {
                //pi*G = d*pi*A + (1-d)*pi*Q = d*pi*A + (1-d)*e
                
                DeleteDanglingNodes();
                
                //Vectors used to valuate convergence 
                Vector previousPageRankValue;
                Vector pageRankVector = previousPageRankValue = initialVector;
                
                //Convergence degree example 0.0001
            double convergenceValue = 1f / (Math.Pow(10, convergenceDegree));
                
                //To test convergence 
            bool converged;
                
                //Itterations number
                nbItterations = 0;

                do
                {
                    converged = true;

                    //New itteration
                    nbItterations++;

                    previousPageRankValue = pageRankVector;

                    
                    var temp1 = _dampinFactor * (pageRankVector * _transitionMatrix);
                var temp2 = (1 - _dampinFactor) * Vector.e(VectorType.Row, _transitionMatrix.Size);
                    pageRankVector = temp1 + temp2;
                    
                    //Testing convergence 

                Parallel.For((long)0, (long)_transitionMatrix.Size, (i, parallelLoopState) =>
                    {
                    if (Math.Abs(pageRankVector[(ulong)i] - previousPageRankValue[(ulong)i]) > convergenceValue)
                        {
                            converged = false;
                            parallelLoopState.Break();
                        }                         
                    });

                } while (!converged);

                return pageRankVector;
            }

            /// <summary>
            /// Calculate the PageRank of all pages represented by the Transition matrix, using the initialVector and the number of itterations, This method 
            /// uses a calulated variant DampingFactor that corresponds to each page <see cref="GetVariantDampingFactorMatrix"/>
            /// </summary>
            /// <param name="initialVector">The suggested intial vector used to proceed the itterations power method that calculates the PageRank vector</param>
            /// <param name="nbItterations">The suggested number of itterations for the itterations power method</param>
            /// <returns>A row Vector object</returns>
            public Vector GetAmelioratedPageRankVector(Vector initialVector, ulong nbItterations)
            {
                //pi*G = d*pi*A + (1-d)*pi*Q = d*pi*A + (1-d)*e
                DeleteDanglingNodes();

                //Calculate Damping factors
                var dampingFactorMatrix = GetVariantDampingFactorMatrix();

                var pageRankVector = initialVector;
                for (ulong i = 0; i < nbItterations; i++)
                {
                    var temp1 = (pageRankVector * _transitionMatrix) * dampingFactorMatrix;
                var temp2 = Vector.e(VectorType.Row, _transitionMatrix.Size) * (Matrix.I(_transitionMatrix.Size) + (-1f) * dampingFactorMatrix);
                    pageRankVector = temp1 + temp2;
                }
                return pageRankVector;
            }

            /// <summary>
            /// Calculate the PageRank of all pages represented by the Transition matrix, using the initialVector and the degree of convergence, This method 
            /// uses a calulated variant DampingFactor that corresponds to each page <see cref="GetVariantDampingFactorMatrix"/>
            /// </summary>
            /// <param name="initialVector">The suggested intial vector used to proceed the itterations power method that calculates the PageRank vector</param>
            /// <param name="convergenceDegree">The degree of convergence at which the itterations power method stops calulcation, <example>0.0001</example> </param>
            /// <param name="nbItterations">Returns the number of itterations made by the power method</param>
            /// <returns>A row Vector object</returns>
            public Vector GetAmelioratedPageRankVector(Vector initialVector, short convergenceDegree, out ulong nbItterations)
            {
                //pi*G = d*pi*A + (1-d)*pi*Q = d*pi*A + (1-d)*e

                DeleteDanglingNodes();

                //Calculate Damping factors
                var dampingFactorMatrix = GetVariantDampingFactorMatrix();
                //Vectors used to valuate convergence 
                Vector previousPageRankValue;
                Vector pageRankVector = previousPageRankValue = initialVector;

                //Convergence degree example 0.0001
                double convergenceValue = 1f / (Math.Pow(10, convergenceDegree));

                //To test convergence 
                bool converged;

                //Itterations number
                nbItterations = 0;

                do
                {
                    converged = true;

                    //New itteration
                    nbItterations++;

                    previousPageRankValue = pageRankVector;


                    var temp1 = (pageRankVector * _transitionMatrix) * dampingFactorMatrix;
                var temp2 = Vector.e(VectorType.Row, _transitionMatrix.Size) * (Matrix.I(_transitionMatrix.Size) + (-1f) * dampingFactorMatrix);
                    pageRankVector = temp1 + temp2;

                    //Testing convergence 

                Parallel.For((long)0, (long)_transitionMatrix.Size, (i, parallelLoopState) =>
                    {
                        if (Math.Abs(pageRankVector[(ulong)i] - previousPageRankValue[(ulong)i]) > convergenceValue)
                        {
                            converged = false;
                            parallelLoopState.Break();
                        }
                    });

                } while (!converged);

                return pageRankVector;
            }

            /// <summary>
            /// Calculate the diagonal matrix containing the DampingFactors corresponding to each page, 
            /// this method is used for the ameliorated version of the GetPageRankVector <see>
            ///     <cref>GetAmelioratedPageRankVector</cref>
            /// </see>
            /// </summary>
            /// <returns>A Matrix object, the matrix is diagonal</returns>
            public Matrix GetVariantDampingFactorMatrix()
            {
            var dampingFactorMatrix = new Matrix(_transitionMatrix.Size);
            for (ulong j = 0; j < _transitionMatrix.Size; j++)
                    {
                        var n = (from page in _transitionMatrix[VectorType.Column, j]
                                where Math.Abs(page) > Epsilon
                                select page).Count();

                        int s = 0;
                for (ulong k = 0; k < _transitionMatrix.Size; k++)
                        {
                            s += (from page in _transitionMatrix[VectorType.Row, k]
                                  where (_transitionMatrix[k, j] > Epsilon) && (page > Epsilon)
                                select page).Count();

                        }
                dampingFactorMatrix[j, j] = (float)n / (float)s;
                    }                    
                return dampingFactorMatrix;
            }
            
            /// <summary>
            /// Fulfil the Zero-lines in the Transition Matrix, this method stochasticize the transition matrix and hence delete the dangling nodes case
            /// </summary>
            private void DeleteDanglingNodes()
            {
            Parallel.For((long)0, (long)_transitionMatrix.Size, (i) =>
                                               {
                                                   if (Math.Abs(_transitionMatrix[VectorType.Row, (ulong)i].Sum()) < Single.Epsilon)
                                                   {
                                                       for (ulong j = 0; j < _transitionMatrix.Size; j++)
                                                       {
                                                           _transitionMatrix[(ulong)i, j] = 1f / _transitionMatrix.Size;
                                                       }
                                                   }
                                               });
            }

            /// <summary>
            /// Calculates the Google matrix used to calculate the PageRank
            /// </summary>
            /// <returns></returns>
        public Matrix GetGoogleMatrix()
                {
                    //G=dA+(1-d)Q
                    DeleteDanglingNodes();
                    //d*A
                    var temp1 = _dampinFactor * _transitionMatrix;
                    //(1-d)Q
            var temp2 = (1f - _dampinFactor) * Matrix.E(_transitionMatrix.Size);
                    //return google matrix
                    return temp1 + temp2;
                }

        #endregion

        #region  Fields

        /// <summary>
        /// The damping factor used to calculate the PageRank
        /// </summary>
        /// <remarks>Value should be between 0 and 1, otherwise <see cref="ArgumentOutOfRangeException"/>will be thrown</remarks>
        private float _dampinFactor;
        /// <summary>
        /// N x N Teleportation matrix used to calculate the Google Matrix
        /// </summary>
            private Matrix _teleportationMatrix;
        /// <summary>
        /// N x N Transition matrix representing the webgraph
        /// </summary>
            private Matrix _transitionMatrix;

        #endregion
    }
}
