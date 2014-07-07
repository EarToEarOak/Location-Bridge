using System;
using System.Device.Location;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Location_Bridge
{
    public partial class WindowMain : Window, IDisposable
    {

        private Forms.ContextMenuStrip contextMenuStrip = new Forms.ContextMenuStrip();
        private Forms.NotifyIcon notifyIcon;
        private WindowMainModel ui;

        private GeoCoordinateWatcher watcher;
        private Location location = new Location();
        private Object lockGps = new Object();
        private Server server;
        private Thread threadServer;

        private bool disposed = false;

        public WindowMain()
        {
            InitializeComponent();
            ui = (WindowMainModel)base.DataContext;

            var appName = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                 Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false))
                .Title;
            Title = appName;

            contextMenuStrip.Items.AddRange(new Forms.ToolStripItem[] {
                new Forms.ToolStripMenuItem(appName, null,
                    new EventHandler(OnMenuOpen), null),
                new Forms.ToolStripSeparator(),
                new Forms.ToolStripMenuItem("Start", null,
                    new EventHandler(OnMenuStart), null),
                new Forms.ToolStripMenuItem("Stop", null,
                    new EventHandler(OnMenuStop), null),
                new Forms.ToolStripSeparator(),
                new Forms.ToolStripMenuItem("Exit", null,
                    new EventHandler(OnMenuExit), null)
            });
            var font = new Font(contextMenuStrip.Items[0].Font,
                System.Drawing.FontStyle.Bold);
            contextMenuStrip.Items[0].Font = font;
            MenuState(true);

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(
                Assembly.GetEntryAssembly().ManifestModule.Name);
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipTitle = appName;
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.DoubleClick += new EventHandler(OnMenuOpen);
            notifyIcon.BalloonTipClicked += new EventHandler(OnMenuOpen);

            ServerStart();

            if (Properties.Settings.Default.StartupGps)
                GpsStart();

            if (Properties.Settings.Default.StartupMinimised)
                MinimiseToTray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                {
                    contextMenuStrip.Dispose();
                    notifyIcon.Dispose();
                    if (watcher != null)
                        watcher.Dispose();
                }
            disposed = true;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                MinimiseToTray();

            base.OnStateChanged(e);
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GpsStop();
            ServerStop();
            notifyIcon.Visible = false;
            notifyIcon.Icon = null;
        }

        private void OnMenuOpen(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void OnMenuStart(object sender, EventArgs e)
        {
            GpsStart();
        }

        private void OnMenuStop(object sender, EventArgs e)
        {
            GpsStop();
        }

        private void OnMenuClear(object sender, EventArgs e)
        {
            ui.LogClear();
        }

        private void OnMenuPrefs(object sender, EventArgs e)
        {
            var serverLocal = Properties.Settings.Default.LocalOnly;
            WindowPreferences prefs = new WindowPreferences();
            prefs.ShowDialog();
            if (serverLocal != Properties.Settings.Default.LocalOnly)
            {
                ServerStop();
                ServerStart();
            }
        }

        private void OnMenuUpdate(object sender, EventArgs e)
        {
            WindowUpdate update = new WindowUpdate();
            update.ShowDialog();
        }

        private void OnMenuAbout(object sender, EventArgs e)
        {
            WindowAbout about = new WindowAbout();
            about.ShowDialog();
        }

        private void OnMenuExit(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnButtonStart(object sender, RoutedEventArgs e)
        {
            if (watcher == null)
                GpsStart();
            else
                GpsStop();
        }

        private void OnGpsPos(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Led.Flash();
            lock (lockGps)
            {
                if (!e.Position.Location.IsUnknown)
                {
                    location.lat = e.Position.Location.Latitude;
                    location.lon = e.Position.Location.Longitude;
                    location.alt = e.Position.Location.Altitude;
                    location.speed = e.Position.Location.Speed;
                    location.ha = e.Position.Location.HorizontalAccuracy;
                    location.va = e.Position.Location.VerticalAccuracy;
                    location.time = e.Position.Timestamp.ToUniversalTime();

                    if (server != null)
                        server.GpsUpdate(location);

                    ui.Latitude = location.lat;
                    ui.Longitude = location.lon;
                    ui.Altitude = location.alt;
                    ui.Speed = location.speed;
                    ui.Time = location.time;
                }
                else
                    ui.LocationClear();
            }
        }

        private void OnGpsStatus(object sender, GeoPositionStatusChangedEventArgs e)
        {
            ui.LogAdd("GPS: " + e.Status.ToString());

            if (e.Status == GeoPositionStatus.Disabled)
                GpsStop();
        }

        private void ServerStart()
        {
            if (threadServer == null)
            {
                server = new Server(lockGps, new CallbackUI(ServerStatus));
                threadServer = new Thread(new ThreadStart(server.Start));
                threadServer.Start();
                while (!threadServer.IsAlive) ;
            }
        }

        private void ServerStop()
        {
            if (threadServer != null)
            {
                server.Stop();
                threadServer.Join(1000);
                threadServer = null;
            }
        }

        private void ServerStatus(ServerStatus status)
        {
            switch (status.status)
            {
                case ServerStatusCode.OK:
                    break;
                case ServerStatusCode.ERROR:
                    GpsStop();
                    ServerStop();
                    ui.ButtonEnabled = false;
                    break;
                case ServerStatusCode.CONN:
                    ui.Clients = status.value;
                    break;
            }

            if (status.message != null)
                ui.LogAdd("Server: " + status.message);
        }

        private void GpsStart()
        {
            if (watcher == null)
            {
                ui.Button = "Stop";
                notifyIcon.BalloonTipText = "Started";
                MenuState(false);

                ui.LogAdd("Starting");
                watcher = new GeoCoordinateWatcher();
                watcher.PositionChanged +=
                    new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(OnGpsPos);
                watcher.StatusChanged +=
                    new EventHandler<GeoPositionStatusChangedEventArgs>(OnGpsStatus);
                watcher.Start();
            }
        }

        private void GpsStop()
        {
            if (watcher != null)
            {
                ui.LogAdd("Stopping");
                watcher.Stop();
                watcher = null;

                ui.Button = "Start";
                notifyIcon.BalloonTipText = "Stopped";
                MenuState(false);
            }
        }

        private void MinimiseToTray()
        {
            WindowState = WindowState.Minimized;
            notifyIcon.ShowBalloonTip(2000);
            Hide();
        }

        private void MenuState(bool enabled)
        {
            contextMenuStrip.Items[2].Enabled = enabled;
            contextMenuStrip.Items[3].Enabled = !enabled;
        }
    }
}
