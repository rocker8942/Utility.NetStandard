using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Utility.Enumerable
{
    public static class EnumerableHelper
    {
        /// <summary>
        /// Convert an enumerable of type T to data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable)
        {
            var dataTable = new DataTable();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var readableProperties = properties.Where(w => w.CanRead).ToArray();

            var columnNames = readableProperties.Select(s => s.Name).ToArray();
            foreach (var name in columnNames)
            {
                var propertyInfo = readableProperties.Single(s => s.Name.Equals(name));
                var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                dataTable.Columns.Add(name, type);
            }

            foreach (var obj in enumerable)
            {
                var values = columnNames
                    .Select(s => readableProperties
                        .Single(s2 => s2.Name.Equals(s))
                        .GetValue(obj) ?? DBNull.Value)
                    .ToArray();
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}