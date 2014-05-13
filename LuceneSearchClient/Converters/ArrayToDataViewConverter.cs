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
    public class ArrayToDataViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null) return null;  
            var vector = value as Vector;                   
            if (vector.Size == 0) return null;
            var dataTable = new DataTable();
            var newRow = dataTable.NewRow();
            for (ulong column = 0; column < vector.Size; column++)
            {
                dataTable.Columns.Add(new DataColumn(column.ToString(CultureInfo.InvariantCulture)));
                newRow[(int) column] = vector[column];
            }                       
            dataTable.Rows.Add(newRow);
            return dataTable.DefaultView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
