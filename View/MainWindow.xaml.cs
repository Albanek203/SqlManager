using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using SqlManager.Data.Models;
using SqlManager.Data.Service;

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
            _dataController.DataSet.Tables.Clear();
            try {
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.TableMappings.AddRange(CreateMappingCollection(TxtQuery.Text));
                adapter.Fill(_dataController.DataSet);
            } catch (Exception exception) {
                MessageBox.Show(exception.ToString().Split('\n')[0], "Error", MessageBoxButton.OK
                              , MessageBoxImage.Error);
                return;
            }

            PanelMain.Children.Clear();
            foreach (DataTable table in _dataController.DataSet.Tables) {
                var grid = new DataGrid {
                    ItemsSource = table.DefaultView, CanUserResizeRows = false, CanUserResizeColumns = false
                };
                grid.BeginningEdit += EditBegin;
                PanelMain.Children.Add(new TextBlock {
                    Text                = table.TableName
                  , FontSize            = 20
                  , Margin              = new Thickness(5)
                  , HorizontalAlignment = HorizontalAlignment.Center
                });
                PanelMain.Children.Add(grid);
                PanelMain.Children.Add(new Separator { Height = 20 });
            }
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
        private void EditBegin(object sender, DataGridBeginningEditEventArgs e) {
            BtnSaveChanges.Visibility = Visibility.Visible;
        }
        private void CmbListNameDb_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            GridExecute.Visibility = CmbListNameDb.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible;
            TxtQuery.Text          = "";
            PanelMain.Children.Clear();
            _dataController.ChangeDb(CmbListNameDb.SelectedItem.ToString());
        }
        private void AcceptChanges(DataTable dataTable) {
            var dbService = App.ServiceProvider.GetService<DbService>();
            if (dbService != null) {
                dbService.InsertRow(dataTable);
                dbService.UpdateRow(dataTable);
                dbService.DeleteRow(dataTable);
            }
            dataTable.AcceptChanges();
        }
        private void SaveChanges_OnClick(object sender, RoutedEventArgs e) {
            foreach (DataTable table in _dataController.DataSet.Tables) AcceptChanges(table);
            BtnSaveChanges.Visibility = Visibility.Collapsed;
        }
    }
}