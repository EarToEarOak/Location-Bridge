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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Location_Bridge
{
    public partial class WidgetLed : UserControl
    {

        public static DependencyProperty pColour = DependencyProperty.Register(
            "Colour", typeof(object), typeof(WidgetLed));
        public static DependencyProperty pBlur = DependencyProperty.Register(
            "Blur", typeof(object), typeof(WidgetLed));

        private DispatcherTimer timer = new DispatcherTimer();

        
        private Color colourOff;
        private Color colourOn;
        private SolidColorBrush blurOff;
        private SolidColorBrush blurOn;

        public WidgetLed()
        {
            colourOff = Colors.Gray;
            colourOn = Colors.Green;
            blurOff = new SolidColorBrush(Color.FromArgb(255,255,255,255));
            blurOn = new SolidColorBrush(colourOn);

            Colour = colourOff;
            Blur = blurOff;
            InitializeComponent();

            timer.Tick += new EventHandler(OnTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
        }

        public Color Colour
        {
            get { return (Color)GetValue(pColour); }
            set { SetValue(pColour, value); }
        }

        public SolidColorBrush Blur
        {
            get { return (SolidColorBrush)GetValue(pBlur); }
            set { SetValue(pBlur, value); }
        }

        public void On()
        {
            SetValue(pColour, colourOn);
            SetValue(pBlur, blurOn);
        }

        public void Off()
        {
            SetValue(pColour, colourOff);
            SetValue(pBlur, blurOff);
        }

        public void Flash()
        {
            if (!timer.IsEnabled)
            {
                On();
                timer.Start();
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            timer.Stop();
            Off();
        }
    }
}
