using System.Collections.Generic;
using System.Linq;
using PageRankCalculator.Model;
using WebGraphMaker.Model;

namespace PageRankCalculator.PageRankCalculation
{
    public static class WebGraphDataConverter
    {
        #region Properties

        /// <summary>
        /// Get the Transition matrix resulted from the conversion of a WebGraph
        /// </summary>
        public static Matrix TransitionMatrix
        {
            get
            {
                return _transitionMatrix;
            }
        }
        public static Matrix AdjacentMatrix
        {
            get
            {
                return _adjacentMatrix;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the adjacence matrix, from a list of pages and the corresponding list of links binding them 
        /// </summary>
        /// <param name="pages">The suggested pages list</param>
        /// <param name="links">The suggested links list</param>
        /// <returns>a N x N matrix representing the links between pages</returns>
        private static Matrix GetAdjacentMatrix(List<Page> pages, List<Link> links)
        {
            var adjacentMatrix = new Matrix((ulong)pages.Count);
            foreach (var page in pages)
            {
                var tailPages = (from link in links
                                 where link.TailPageId == page.Id
                                 select link.HeadPageId).ToList();
                foreach (var tailPage in tailPages)
                {
                    adjacentMatrix[page.Id, tailPage] = 1;
                }
            }
            return adjacentMatrix;
        }

        /// <summary>
        /// Sets the Transition matrix, from a list of pages and the corresponding list of links binding them
        /// </summary>
        /// <param name="pages">The suggested pages list</param>
        /// <param name="links">The suggested pages list</param>
        public static void SetTransitionMatrix(List<Page> pages, List<Link> links)
        {
            _transitionMatrix = GetAdjacentMatrix(pages, links);
            _adjacentMatrix = new Matrix(_transitionMatrix);
            _transitionMatrix.ToProbablityMatrix();
        }
        
        #endregion

        #region Fields

        /// <summary>
        /// Represents the Transition matrix generated from converting the WebGraph
        /// </summary>
        private static Matrix _transitionMatrix;
        private static Matrix _adjacentMatrix;
        


        #endregion
    }
}
