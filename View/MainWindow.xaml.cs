using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using SqlManager.Data.Models;

namespace SqlManager.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private readonly DataController _dataController;
        public MainWindow(DataController dataController) {
            InitializeComponent();
            _dataController = dataController;
        }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 100 };
            var errorWindow = App.ServiceProvider.GetService<ConnectSettingsWindow>();
            if (errorWindow != null) {
                errorWindow.Owner = this;
                if (errorWindow.ShowDialog() == true) {
                    CmbListNameDb.ItemsSource = _dataController.DbNameList;
                    TxtServerName.Text        = $"Server: {_dataController.CurrentServer}";
                }
            }
            Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 0 };
        }
        private void MovePanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) { DragMove(); }
        private void BtnClose_OnClick(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }
        private void BtnHide_OnClick(object sender, RoutedEventArgs e) { WindowState = WindowState.Minimized; }
        private void BtnSettings(object sender, RoutedEventArgs e) {
            Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 100 };
            var errorWindow = App.ServiceProvider.GetService<ConnectSettingsWindow>();
            if (errorWindow != null) {
                errorWindow.Owner = this;
                if (errorWindow.ShowDialog() == true) {
                    CmbListNameDb.ItemsSource = _dataController.DbNameList;
                    TxtServerName.Text        = $"Server: {_dataController.CurrentServer}";
                }
            }
            Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 0 };
        }
        private void BtnExecute_OnClick(object sender, RoutedEventArgs e) {
            var adapter = new SqlDataAdapter(TxtQuery.Text, _dataController.SqlConnection);
            /*adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;*/
            try {
                adapter.Fill(_dataController.DataSet);
            } catch (Exception exception) {
                MessageBox.Show(exception.ToString().Split('\n')[0], "Error", MessageBoxButton.OK
                              , MessageBoxImage.Error);
                return;
            }

            PanelMain.Children.Clear();
            foreach (DataTable table in _dataController.DataSet.Tables) {
                PanelMain.Children.Add(new DataGrid {
                    ItemsSource = table.DefaultView, CanUserResizeRows = false, CanUserResizeColumns = false
                });
            }
        }
        private void CmbListNameDb_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            GridExecute.Visibility = CmbListNameDb.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible;
            TxtQuery.Text          = "";
            PanelMain.Children.Clear();
            _dataController.ChangeDb(CmbListNameDb.SelectedItem.ToString());
        }
    }
}