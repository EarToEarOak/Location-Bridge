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
