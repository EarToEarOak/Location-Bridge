using System;
using System.Windows;

namespace Location_Bridge
{
    public partial class App : Application
    {
    }

    public class Location
    {
        public double lat;
        public double lon;
        public double alt;
        public double speed;
        public double ha, va;
        public DateTimeOffset time;

        public Location()
        {
        }

        public Location(Location original)
        {
            lat = original.lat;
            lon = original.lon;
            alt = original.alt;
            speed = original.speed;
            ha = original.ha;
            va = original.va;
            time = original.time;
        }
    }

    public enum ServerStatusCode { OK, ERROR, CONN };

    public class ServerStatus
    {
        public ServerStatusCode status { get; set; }
        public string message { get; set; }
        public int value { get; set; }

        public ServerStatus(string message)
        {
            this.status = ServerStatusCode.OK;
            this.message = message;
            this.value = 0;
        }

        public ServerStatus(ServerStatusCode status, string message)
        {
            this.status = status;
            this.message = message;
            this.value = 0;
        }

        public ServerStatus(ServerStatusCode status, int value)
        {
            this.status = status;
            this.message = null;
            this.value = value;
        }
    }

    delegate void CallbackUI(ServerStatus status);
}
