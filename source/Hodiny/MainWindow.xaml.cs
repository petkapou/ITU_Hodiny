using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;

namespace Hodiny
{
    public enum ClockTypes { Analog, Digital, Sun };

    public partial class MainWindow : Window
    {
        private Color bgColor = new Color();
        private List<String> locales = new List<String>();

        private List<Window> clockObjects = new List<Window>();
        ClockTypes previous = ClockTypes.Digital;

        public MainWindow()
        {
            InitializeComponent();

            clockObjects.Add(new Analog(bgColor, locales, setClock));
            clockObjects.Add(new Digital(bgColor, locales, setClock));
            clockObjects.Add(new Sun(bgColor, locales, setClock));

            bgColor = Color.FromArgb(255, 255, 255, 255);

            this.Hide();
            setClock(ClockTypes.Sun);
        }

        public void setClock(ClockTypes type)
        {
            clockObjects[(int)previous].Hide();
            clockObjects[(int)type].Top = clockObjects[(int)previous].Top;
            clockObjects[(int)type].Left = clockObjects[(int)previous].Left;
            clockObjects[(int)type].Show();
            previous = type;
        }
    }
}
