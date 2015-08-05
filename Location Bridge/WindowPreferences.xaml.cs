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

using System.Windows;

namespace Location_Bridge
{
    partial class WindowPreferences : Window
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
