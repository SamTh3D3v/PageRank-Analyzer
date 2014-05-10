using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PageRankCalculator.Model;
using WebGraphMaker.Model;

namespace PageRankCalculator.BusinessModel
{
    public static class WebGraphDataConverter
    {
        #region Properties


        public static Matrix TransitionMatrix
        {
            get
            {
                return _transitionMatrix;
            } 
        }

        #endregion

        #region Constructors
        #endregion

        #region Methods
        private static Matrix GetAdjacentMatrix(List<Page> pages, List<Link> links)
        {
            var adjacentMatrix = new Matrix((ulong) pages.Count);
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

        public static void SetTransitionMatrix(List<Page> pages, List<Link> links)
        {
            _transitionMatrix = GetAdjacentMatrix(pages, links);
            _transitionMatrix.ToProbablityMatrix();
        }
        #endregion

        #region Fields

        private static Matrix _transitionMatrix;

        #endregion
    }
}
