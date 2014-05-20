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
