using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;

using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;


namespace WebGraphMaker.ExcelDataCovertion
{
    public static class ExcelDataReader
    {
        #region Methods

        /// <summary>
        /// Loads the Workbook from Excel file filename 
        /// </summary>
        /// <param name="fileName">Full name of the chosen Excel file</param>
        /// <exception cref="NullReferenceException">Throw if no file was chosen</exception>
        /// <returns>Excel Workbook</returns>
        private static Workbook LoadWorkbook(string fileName)
        {
            var xlApp = new Application();
            Workbook xlWorkbook = null;
            if (fileName!=null)
            {
                xlWorkbook = xlApp.Workbooks.Open(fileName);
            }
            if (xlWorkbook == null) throw new NullReferenceException("Workbook object is null !");
            return xlWorkbook;

        }

        /// <summary>
        /// Reads data from an Excel file, inputs the file name of the chosen Excel file
        /// </summary>
        /// <param name="fileName">The full name of the chosen Excel file</param>
        /// <returns>The used range within the Excel file</returns>
        public static Range ReadData(string fileName)
        {
            var r = LoadWorkbook(fileName);
            _Worksheet xlWorksheet = r.Sheets[1];
            var tmp = xlWorksheet.UsedRange;
            return tmp;
        }

        #endregion
    }
}
