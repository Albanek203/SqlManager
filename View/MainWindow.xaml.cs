using System.Data;
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
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) { ShowSettings(); }
        //
        private void BtnSettings(object sender, RoutedEventArgs e) { ShowSettings(); }
        private void ShowSettings() {
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
        private void BtnExecute_OnClick(object sender, RoutedEventArgs e) {
            var error = _dataController.Execute(TxtQuery.Text);
            if (error != null) {
                PanelMain.Children.Clear();
                _dataController.DataSet.Tables.Clear();
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void EditBegin(object sender, DataGridBeginningEditEventArgs e) {
            BtnSaveChanges.Visibility = Visibility.Visible;
        }
        private void CmbListNameDb_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            GridExecute.Visibility = CmbListNameDb.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible;
            TxtQuery.Text          = "";
            PanelMain.Children.Clear();
            _dataController.ChangeDb(CmbListNameDb.SelectedItem.ToString());
        }
        private void SaveChanges_OnClick(object sender, RoutedEventArgs e) {
            _dataController.SqlDataAdapter.Update(_dataController.DataSet);
            BtnSaveChanges.Visibility = Visibility.Collapsed;
        }
    }
}