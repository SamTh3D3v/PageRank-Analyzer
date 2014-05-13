using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PageRankCalculator.Model;

namespace LuceneSearchClient.Converters
{
    public class MatrixToDataViewConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var matrix = value as Matrix;                        
            if (matrix.Size == 0) return null;                        
            var dataTable = new DataTable();
            for (ulong column = 0; column < matrix.Size; column++)
            {
                dataTable.Columns.Add(new DataColumn(column.ToString(CultureInfo.InvariantCulture)));
            }
            for (ulong row = 0; row < matrix.Size; row++)
            {
                var newRow = dataTable.NewRow();
                for (ulong column = 0; column < matrix.Size; column++)
                {
                    newRow[(int) column] = matrix[row, column];
                }
                dataTable.Rows.Add(newRow);
            }
            return dataTable.DefaultView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
