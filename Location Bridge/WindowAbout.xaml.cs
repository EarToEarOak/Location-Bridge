using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace Location_Bridge
{
    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();

            textTitle.Text = assembly.GetName().Name;
            var attrDesc = assembly
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault();
            textDesc.Text = attrDesc.Description;
            textVersion.Text = "Version " + assembly.GetName().Version.ToString();
            var attrCopyright = assembly
                .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
                .OfType<AssemblyCopyrightAttribute>()
                .FirstOrDefault();
            textCopyright.Text = attrCopyright.Copyright;
        }

        private void OnButtonOk(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnLink(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }
    }
}
