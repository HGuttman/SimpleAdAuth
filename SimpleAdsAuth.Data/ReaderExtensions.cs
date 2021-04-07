using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SimpleAdsAuth.Data
{
    public static class ReaderExtensions
    {
        public static T Get<T>(this SqlDataReader reader, string name)
        {
            object value = reader[name];
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}
