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
    public class ArrayToDataViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var array = value as float[];
            if (array == null) return null;
            //colomns
            if (array.Length == 0) return null;
            var dataTable = new DataTable();
            for (var column = 0; column < array.Length; column++)
            {
                dataTable.Columns.Add(new DataColumn(column.ToString(CultureInfo.InvariantCulture)));
            }
            var newRow = dataTable.NewRow();
            for (var column = 0; column < array.Length; column++)
            {
                newRow[column] = array[column];
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
