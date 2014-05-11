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
        public static float DefaultDampingFactor = 0.85f;
        #endregion

        #region Properties
            public float DampingFactor
        {
            get
            {
                return _dampinFactor;
            }
        }
            public Matrix TeleportationMatrix
        {
            get
            {
                return _teleportationMatrix;
            }
        }
            public Matrix TransitionMatrix
        {
            get
            {
                return _transitionMatrix;
            } 
        }
        #endregion

        #region Constructors

        public PageRank(Matrix transitionMatrix, float dampingFactor)
        {
            if ((dampingFactor > 1) || (dampingFactor < 0))
            {
                throw new ArgumentOutOfRangeException("dampingFactor", dampingFactor,
                    "the DampingFactor should be between 0 and 1");
            }
            _transitionMatrix = transitionMatrix;
            _teleportationMatrix = Matrix.E(transitionMatrix.Length);
            _dampinFactor = dampingFactor;
        }
        public PageRank(Matrix transitionMatrix, float dampingFactor, Matrix teleportationMatrix)
        {
            if (transitionMatrix.Length != teleportationMatrix.Length)
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
            
        #endregion

        #region Methods

            public Vector PageRankVector(Vector initialVector, ulong nbItterations)
            {
                //pi*G = d*pi*A + (1-d)*pi*Q = d*pi*A + (1-d)*e
                DeleteDanglingNodes();

                var pageRankVector = initialVector;
                for (ulong i = 0; i < nbItterations; i++)
                {
                    var temp1 = _dampinFactor * (pageRankVector * _transitionMatrix);
                    var temp2 = (1 - _dampinFactor) * Vector.e(VectorType.Row, _transitionMatrix.Length);
                    pageRankVector = temp1 + temp2;
                }
                return pageRankVector;
            }

            public Vector PageRankVector(Vector initialVector, short convergenceDegree, out ulong nbItterations)
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
                    var temp2 = (1 - _dampinFactor) * Vector.e(VectorType.Row, _transitionMatrix.Length);
                    pageRankVector = temp1 + temp2;
                    
                    //Testing convergence 

                    Parallel.For((long)0, (long)_transitionMatrix.Length, (i, parallelLoopState) =>
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

            public Vector ModifiedPageRankVector(Vector initialVector, ulong nbItterations)
            {
                //pi*G = d*pi*A + (1-d)*pi*Q = d*pi*A + (1-d)*e
                DeleteDanglingNodes();

                //Calculate Damping factors
                var dampingFactorMatrix = DampingFactorMatrix(_transitionMatrix);

                var pageRankVector = initialVector;
                for (ulong i = 0; i < nbItterations; i++)
                {
                    var temp1 = (pageRankVector * _transitionMatrix) * dampingFactorMatrix;
                    var temp2 = Vector.e(VectorType.Row, _transitionMatrix.Length) * (Matrix.I(_transitionMatrix.Length) + (-1f) * dampingFactorMatrix);
                    pageRankVector = temp1 + temp2;
                }
                return pageRankVector;
            }

            public Vector ModifiedPageRankVector(Vector initialVector, short convergenceDegree, out ulong nbItterations)
            {
                //pi*G = d*pi*A + (1-d)*pi*Q = d*pi*A + (1-d)*e

                DeleteDanglingNodes();

                //Calculate Damping factors
                var dampingFactorMatrix = DampingFactorMatrix(_transitionMatrix);
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
                    var temp2 = Vector.e(VectorType.Row, _transitionMatrix.Length) * (Matrix.I(_transitionMatrix.Length) + (-1f) * dampingFactorMatrix);
                    pageRankVector = temp1 + temp2;

                    //Testing convergence 

                    Parallel.For((long)0, (long)_transitionMatrix.Length, (i, parallelLoopState) =>
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


            public Matrix DampingFactorMatrix(Matrix trannsitionMatrix)
            {
                var dampingFactorMatrix = new Matrix(trannsitionMatrix.Length);
                    for (ulong j = 0; j < trannsitionMatrix.Length; j++)
                    {
                        var n = (from page in trannsitionMatrix[VectorType.Column, j]
                                where Math.Abs(page) > Epsilon
                                select page).Count();

                        int s = 0;
                        for (ulong k = 0; k < trannsitionMatrix.Length; k++)
                        {
                            s += (from page in trannsitionMatrix[VectorType.Row, k]
                                where (trannsitionMatrix[k, j] > Epsilon) && (page > Epsilon)
                                select page).Count();

                        }
                        dampingFactorMatrix[j, j] = (float) n/(float) s;
                    }                    
                return dampingFactorMatrix;
            }
            
            private void DeleteDanglingNodes()
            {
            Parallel.For((long)0, (long)_transitionMatrix.Length, (i) =>
                                               {
                                                   if (Math.Abs(_transitionMatrix[VectorType.Row, (ulong)i].Sum()) < Single.Epsilon)
                                                   {
                                                       for (ulong j = 0; j < _transitionMatrix.Length; j++)
                                                       {
                                                           _transitionMatrix[(ulong)i, j] = 1f / _transitionMatrix.Length;
                                                       }
                                                   }
                                               });
            }

            public Matrix GoogleMatrix()
                {
                    //G=dA+(1-d)Q
                    DeleteDanglingNodes();
                    //d*A
                    var temp1 = _dampinFactor * _transitionMatrix;
                    //(1-d)Q
                    var temp2 = (1f - _dampinFactor) * Matrix.E(_transitionMatrix.Length);
                    //return google matrix
                    return temp1 + temp2;
                }

        #endregion

        #region  Fields

            private const float Epsilon = 0.0001f;
            private float _dampinFactor ;
            private Matrix _teleportationMatrix;
            private Matrix _transitionMatrix;

        #endregion
    }
}
