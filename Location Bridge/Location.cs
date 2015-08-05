/*
 Location Bridge

 http://eartoearoak.com/software/location-bridge

 Copyright 2014-2015 Al Brown

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

namespace Location_Bridge
{
    class Location
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public double alt { get; set; }
        public double speed { get; set; }
        public double ha { get; set; }
        public double va { get; set; }
        public DateTimeOffset time { get; set; }

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
}
