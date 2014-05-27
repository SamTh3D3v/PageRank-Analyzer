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
    public class VectorToDataViewColumnConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var vector = value as Vector;
            if (vector.Size == 0) return null;
            var dataTable = new DataTable();
            dataTable.Columns.Add("values");
            for (ulong row = 0; row < vector.Size; row++)
            {
                var newRow = dataTable.NewRow();                
                newRow[0] = vector[row];
                dataTable.Rows.Add(newRow);
            }            
            return dataTable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
