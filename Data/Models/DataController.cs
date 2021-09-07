using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlManager.Data.Models {
    public class DataController {
        private         string        _currentCatalog;
        public readonly List<string>  DbNameList    = new();
        public          SqlConnection SqlConnection = new();
        public          DataSet       DataSet;
        public void ChangeDb(string newDb) {
            var sqlConnection = SqlConnection.ConnectionString;

            var existsCatalog = sqlConnection.Contains("Initial Catalog");
            if (!existsCatalog)
                sqlConnection += $"Initial Catalog={newDb};";
            else
                sqlConnection = sqlConnection.Replace(_currentCatalog, newDb);
            _currentCatalog = newDb;
            SqlConnection   = new SqlConnection(sqlConnection);
            DataSet         = new DataSet();
        }
    }
}