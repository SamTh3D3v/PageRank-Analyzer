using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Core;
using PageRankCalculator.BusinessModel;
using PageRankCalculator.Model;
using WebGraphMaker.businessLogic;
using WebGraphMaker.Model;

namespace PageRankCalculator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            //var openFileDialog1 = new OpenFileDialog
            //{
            //    Filter = "xml Files (.xml)|*.xml",
            //    FilterIndex = 1,
            //    Multiselect = false
            //};

            //var dialogResult1 = openFileDialog1.ShowDialog();
            //WebGraphDataReader xmlReader = null;

            //if (dialogResult1 == DialogResult.OK)
            //{
            //    xmlReader = new WebGraphDataReader();
            //    xmlReader.ExtractDataFromWebGraph(GraphEntities.Links,openFileDialog1.FileName);
            //}

            //var openFileDialog2 = new OpenFileDialog
            //{
            //    Filter = "xml Files (.xml)|*.xml",
            //    FilterIndex = 1,
            //    Multiselect = false
            //};

            //var dialogResult2 = openFileDialog2.ShowDialog();

            //if (dialogResult2 == DialogResult.OK)
            //{
            //    xmlReader.ExtractDataFromWebGraph(GraphEntities.Pages, openFileDialog2.FileName);
            //}


            //WebGraphDataConverter.SetTransitionMatrix(xmlReader.Pages, xmlReader.Links);


            var m = new Matrix(7);
            m[(ulong)0, (ulong)0] = 0f;
            m[(ulong)0, (ulong)1] = 1f;
            m[(ulong)0, (ulong)2] = 0f;
            m[(ulong)0, (ulong)3] = 0f;
            m[(ulong)0, (ulong)4] = 0f;
            m[(ulong)0, (ulong)5] = 0f;
            m[(ulong)0, (ulong)6] = 0f;
            m[(ulong)1, (ulong)0] = 1/2f;
            m[(ulong)1, (ulong)1] = 0f;
            m[(ulong)1, (ulong)2] = 1/2f;
            m[(ulong)1, (ulong)3] = 0f;
            m[(ulong)1, (ulong)4] = 0f;
            m[(ulong)1, (ulong)5] = 0f;
            m[(ulong)1, (ulong)6] = 0f;
            m[(ulong)2, (ulong)0] = 1/3f;
            m[(ulong)2, (ulong)1] = 1/3f;
            m[(ulong)2, (ulong)2] = 0f;
            m[(ulong)2, (ulong)3] = 0f;
            m[(ulong)2, (ulong)4] = 1/3f;
            m[(ulong)2, (ulong)5] = 0f;
            m[(ulong)2, (ulong)6] = 0f;
            m[(ulong)3, (ulong)0] = 1/2f;
            m[(ulong)3, (ulong)1] = 0f;
            m[(ulong)3, (ulong)2] = 0f;
            m[(ulong)3, (ulong)3] = 0f;
            m[(ulong)3, (ulong)4] = 0f;
            m[(ulong)3, (ulong)5] = 1/2f;
            m[(ulong)3, (ulong)6] = 0f;
            m[(ulong)4, (ulong)0] = 0f;
            m[(ulong)4, (ulong)1] = 1/3f;
            m[(ulong)4, (ulong)2] = 1/3f;
            m[(ulong)4, (ulong)3] = 1/3f;
            m[(ulong)4, (ulong)4] = 0f;
            m[(ulong)4, (ulong)5] = 0f;
            m[(ulong)4, (ulong)6] = 0f;
            m[(ulong)5, (ulong)0] = 0f;
            m[(ulong)5, (ulong)1] = 0f;
            m[(ulong)5, (ulong)2] = 0f;
            m[(ulong)5, (ulong)3] = 0f;
            m[(ulong)5, (ulong)4] = 0f;
            m[(ulong)5, (ulong)5] = 0f;
            m[(ulong)5, (ulong)6] = 1f;
            m[(ulong)6, (ulong)0] = 0f;
            m[(ulong)6, (ulong)1] = 0f;
            m[(ulong)6, (ulong)2] = 0f;
            m[(ulong)6, (ulong)3] = 0f;
            m[(ulong)6, (ulong)4] = 0f;
            m[(ulong)6, (ulong)5] = 1f;
            m[(ulong)6, (ulong)6] = 0f;
            
            //var r = new Random();

            //for (ulong i = 0; i < 2000; i++)
            //{
            //    if (i==1000)
            //    {
            //        for (ulong j = 0; j < 2000; j++)
            //        {
            //            m[i, j] = 0;
            //        } 
            //    }
            //    else
            //    {
            //        for (ulong j = 0; j < 2000; j++)
            //        {
            //            m[i, j] = r.Next(0, 2);
            //        }   
            //    }
            //}
            
            m.ToProbablityMatrix();
            var isStochastic = m.IsSochasitc();
            var isIrreductible = m.IsIrreducible();
            m.Eigenvalues();

            var timeDebut = DateTime.Now;

            var pageRank = new PageRank(m,0.6f);
            var m2 = pageRank.GoogleMatrix();
            var b1 = m2.IsSochasitc();
            var b2 = m2.IsIrreducible();
            var e = m2.Eigenvalues();

            Console.WriteLine("Start calculation !!");

            ulong itterations;
            var v = pageRank.PageRankVector(Vector.e(VectorType.Row, 7), (short)5,out itterations);

            var timeEnd = DateTime.Now;
            var t = (timeEnd - timeDebut).TotalMilliseconds;

            Console.Read();
        }
    }
}
