using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;


namespace WebGraphMaker.businessLogic
{
    public static class ExcelDataReader
    {
        #region Methods
            private static Workbook LoadWorkbook()
            {
                Debug.WriteLine("Loading Excel file ...");
                
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
                    xlWorkbook = xlApp.Workbooks.Open(openFileDialog.FileName);
                }

                if (xlWorkbook == null) throw new NullReferenceException("Workbook object is null !");
                return xlWorkbook;
            
            }

            public static Range ReadData()
            {
                var r = LoadWorkbook();
                _Worksheet xlWorksheet = r.Sheets[1];
                return xlWorksheet.UsedRange;
            }

        #endregion

    }
}
