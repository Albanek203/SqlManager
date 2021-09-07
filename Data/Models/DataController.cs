using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace SqlManager.Data.Models {
    public class DataController {
        public readonly List<string>  DbNameList    = new List<string>();
        public          SqlConnection SqlConnection = new SqlConnection();
        public          DataSet       DataSet       = new DataSet();
        public void ChangeDb(string newDb) {
            var sqlConnection = SqlConnection.ConnectionString;

            var s = sqlConnection.IndexOf("Initial Catalog", StringComparison.Ordinal);
            if (s == -1) {
                sqlConnection += $"Initial Catalog={newDb};";
            }
            else {
                var insert = sqlConnection.Insert(s, "9");
                MessageBox.Show("Hello\r\n" + insert + $"\r\n {s}");
            }

            SqlConnection = new SqlConnection(sqlConnection);
        }
    }
}