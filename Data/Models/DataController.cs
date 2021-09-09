using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace SqlManager.Data.Models {
    public class DataController {
        public readonly List<string>   DbNameList = new();
        private         string         _currentCatalog;
        private         SqlConnection  _sqlConnection = new();
        public          SqlDataAdapter SqlDataAdapter;
        public          DataSet        DataSet;
        public          string         CurrentServer { get; set; }
        public string Execute(string query) {
            SqlDataAdapter = new SqlDataAdapter(query, _sqlConnection);
            var cmdBuilder = new SqlCommandBuilder(SqlDataAdapter);
            DataSet.Tables.Clear();
            try {
                SqlDataAdapter.InsertCommand       = cmdBuilder.GetInsertCommand();
                SqlDataAdapter.UpdateCommand       = cmdBuilder.GetUpdateCommand();
                SqlDataAdapter.DeleteCommand       = cmdBuilder.GetDeleteCommand();
                SqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                SqlDataAdapter.TableMappings.AddRange(CreateMappingCollection(query));
                SqlDataAdapter.Fill(DataSet);
            } catch (Exception exception) {
                return exception.ToString().Split('\n')[0];
            }
            return null;
        }
        private DataTableMapping[] CreateMappingCollection(string cmd) {
            cmd = cmd.ToLower();
            cmd = cmd.Replace(";", " ");
            var res = cmd.Split("from");
            var map = new DataTableMapping[res.Length - 1];

            for (int i = 1, countIndex = 0; i < res.Length; i++, countIndex++) {
                if (i == 1)
                    map[countIndex] = new DataTableMapping("Table", res[i].Split(" ")[1]);
                else
                    map[countIndex] = new DataTableMapping($"Table{countIndex}", res[i].Split(" ")[1]);
            }
            return map;
        }
        public void ChangeDb(string newDb) {
            var sqlConnection = _sqlConnection.ConnectionString;

            var existsCatalog = sqlConnection.Contains("Initial Catalog");
            if (!existsCatalog)
                sqlConnection += $"Initial Catalog={newDb};";
            else
                sqlConnection = sqlConnection.Replace(_currentCatalog, newDb);
            _currentCatalog = newDb;
            _sqlConnection  = new SqlConnection(sqlConnection);
            DataSet         = new DataSet();
        }
        public async Task<bool> Connection(ConnectionData connectionData) {
            var strConnection = $@"Data Source={connectionData.ServerName};";
            if (connectionData.IntegratedSecurity)
                strConnection += "Integrated Security=True;";
            else {
                if (string.IsNullOrWhiteSpace(connectionData.Login) ||
                    string.IsNullOrWhiteSpace(connectionData.Password)) {
                    MessageBox.Show("Enter login and password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                strConnection += $"User={connectionData.Login} Password={connectionData.Password};";
            }
            _sqlConnection = new SqlConnection(strConnection);
            var success = await CheckConnection();
            CurrentServer = connectionData.ServerName;
            return success;
        }
        private async Task<bool> CheckConnection() {
            DbNameList.Clear();
            var result = true;
            await Task.Run(() => {
                try {
                    _sqlConnection.Open();
                    var sqlString =
                        $"SELECT name FROM master.dbo.sysdatabases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb');";
                    var cmd    = new SqlCommand(sqlString, _sqlConnection);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read()) DbNameList.Add(reader.GetString(0));
                    _sqlConnection.Close();
                } catch (Exception) { result = false; }
            });
            return result;
        }
    }
}