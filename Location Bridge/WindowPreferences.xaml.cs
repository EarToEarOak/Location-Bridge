using System.Windows;

namespace Location_Bridge
{
    public partial class WindowPreferences : Window
    {
        public WindowPreferences()
        {
            InitializeComponent();
            checkMinimised.IsChecked = Properties.Settings.Default.StartupMinimised;
            checkStart.IsChecked = Properties.Settings.Default.StartupGps;
            checkLocal.IsChecked = Properties.Settings.Default.LocalOnly;
        }

        private void OnButtonOk(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StartupMinimised = (bool)checkMinimised.IsChecked;
            Properties.Settings.Default.StartupGps = (bool)checkStart.IsChecked;
            Properties.Settings.Default.LocalOnly = (bool)checkLocal.IsChecked;
            Properties.Settings.Default.Save();
            Close();
        }

        private void OnButtonCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
