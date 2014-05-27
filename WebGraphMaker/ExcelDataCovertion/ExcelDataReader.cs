using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using LinqToExcel; 

namespace WebGraphMaker.ExcelDataCovertion
{
    public static class ExcelDataReader
    {
        #region Methods

        /// <summary>
        /// Loads the Workbook from Excel file, outputs the file name of the chosen Excel file
        /// </summary>
        /// <param name="fileName">Full name of the chosen Excel file</param>
        /// <exception cref="NullReferenceException">Throw if no file was chosen</exception>
        /// <returns>Excel Workbook</returns>
        private static Workbook LoadWorkbook(out string fileName)
        {
            var xlApp = new Application();
            Workbook xlWorkbook = null;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (.xlsx)|*.xlsx",
                FilterIndex = 1,
                Multiselect = false
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;

                xlWorkbook = xlApp.Workbooks.Open(fileName);
            }
            else
            {
                fileName = null;
            }

            if (xlWorkbook == null) throw new NullReferenceException("Workbook object is null !");
            return xlWorkbook;

        }

            /// <summary>
        /// Reads data from an Excel file, outputs the file name of the chosen Excel file
        /// </summary>
        /// <param name="fileName">The full name of the chosen Excel file</param>
        /// <returns>The used range within the Excel file</returns>
        public static Range ReadData(out string fileName)
        {
            var r = LoadWorkbook(out fileName);
            _Worksheet xlWorksheet = r.Sheets[1];
            return xlWorksheet.UsedRange;
        }

        #endregion
    }
}
