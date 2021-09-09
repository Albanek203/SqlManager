using System.Threading.Tasks;
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
        private async Task<bool> CheckConnection() {
            PanelInputValues.Visibility = Visibility.Collapsed;
            TxtLoading.Visibility       = Visibility.Visible;
            BtnExit.Visibility          = Visibility.Collapsed;

            var success = IntegratedSecurity.IsChecked != null && await _dataController.Connection(new ConnectionData {
                ServerName         = TxtServerName.Text
              , Login              = TxtLogin.Text
              , Password           = TxtPassword.Password
              , IntegratedSecurity = (bool)IntegratedSecurity.IsChecked
            });

            if (success) DialogResult = true;
            TxtLoading.Visibility       = Visibility.Collapsed;
            PanelInputValues.Visibility = Visibility.Visible;
            BtnExit.Visibility          = Visibility.Visible;
            return success;
        }
        private async void BtnConnect_OnClick(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtServerName.Text)) {
                MessageBox.Show("Enter server name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var success = await CheckConnection();
            if (!success) MessageBox.Show("Error connection", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void BtnExit_OnClick(object sender, RoutedEventArgs e) { DialogResult = false; }
    }
}