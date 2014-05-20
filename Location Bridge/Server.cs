/*
 Location Bridge

 http://eartoearoak.com/software/location-bridge

 Copyright 2014 Al Brown

 A simple server that provides NMEA sentences over TCP from the
 Windows location sensors.


 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, or (at your option)
 any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Location_Bridge
{

    // Based on http://msdn.microsoft.com/en-us/library/fx6588te.aspx

    public class Client
    {
        public Socket socket = null;

        public Client(Socket socket)
        {
            this.socket = socket;
        }
    }

    class Server
    {
        public const int MAX_CLIENTS = 10;

        private static byte[] warnMaxClients  = Encoding.ASCII.GetBytes(
            "Too many connections");

        private object lockGps;
        private CallbackUI callback;

        private volatile bool cancel = false;
        private Object lockClients = new Object();
        private List<Client> clients = new List<Client>();

        public static ManualResetEvent signal = new ManualResetEvent(false);

        public Server(object gpsLock, CallbackUI callback)
        {
            lockGps = gpsLock;
            this.callback = callback;
            UpdateCount();
        }

        public void Start()
        {
            IPEndPoint localEndPoint;
            if (Properties.Settings.Default.LocalOnly)
                localEndPoint = new IPEndPoint(IPAddress.Loopback, 10110);
            else
                localEndPoint = new IPEndPoint(IPAddress.Any, 10110);

            var socketServer = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream, ProtocolType.Tcp);
            callback(new ServerStatus("waiting"));

            try
            {
                socketServer.Bind(localEndPoint);
                socketServer.Listen(100);

                while (!cancel)
                {
                    signal.Reset();
                    socketServer.BeginAccept(new AsyncCallback(ClientCallback),
                        socketServer);
                    signal.WaitOne();
                }
            }
            catch (SocketException e)
            {
                callback(new ServerStatus(ServerStatusCode.ERROR,
                    String.Format("error {0}", e.SocketErrorCode.ToString())));
                if (socketServer.IsBound)
                    socketServer.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                socketServer.Close();
                foreach (var client in clients.ToArray())
                    ClientRemove(client);
            }
        }

        private string GetHostName(string address)
        {
            string host = address.Split(':')[0];
            string hostName = "";
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(host);

                hostName = hostEntry.HostName;
            }
            catch (SocketException)
            {
                hostName = host;
            }

            return "'" + hostName + "'";

        }

        private void ClientAdd(Client client)
        {
            lock (lockClients)
                clients.Add(client);
        }

        private void ClientRemove(Client client)
        {
            lock (lockClients)
                clients.Remove(client);
            var addr = GetHostName(client.socket.RemoteEndPoint.ToString());
            client.socket.Shutdown(SocketShutdown.Both);
            client.socket.Close();
            callback(new ServerStatus(String.Format("{0} disconnected", addr)));
            UpdateCount();
        }

        private void ClientCallback(IAsyncResult ar)
        {
            signal.Set();

            try
            {
                var socketServer = (Socket)ar.AsyncState;
                var socketClient = socketServer.EndAccept(ar);
                var addr = GetHostName(socketClient.RemoteEndPoint.ToString());

                if (clients.Count < MAX_CLIENTS)
                {
                    var client = new Client(socketClient);
                    ClientAdd(client);
                    callback(new ServerStatus(
                        String.Format("{0} connected", addr)));
                    UpdateCount();
                }
                else{
                    socketClient.Send(warnMaxClients);
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                    callback(new ServerStatus(
                        String.Format("{0} refused (too many connections)", addr)));
                }
            }
            catch (ObjectDisposedException) { }
        }

        private void Send(Client client, String data)
        {
            var byteData = Encoding.ASCII.GetBytes(data);
            try
            {
                client.socket.BeginSend(byteData, 0, byteData.Length, 0,
                                        new AsyncCallback(SendCallback), client);
            }
            catch (SocketException)
            {
                ClientRemove(client);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            var client = (Client)ar.AsyncState;
            try
            {
                client.socket.EndSend(ar);
            }
            catch (SocketException)
            {
                ClientRemove(client);
            }
        }

        private string NmeaCoord(double coord, bool isLat)
        {

            var direct = "";
            var format = "";

            if (isLat)
            {
                if (coord < 0)
                    direct = "S";
                else
                    direct = "N";
                format = "{0:D2}{1,7:F4},{2}";
            }
            else
            {
                if (coord < 0)
                    direct = "W";
                else
                    direct = "E";
                format = "{0:D3}{1,7:F4},{2}";
            }

            coord = Math.Abs(coord);
            var degs = (int)coord;
            var mins = (coord - degs) * 60.0;

            var nmea = String.Format(format, degs, mins, direct);
            return nmea;
        }

        private string NmeaChecksum(string sentence)
        {
            var checksum = 0;

            for (var i = 0; i < sentence.Length; i++)
                checksum ^= sentence[i];

            return "*" + checksum.ToString("X2");

        }

        private string NmeaFormat(Location location)
        {
            var lat = NmeaCoord(location.lat, true);
            var lon = NmeaCoord(location.lon, false);
            var alt = location.alt.ToString("F1");
            var speed = (location.speed * 1.94384449).ToString("F3");
            var time = location.time.ToString("HHmmss.ff");
            var date = location.time.ToString("ddMMyy");

            var gga = String.Format("GPGGA,{0},{1},{2},1,12,,{3},M,,M,,,",
                time, lat, lon, alt);
            var gll = String.Format("GPGLL,{0},{1},{2},V",
                lat, lon, time, alt);
            var rmc = String.Format("GPRMC,{0},A,{1},{2},{3},0,{4},0,0,A",
                time, lat, lon, speed, date);

            gga = "$" + gga + NmeaChecksum(gga);
            gll = "$" + gll + NmeaChecksum(gll);
            rmc = "$" + rmc + NmeaChecksum(rmc);

            return gga + "\n\r"
                + gll + "\n\r"
                + rmc + "\n\r";
        }

        private void UpdateCount()
        {
            callback(new ServerStatus(ServerStatusCode.CONN,
                clients.Count));
        }

        public void GpsUpdate(Location location)
        {
            lock (lockGps)
                location = new Location(location);

            lock (lockClients)
                foreach (var client in clients.ToArray())
                    Send(client, NmeaFormat(location));
        }

        public void Stop()
        {
            cancel = true;
            signal.Set();
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
