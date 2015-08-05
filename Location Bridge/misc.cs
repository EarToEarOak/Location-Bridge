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
using System.Windows.Controls;

namespace Location_Bridge
{
    static class TextBoxUtils
    {

        public static readonly DependencyProperty pAutoScroll =
            DependencyProperty.RegisterAttached("AutoScroll",
                typeof(bool), typeof(TextBoxUtils),
                new PropertyMetadata(false, AutoScrollChanged));

        public static bool GetAutoScroll(TextBox textBox)
        {
            return (bool)textBox.GetValue(pAutoScroll);
        }

        public static void SetAutoScroll(TextBox textBox, bool autoScroll)
        {
            textBox.SetValue(pAutoScroll, autoScroll);
        }


        private static void AutoScrollChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.NewValue != null && (bool)e.NewValue)
            {
                textBox.TextChanged += OnTextChanged;
                textBox.ScrollToEnd();
            }
            else
                textBox.TextChanged -= OnTextChanged;
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }
    }
}
