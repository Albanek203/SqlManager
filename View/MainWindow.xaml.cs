using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
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
        private void BtnSettings(object sender, RoutedEventArgs e) {
            Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 100 };

            var errorWindow = App.ServiceProvider.GetService<ConnectSettingsWindow>();
            if (errorWindow != null) {
                errorWindow.Owner = this;
                errorWindow.ShowDialog();
            }
            CmbListNameDb.ItemsSource = _dataController.DbNameList;

            Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 0 };
        }
        private void BtnExecute_OnClick(object sender, RoutedEventArgs e) {
            var adapter = new SqlDataAdapter(TxtQuery.Text, _dataController.SqlConnection);
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            adapter.Fill(_dataController.DataSet);

            PanelMain.Children.Clear();
            foreach (DataTable table in _dataController.DataSet.Tables) {
                PanelMain.Children.Add(new DataGrid { ItemsSource = table.DefaultView });
            }
        }
        private void CmbListNameDb_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            GridExecute.Visibility = CmbListNameDb.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible;
            _dataController.ChangeDb(CmbListNameDb.SelectedItem.ToString());
        }
    }
}