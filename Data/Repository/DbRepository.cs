using System.Data;
using System.Data.SqlClient;
using SqlManager.Data.Models;

namespace SqlManager.Data.Repository {
    public class DbRepository {
        private readonly SqlConnection _sqlConnection;
        public DbRepository(DataController dataController) { _sqlConnection = dataController.SqlConnection; }
        // Insert Row
        private void InsertRow(DataRow dataRow) {
            var sqlString = $"INSERT {dataRow.Table.TableName} (";
            for (var i = 1; i < dataRow.Table.Columns.Count; i++) {
                if (i == dataRow.Table.Columns.Count - 1)
                    sqlString += $"{dataRow.Table.Columns[i].ColumnName}) ";
                else
                    sqlString += $"{dataRow.Table.Columns[i].ColumnName}, ";
            }
            sqlString += "VALUES (";
            for (var i = 1; i < dataRow.Table.Columns.Count; i++) {
                if (i == dataRow.Table.Columns.Count - 1) {
                    if (!(dataRow.Table.Columns[i].DataType == typeof(string)))
                        sqlString += $"{dataRow[i]});";
                    else
                        sqlString += $"'{dataRow[i]}');";
                }
                else {
                    if (!(dataRow.Table.Columns[i].DataType == typeof(string)))
                        sqlString += $"{dataRow[i]},";
                    else
                        sqlString += $"'{dataRow[i]}',";
                }
            }

            var cmd = new SqlCommand(sqlString, _sqlConnection);
            _sqlConnection.Open();
            cmd.ExecuteNonQuery();
            _sqlConnection.Close();
        }
        public void InsertRow(DataTable dataTable) {
            foreach (DataRow row in dataTable.Rows)
                if (row.RowState == DataRowState.Added)
                    InsertRow(row);
        }
        // Delete Row
        private void DeleteRow(DataRow dataRow) {
            var sqlString = $"DELETE {dataRow.Table.TableName} WHERE Id={dataRow[0, DataRowVersion.Original]}";
            var cmd       = new SqlCommand(sqlString, _sqlConnection);
            _sqlConnection.Open();
            cmd.ExecuteNonQuery();
            _sqlConnection.Close();
        }
        public void DeleteRow(DataTable dataTable) {
            foreach (DataRow row in dataTable.Rows)
                if (row.RowState == DataRowState.Deleted)
                    DeleteRow(row);
        }
        // Update Row
        private void UpdateRow(DataRow dataRow) {
            var sqlString = $"UPDATE {dataRow.Table.TableName} SET ";
            for (var i = 1; i < dataRow.Table.Columns.Count; i++) {
                if (!(dataRow.Table.Columns[i].DataType == typeof(string))) {
                    if (i == dataRow.Table.Columns.Count - 1)
                        sqlString +=
                            $"{dataRow.Table.Columns[i].ColumnName} = {dataRow[i].ToString()?.Replace(',', '.')} ";
                    else
                        sqlString +=
                            $"{dataRow.Table.Columns[i].ColumnName} = {dataRow[i].ToString()?.Replace(',', '.')}, ";
                }
                else {
                    if (i == dataRow.Table.Columns.Count - 1)
                        sqlString += $"{dataRow.Table.Columns[i].ColumnName} = '{dataRow[i]}' ";
                    else
                        sqlString += $"{dataRow.Table.Columns[i].ColumnName} = '{dataRow[i]}', ";
                }
            }
            sqlString += $"WHERE Id = {dataRow[0]}";
            var cmd = new SqlCommand(sqlString, _sqlConnection);
            _sqlConnection.Open();
            cmd.ExecuteNonQuery();
            _sqlConnection.Close();
        }
        public void UpdateRow(DataTable dataTable) {
            foreach (DataRow row in dataTable.Rows)
                if (row.RowState == DataRowState.Modified)
                    UpdateRow(row);
        }
    }
}