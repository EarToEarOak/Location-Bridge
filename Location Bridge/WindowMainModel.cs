using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Location_Bridge
{

    public class WindowMainModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double lat = double.NaN;
        private double lon = double.NaN;
        private double alt = double.NaN;
        private double speed = double.NaN;
        private DateTimeOffset time = DateTimeOffset.MinValue;
        private int clients;
        private string log = "";
        private string button;
        private bool buttonEnabled = true;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                  new PropertyChangedEventArgs(propertyName));
        }

        public double Latitude
        {
            get { return lat; }
            set { lat = value; RaisePropertyChanged("Latitude"); }
        }

        public double Longitude
        {
            get { return lon; }
            set { lon = value; RaisePropertyChanged("Longitude"); }
        }

        public double Altitude
        {
            get { return alt; }
            set { alt = value; RaisePropertyChanged("Altitude"); }
        }

        public double Speed
        {
            get { return speed; }
            set { speed = value; RaisePropertyChanged("Speed"); }
        }

        public DateTimeOffset Time
        {
            get { return time; }
            set { time = value; RaisePropertyChanged("Time"); }
        }

        public void LocationClear()
        {
            lat = double.NaN;
            lon = double.NaN;
            alt = double.NaN;
            speed = double.NaN;
            time = DateTimeOffset.MinValue;

            string[] properties = { "Latitude", "Longitude", "Altitude", "Speed", "Time" };
            foreach (var property in properties)
                RaisePropertyChanged(property);
        }

        public int Clients
        {
            get { return clients; }
            set { clients = value; RaisePropertyChanged("Clients"); }
        }

        public string Log
        {
            get { return log; }
            set { log = value; RaisePropertyChanged("Log"); }
        }

        public void LogAdd(string line)
        {
            var lines = new List<string>(log.Split('\n'));
            while (lines.Count > 100)
                lines.RemoveAt(0);

            var time = String.Format("{0:T}", DateTime.Now);
            var logLine = time + " - " + line;
            lines.Add(logLine);

            Log = String.Join("\n", lines);
        }

        public void LogClear()
        {
            Log = "";
        }

        public string Button
        {
            get { return button; }
            set { button = value; RaisePropertyChanged("Button"); }
        }

        public bool ButtonEnabled
        {
            get { return buttonEnabled; }
            set { buttonEnabled = value; RaisePropertyChanged("ButtonEnabled"); }
        }
    }

    public class CoordToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if (Double.IsNaN((double)value))
                return "";
            return String.Format("{0:F6} °", value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DistToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if (Double.IsNaN((double)value))
                return "";
            return String.Format("{0:F1} m", value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SpeedToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if (Double.IsNaN((double)value))
                return "";
            return String.Format("{0:F1} m/s", value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if ((DateTimeOffset)value == DateTimeOffset.MinValue)
                return "";
            return String.Format("{0:T}",
                ((DateTimeOffset)value).ToLocalTime());
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ClientsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            return String.Format("{0}/{1}", value, Server.MAX_CLIENTS);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}