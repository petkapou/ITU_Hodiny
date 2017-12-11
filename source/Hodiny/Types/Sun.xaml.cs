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
        private Color strokeColor = new Color();

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


            strokeColor = (stroke_Polygon.Fill as SolidColorBrush).Color;
            Slider_Stroke_Red.Value = strokeColor.R;
            Slider_Stroke_Green.Value = strokeColor.G;
            Slider_Stroke_Blue.Value = strokeColor.B;
        }

        void timerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 21)
                {
                    shadow1.Visibility = Visibility.Visible;
                    shadow2.Visibility = Visibility.Visible;

                    double time = ((DateTime.Now.Hour - 6) * 60 + DateTime.Now.Minute) / 5;
                    //double time = DateTime.Now.Second * 9;

                    double halfWidth = ActualWidth / 2;
                    double halfHeight = ActualHeight / 2;

                    double x = Math.Cos((Math.PI / 180) * (time + 180)) * halfWidth + halfWidth;
                    double y = Math.Sin((Math.PI / 180) * time) * halfHeight + halfHeight;
                    y -= Math.Sin((Math.PI / 180) * time) * halfHeight * 0.5;

                    shadow1.Points.Clear();
                    shadow1.Points.Add(new Point(halfWidth - 10, halfHeight - 5));
                    shadow1.Points.Add(new Point(halfWidth, halfHeight + 5));
                    shadow1.Points.Add(new Point(x, y));

                    shadow2.Points.Clear();
                    shadow2.Points.Add(new Point(halfWidth, halfHeight + 5));
                    shadow2.Points.Add(new Point(halfWidth + 10, halfHeight - 5));
                    shadow2.Points.Add(new Point(x, y));
                }
                else
                {
                    shadow1.Visibility = Visibility.Hidden;
                    shadow2.Visibility = Visibility.Hidden;
                }
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

            AddThemes();

            ComboBox_Bg_Type.SelectedIndex = 2;
            ComboBox_Theme.SelectedIndex = 0;
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

        void AddThemes()
        {
            String directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            directory = System.IO.Path.Combine(directory, "Themes", "Sun");
            DirectoryInfo d = new DirectoryInfo(directory);
            String filepath;
            foreach (var file in d.GetFiles("*.xaml"))
            {
                filepath = System.IO.Path.Combine(directory, file.Name);
                var languageDictionary = new ResourceDictionary();
                languageDictionary.Source = new Uri(filepath);
                if (languageDictionary.Contains("ThemeName"))
                {
                    ComboBox_Theme.Items.Add(languageDictionary["ThemeName"].ToString());
                }                
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_File_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "Image files (*.jpg, *.jpeg, *.gif, *.png) | *.jpg; *.jpeg; *.gif; *.png"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                TextBox_File.Text = filename;
                this.Sun_Rectangle_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
            }
        }

        private void ComboBox_Bg_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_Bg_Type.SelectedIndex)
            {
                case 0:
                    Label_Red.IsEnabled = true;
                    Label_Red.Visibility = Visibility.Visible;
                    Slider_Red.IsEnabled = true;
                    Slider_Red.Visibility = Visibility.Visible;
                    TextBox_Red.IsEnabled = true;
                    TextBox_Red.Visibility = Visibility.Visible;

                    Label_Green.IsEnabled = true;
                    Label_Green.Visibility = Visibility.Visible;
                    Slider_Green.IsEnabled = true;
                    Slider_Green.Visibility = Visibility.Visible;
                    TextBox_Green.IsEnabled = true;
                    TextBox_Green.Visibility = Visibility.Visible;

                    Label_Blue.IsEnabled = true;
                    Label_Blue.Visibility = Visibility.Visible;
                    Slider_Blue.IsEnabled = true;
                    Slider_Blue.Visibility = Visibility.Visible;
                    TextBox_Blue.IsEnabled = true;
                    TextBox_Blue.Visibility = Visibility.Visible;

                    TextBox_File.IsEnabled = false;
                    TextBox_File.Visibility = Visibility.Hidden;
                    Button_File.IsEnabled = false;
                    Button_File.Visibility = Visibility.Hidden;

                    ComboBox_Theme.IsEnabled = false;
                    ComboBox_Theme.Visibility = Visibility.Hidden;
                    SetBg(bgColor);
                    break;

                case 1:
                    Label_Red.IsEnabled = false;
                    Label_Red.Visibility = Visibility.Hidden;
                    Slider_Red.IsEnabled = false;
                    Slider_Red.Visibility = Visibility.Hidden;
                    TextBox_Red.IsEnabled = false;
                    TextBox_Red.Visibility = Visibility.Hidden;

                    Label_Green.IsEnabled = false;
                    Label_Green.Visibility = Visibility.Hidden;
                    Slider_Green.IsEnabled = false;
                    Slider_Green.Visibility = Visibility.Hidden;
                    TextBox_Green.IsEnabled = false;
                    TextBox_Green.Visibility = Visibility.Hidden;

                    Label_Blue.IsEnabled = false;
                    Label_Blue.Visibility = Visibility.Hidden;
                    Slider_Blue.IsEnabled = false;
                    Slider_Blue.Visibility = Visibility.Hidden;
                    TextBox_Blue.IsEnabled = false;
                    TextBox_Blue.Visibility = Visibility.Hidden;

                    TextBox_File.IsEnabled = true;
                    TextBox_File.Visibility = Visibility.Visible;
                    Button_File.IsEnabled = true;
                    Button_File.Visibility = Visibility.Visible;

                    ComboBox_Theme.IsEnabled = false;
                    ComboBox_Theme.Visibility = Visibility.Hidden;
                    break;

                case 2:
                    Label_Red.IsEnabled = false;
                    Label_Red.Visibility = Visibility.Hidden;
                    Slider_Red.IsEnabled = false;
                    Slider_Red.Visibility = Visibility.Hidden;
                    TextBox_Red.IsEnabled = false;
                    TextBox_Red.Visibility = Visibility.Hidden;

                    Label_Green.IsEnabled = false;
                    Label_Green.Visibility = Visibility.Hidden;
                    Slider_Green.IsEnabled = false;
                    Slider_Green.Visibility = Visibility.Hidden;
                    TextBox_Green.IsEnabled = false;
                    TextBox_Green.Visibility = Visibility.Hidden;

                    Label_Blue.IsEnabled = false;
                    Label_Blue.Visibility = Visibility.Hidden;
                    Slider_Blue.IsEnabled = false;
                    Slider_Blue.Visibility = Visibility.Hidden;
                    TextBox_Blue.IsEnabled = false;
                    TextBox_Blue.Visibility = Visibility.Hidden;

                    TextBox_File.IsEnabled = false;
                    TextBox_File.Visibility = Visibility.Hidden;
                    Button_File.IsEnabled = false;
                    Button_File.Visibility = Visibility.Hidden;

                    ComboBox_Theme.IsEnabled = true;
                    ComboBox_Theme.Visibility = Visibility.Visible;
                    int tmp = ComboBox_Theme.SelectedIndex;
                    ComboBox_Theme.SelectedIndex = -1;
                    ComboBox_Theme.SelectedIndex = tmp;
                    break;

                default:
                    break;
            }
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


        private void Slider_Stroke_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Stroke_Red.Value = Math.Round(Slider_Stroke_Red.Value);
            strokeColor.R = (byte)Slider_Stroke_Red.Value;
            Set_Stroke_Color();
        }

        private void Slider_Stroke_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Stroke_Green.Value = Math.Round(Slider_Stroke_Green.Value);
            strokeColor.G = (byte)Slider_Stroke_Green.Value;
            Set_Stroke_Color();
        }

        private void Slider_Stroke_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Stroke_Blue.Value = Math.Round(Slider_Stroke_Blue.Value);
            strokeColor.B = (byte)Slider_Stroke_Blue.Value;
            Set_Stroke_Color();
        }

        private void Set_Stroke_Color()
        {
            stroke_Polygon.Fill = new SolidColorBrush(strokeColor);
            stroke_Polygon.Stroke = new SolidColorBrush(strokeColor);
        }

        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            activeBoard = "HideMenu";
            Storyboard sb = Resources[activeBoard] as Storyboard;
            sb.Begin(pnlLeftMenu);
        }

        private void ComboBox_Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Theme.SelectedIndex == -1)
                return;
            String directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            directory = System.IO.Path.Combine(directory, "Themes", "Sun");
            DirectoryInfo d = new DirectoryInfo(directory);
            String filepath;
            String imagepath;
            foreach (var file in d.GetFiles("*.xaml"))
            {
                filepath = System.IO.Path.Combine(directory, file.Name);
                var languageDictionary = new ResourceDictionary();
                languageDictionary.Source = new Uri(filepath);
                if (languageDictionary.Contains("ThemeName"))
                {
                    if (languageDictionary["ThemeName"].ToString() == ComboBox_Theme.SelectedItem.ToString())
                    {
                        switch (languageDictionary["Type"].ToString())
                        {
                            case "Image":
                                imagepath = System.IO.Path.Combine(directory, languageDictionary["ImageFileName"].ToString());
                                this.Sun_Rectangle_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(imagepath)));
                                break;
                            case "Color":
                                bgColor.R = byte.Parse(languageDictionary["BgColorR"].ToString());
                                bgColor.G = byte.Parse(languageDictionary["BgColorG"].ToString());
                                bgColor.B = byte.Parse(languageDictionary["BgColorB"].ToString());
                                Slider_Red.Value = bgColor.R;
                                Slider_Green.Value = bgColor.G;
                                Slider_Blue.Value = bgColor.B;
                                SetBg(bgColor);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
