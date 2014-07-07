using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;

namespace Location_Bridge
{
    public partial class WindowUpdate : Window
    {
        public WindowUpdate()
        {
            InitializeComponent();

            textUpdate.Text = "Checking for update...";

            var webClient = new WebClient();
            webClient.DownloadDataCompleted += Completed;
            try
            {
                webClient.DownloadDataAsync(new Uri("https://raw.githubusercontent.com/EarToEarOak/Location-Bridge/master/Location%20Bridge/Setup/setup.iss"));
            }
            catch (WebException)
            {
                textUpdate.Text = "Connection failed";
            }
        }

        private void Completed(object sender, DownloadDataCompletedEventArgs e)
        {
            int major, minor, build;

            var raw = e.Result;
            var file = Encoding.UTF8.GetString(raw);
            var regex = new Regex(@"#define _AppVersion ""(\d+)\.(\d+)\.(\d+)""");
            var match = regex.Match(file);

            if (match.Groups.Count == 4)
            {
                if (Int32.TryParse(match.Groups[1].ToString(), out major) &&
                    Int32.TryParse(match.Groups[2].ToString(), out minor) &&
                    Int32.TryParse(match.Groups[3].ToString(), out build))
                {
                    var verRemote = new Version(major, minor, build);
                    var assembly = Assembly.GetExecutingAssembly();
                    var verLocal = assembly.GetName().Version;

                    if (verRemote > verLocal)
                        textUpdate.Text = String.Format("Update available: {0}",
                            verRemote.ToString());
                    else
                        textUpdate.Text = "No update available";
                }
                else
                    textUpdate.Text = "Check failed";
            }
            else
                textUpdate.Text = "Check failed";
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
