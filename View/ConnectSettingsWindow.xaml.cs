using System;
using System.Data.SqlClient;
using System.Windows;
using SqlManager.Data.Models;

namespace SqlManager.View {
    public partial class ConnectSettingsWindow {
        private readonly DataController _dataController;
        public ConnectSettingsWindow(DataController dataController) {
            InitializeComponent();
            _dataController              = dataController;
            IntegratedSecurity.IsChecked = true;
        }
        private void IntegratedSecurity_OnChecked(object sender, RoutedEventArgs e) {
            PanelIntegratedSecurity.Visibility = Visibility.Collapsed;
        }
        private void IntegratedSecurity_OnUnchecked(object sender, RoutedEventArgs e) {
            PanelIntegratedSecurity.Visibility = Visibility.Visible;
        }
        private void BtnConnect_OnClick(object sender, RoutedEventArgs e) {
            var strConnection = $@"Data Source={TxtServerName.Text};";
            if (IntegratedSecurity.IsChecked == true)
                strConnection += "Integrated Security=True;";
            else
                strConnection += $"User={TxtLogin.Text} Password={TxtPassword.Password};";

            _dataController.SqlConnection = new SqlConnection(strConnection);
            try {
                _dataController.SqlConnection.Open();
                var sqlString =
                    $"SELECT name FROM master.dbo.sysdatabases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb');";
                var cmd    = new SqlCommand(sqlString, _dataController.SqlConnection);
                var reader = cmd.ExecuteReader();
                while (reader.Read()) _dataController.DbNameList.Add(reader.GetString(0));
                _dataController.SqlConnection.Close();
                DialogResult = true;
            } catch (Exception) {
                MessageBox.Show("Error connection", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}