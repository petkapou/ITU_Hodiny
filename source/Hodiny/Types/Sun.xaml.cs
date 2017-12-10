using System;
using System.IO;
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

namespace Hodiny
{
    public partial class Sun : Window
    {
        string activeBoard = "ShowMenu";
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        private Color bgColor;

        private Action<ClockTypes> mCallback;
        private List<String> locales = new List<String>();

        public Sun(Color bgColor, List<String> locale, Action<ClockTypes> callback)
        {
            InitializeComponent();

            timer.Elapsed += new System.Timers.ElapsedEventHandler(timerEvent);
            timer.Enabled = true;

            this.bgColor = bgColor;
            locales = locale;
            mCallback = callback;
        }

        void timerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 21)
                {
                    shadow.Visibility = Visibility.Visible;

                    double time = ((DateTime.Now.Hour - 6) * 60 + DateTime.Now.Minute) / 5;
                    //double time = DateTime.Now.Second * 9;

                    double halfWidth = ActualWidth / 2;
                    double halfHeight = ActualHeight / 2;

                    double x = Math.Cos((Math.PI / 180) * (time + 180)) * halfWidth + halfWidth;
                    double y = Math.Sin((Math.PI / 180) * time) * halfHeight + halfHeight;
                    y -= Math.Sin((Math.PI / 180) * time) * halfHeight * 0.5;

                    shadow.Points.Clear();
                    shadow.Points.Add(new Point(halfWidth - 10, halfHeight - 5));
                    shadow.Points.Add(new Point(halfWidth + 10, halfHeight - 5));
                    shadow.Points.Add(new Point(x, y));
                }
                else
                    shadow.Visibility = Visibility.Hidden;
            }
            ));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_Style.SelectionChanged -= ComboBox_Style_SelectionChanged;
            ComboBox_Style.SelectedIndex = 2;
            ComboBox_Style.SelectionChanged += ComboBox_Style_SelectionChanged;

            AddLanguages();

            Slider_Transparency.Value = 1;
            ComboBox_Lang.SelectedIndex = 0;
            //ComboBox_Bg_Type.SelectedIndex = 0;
            //ComboBox_Hand.SelectedIndex = 0;
            //Slider_Lenght.Value = 0.8;

            bgColor.A = 255;
            Slider_Red.Value = bgColor.R;
            Slider_Green.Value = bgColor.G;
            Slider_Blue.Value = bgColor.B;

            //Slider_Font_Size.Value = 12;

            //Slider_Font_Red_AD.Value = 255;
            //Slider_Font_Green_AD.Value = 255;
            //Slider_Font_Blue_AD.Value = 255;
        }

        void AddLanguages()
        {
            String directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            directory = System.IO.Path.Combine(directory, "Localization");
            DirectoryInfo d = new DirectoryInfo(directory);
            String filepath;
            foreach (var file in d.GetFiles("*.xaml"))
            {
                filepath = System.IO.Path.Combine(directory, file.Name);
                var languageDictionary = new ResourceDictionary();
                languageDictionary.Source = new Uri(filepath);
                if (languageDictionary.Contains("LanguageName") && languageDictionary.Contains("ResourceDictionaryName"))
                {
                    ComboBox_Lang.Items.Add(languageDictionary["LanguageName"].ToString());
                    locales.Add(languageDictionary["ResourceDictionaryName"].ToString().Replace("Loc-", ""));
                }

            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activeBoard == "HideMenu") activeBoard = "ShowMenu";
            else activeBoard = "HideMenu";
            Storyboard sb = Resources[activeBoard] as Storyboard;
            sb.Begin(pnlLeftMenu);
        }

        private void TabItem_Exit_Clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ComboBox_Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Instance.SwitchLanguage(locales[ComboBox_Lang.SelectedIndex]);
        }

        private void ComboBox_Style_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_Style.SelectedIndex)
            {
                case 0:
                    mCallback(ClockTypes.Analog);
                    break;

                case 1:
                    mCallback(ClockTypes.Digital);
                    break;

                case 2:
                    mCallback(ClockTypes.Sun);
                    break;

                default:
                    break;
            }
            ComboBox_Style.SelectionChanged -= ComboBox_Style_SelectionChanged;
            ComboBox_Style.SelectedIndex = 2;
            ComboBox_Style.SelectionChanged += ComboBox_Style_SelectionChanged;
        }

        private void Slider_Transparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Transparency.Value = Math.Round(Slider_Transparency.Value * 100) / 100;
            if (Slider_Transparency.Value < 0.05)
            {
                Slider_Transparency.Value = 0.05;
            }
        }

        private void Slider_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Red.Value = Math.Round(Slider_Red.Value);
            bgColor.R = Convert.ToByte(Math.Round(Slider_Red.Value));
            SetColor();
        }

        private void Slider_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Green.Value = Math.Round(Slider_Green.Value);
            bgColor.G = Convert.ToByte(Math.Round(Slider_Green.Value));
            SetColor();
        }

        private void Slider_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Blue.Value = Math.Round(Slider_Blue.Value);
            bgColor.B = Convert.ToByte(Math.Round(Slider_Blue.Value));
            SetColor();
        }

        private void SetColor()
        {
            this.SetBg(bgColor);
        }

        public void SetBg(Color color)
        {
            Sun_Rectangle_Bg.Fill = new SolidColorBrush(color);
        }

        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            activeBoard = "HideMenu";
            Storyboard sb = Resources[activeBoard] as Storyboard;
            sb.Begin(pnlLeftMenu);
        }
    }
}
