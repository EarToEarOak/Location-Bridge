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
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle("Location Bridge")]
[assembly: AssemblyDescription("NMEA server for Windows location sensors")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Location Bridge")]
[assembly: AssemblyCopyright("© 2014 Al Brown")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly 
)]

[assembly: AssemblyVersion("1.1.0.*")]

[assembly: CLSCompliant(true)]
