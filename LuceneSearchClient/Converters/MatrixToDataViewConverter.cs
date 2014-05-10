using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LuceneSearchClient.Converters
{
    public class MatrixToDataViewConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var array = value as short[,];
            if (array == null) return null;
            //rows
            if (array.GetLength(0) == 0) return null;            
            if (array.GetLength(1) == 0) return null;
            var dataTable = new DataTable();
            for (var column = 0; column < array.GetLength(1); column++)
            {
                dataTable.Columns.Add(new DataColumn(column.ToString(CultureInfo.InvariantCulture)));
            }            
            for (var row = 0; row < array.GetLength(0); row++)
            {
                var newRow = dataTable.NewRow();
                for (var column = 0; column < array.GetLength(1); column++)
                {
                    newRow[column] = array[row, column];
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
