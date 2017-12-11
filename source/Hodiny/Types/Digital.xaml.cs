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
    public partial class Digital : Window
    {
        string activeBoard = "ShowMenu";
        private Color bgColor;
        private Color fontColorTime = new Color();
        private Color fontColorDate = new Color();
        System.Timers.Timer timer = new System.Timers.Timer(1000);

        private Action<ClockTypes> mCallback;
        private List<String> locales = new List<String>();

        public Digital(Color bgColor, List<String> locale, Action<ClockTypes> callback)
        {
            InitializeComponent();

            fontColorDate = Color.FromArgb(255, 255, 255, 255);
            fontColorTime = Color.FromArgb(255, 0, 0, 0);

            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            timer.Enabled = true;
            this.bgColor = bgColor;
            locales = locale;
            mCallback = callback;
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
            directory = System.IO.Path.Combine(directory, "Themes", "Digital");
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

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                Actualize_Time_Date(sender);
            }
            ));
        }

        private void Actualize_Date(object sender)
        {
            if (Digital_Date == null)
            {
                return;
            }
            switch (ComboBox_Date_Format.SelectedIndex)
            {
                case 0:
                    Digital_Date.Text = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
                    break;
                case 1:
                    Digital_Date.Text = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                    break;
                case 2:
                    Digital_Date.Text = DateTime.Now.Month + "." + DateTime.Now.Day + "." + DateTime.Now.Year;
                    break;
                case 3:
                    Digital_Date.Text = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year;
                    break;
                case 4:
                    Digital_Date.Text = DateTime.Now.Day + "." + DateTime.Now.Month;
                    break;
                case 5:
                    Digital_Date.Text = DateTime.Now.Day + "/" + DateTime.Now.Month;
                    break;
                case 6:
                    Digital_Date.Text = DateTime.Now.Month + "." + DateTime.Now.Day;
                    break;
                case 7:
                    Digital_Date.Text = DateTime.Now.Month + "/" + DateTime.Now.Day;
                    break;
                default:
                    break;
            }
        }

        private void Actualize_Date(object sender, RoutedEventArgs e)
        {
            Actualize_Date(sender);
        }

        private void Actualize_Time_Date(object sender)
        {
            if (Digital_Time == null || Digital_Date == null || ComboBox_Time_Format == null)
            {
                return;
            }
            string tmp_time = "";
            bool pm_active = false;
            switch (ComboBox_Time_Format.SelectedIndex)
            {
                case 0: //12hodin
                    int oclock = DateTime.Now.Hour;
                    if (oclock > 12)
                    {
                        oclock -= 12;
                        pm_active = true;
                    }
                    else if (oclock == 0)
                    {
                        oclock = 12;
                    }
                    tmp_time += oclock + ":";
                    break;
                case 1: //24hodin
                    tmp_time += DateTime.Now.Hour + ":";
                    break;
                default:
                    break;
            }
            if (DateTime.Now.Minute < 10)
            {
                tmp_time += "0";
            }
            tmp_time += DateTime.Now.Minute;
            if (CheckBox_Show_Sec.IsChecked == true)
            {
                tmp_time += ":";
                if (DateTime.Now.Second < 10)
                {
                    tmp_time += "0";
                }
                tmp_time += DateTime.Now.Second;
            }
            switch (ComboBox_Time_Format.SelectedIndex)
            {
                case 0: //12hodin
                    if (pm_active)
                    {
                        tmp_time += " pm";
                    }
                    else
                    {
                        tmp_time += " am";
                    }
                    break;
                default:
                    break;
            }
            Digital_Time.Text = tmp_time;
            Actualize_Date(sender);
        }

        private void Actualize_Time_Date(object sender, RoutedEventArgs e)
        {
            Actualize_Time_Date(sender);
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddLanguages();

            ComboBox_Style.SelectionChanged -= ComboBox_Style_SelectionChanged;
            ComboBox_Style.SelectedIndex = 1;
            ComboBox_Style.SelectionChanged += ComboBox_Style_SelectionChanged;

            Slider_Transparency.Value = 1;
            ComboBox_Lang.SelectedIndex = 0;
            ComboBox_Bg_Type.SelectedIndex = 0;
            
            Slider_Red.Value = bgColor.R;
            Slider_Green.Value = bgColor.G;
            Slider_Blue.Value = bgColor.B;

            fontColorTime.A = 255;
            Slider_Font_Red_DT.Value = fontColorTime.R;
            Slider_Font_Green_DT.Value = fontColorTime.G;
            Slider_Font_Blue_DT.Value = fontColorTime.B;
            Digital_Time.FontSize = 50;
            Slider_Font_Size_DT.Value = 50;

            fontColorDate.A = 255;
            Slider_Font_Red_DD.Value = fontColorDate.R;
            Slider_Font_Green_DD.Value = fontColorDate.G;
            Slider_Font_Blue_DD.Value = fontColorDate.B;
            Digital_Date.FontSize = 15;
            Slider_Font_Size_DD.Value = 15;

            CalculateTimeDatePosition();

            FillFontComboBox();

            AddThemes();

            ComboBox_Bg_Type.SelectedIndex = 2;
            ComboBox_Theme.SelectedIndex = 0;
        }

        private void FillFontComboBox()
        {
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                ComobBox_Fonts_DT.Items.Add(fontFamily.Source);
            }
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                ComobBox_Fonts_DD.Items.Add(fontFamily.Source);
            }

            ComobBox_Fonts_DD.SelectedIndex = 0;
            ComobBox_Fonts_DT.SelectedIndex = 0;
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
            ComboBox_Style.SelectedIndex = 1;
            ComboBox_Style.SelectionChanged += ComboBox_Style_SelectionChanged;
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
                this.Rectangle_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
            }
        }

        private void CheckBox_Italic_Checked_DT(object sender, RoutedEventArgs e)
        {
            Digital_Time.FontStyle = FontStyles.Italic;
        }

        private void CheckBox_Bold_Checked_DT(object sender, RoutedEventArgs e)
        {
            Digital_Time.FontWeight = FontWeights.Bold;
        }

        private void CheckBox_Italic_Unchecked_DT(object sender, RoutedEventArgs e)
        {
            Digital_Time.FontStyle = FontStyles.Normal;
        }

        private void CheckBox_Bold_Unchecked_DT(object sender, RoutedEventArgs e)
        {
            Digital_Time.FontWeight = FontWeights.Normal;
        }

        private void ComboBox_SelectionChanged_DT(object sender, SelectionChangedEventArgs e)
        {
            Digital_Time.FontFamily = new FontFamily(ComobBox_Fonts_DT.Text);
        }

        private void Slider_Font_Size_ValueChanged_DT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Size_DT.Value = Math.Round(Slider_Font_Size_DT.Value);
            if (this.IsLoaded)
            {
                Digital_Time.FontSize = Math.Round(Slider_Font_Size_DT.Value);
            }
        }

        private void Slider_Font_Red_ValueChanged_DT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Red_DT.Value = Math.Round(Slider_Font_Red_DT.Value);
            fontColorTime.R = Convert.ToByte(Math.Round(Slider_Font_Red_DT.Value));
            SetFontColor_DT();
        }

        private void Slider_Font_Green_ValueChanged_DT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green_DT.Value = Math.Round(Slider_Font_Green_DT.Value);
            fontColorTime.G = Convert.ToByte(Math.Round(Slider_Font_Green_DT.Value));
            SetFontColor_DT();
        }

        private void Slider_Font_Blue_ValueChanged_DT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue_DT.Value = Math.Round(Slider_Font_Blue_DT.Value);
            fontColorTime.B = Convert.ToByte(Math.Round(Slider_Font_Blue_DT.Value));
            SetFontColor_DT();
        }

        private void SetFontColor_DT()
        {
            this.SetFontColor_DT(fontColorTime);
        }

        public void SetFontColor_DT(Color color)
        {
            Digital_Time.Foreground = new SolidColorBrush(color);
        }

        // DIGITAL DATE FONTS

        private void CheckBox_DD_Checked(object sender, RoutedEventArgs e)
        {
            if (Digital_Date != null)
            {
                Digital_Date.Visibility = Visibility.Visible;
            }
        }

        private void CheckBox_DD_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Digital_Date != null)
            {
                Digital_Date.Visibility = Visibility.Hidden;
            }
        }

        private void CheckBox_Italic_Checked_DD(object sender, RoutedEventArgs e)
        {
            Digital_Date.FontStyle = FontStyles.Italic;
        }

        private void CheckBox_Bold_Checked_DD(object sender, RoutedEventArgs e)
        {
            Digital_Date.FontWeight = FontWeights.Bold;
        }

        private void CheckBox_Italic_Unchecked_DD(object sender, RoutedEventArgs e)
        {
            Digital_Date.FontStyle = FontStyles.Normal;
        }

        private void CheckBox_Bold_Unchecked_DD(object sender, RoutedEventArgs e)
        {
            Digital_Date.FontWeight = FontWeights.Normal;
        }

        private void ComboBox_SelectionChanged_DD(object sender, SelectionChangedEventArgs e)
        {
            Digital_Date.FontFamily = new FontFamily(ComobBox_Fonts_DD.Text);
        }

        private void Slider_Font_Size_ValueChanged_DD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Size_DD.Value = Math.Round(Slider_Font_Size_DD.Value);
            if (this.IsLoaded)
            {
                Digital_Date.FontSize = Math.Round(Slider_Font_Size_DD.Value);
            }
        }

        private void Slider_Font_Red_ValueChanged_DD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Red_DD.Value = Math.Round(Slider_Font_Red_DD.Value);
            fontColorDate.R = Convert.ToByte(Math.Round(Slider_Font_Red_DD.Value));
            SetFontColor_DD();
        }

        private void Slider_Font_Green_ValueChanged_DD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green_DD.Value = Math.Round(Slider_Font_Green_DD.Value);
            fontColorDate.G = Convert.ToByte(Math.Round(Slider_Font_Green_DD.Value));
            SetFontColor_DD();
        }

        private void Slider_Font_Blue_ValueChanged_DD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue_DD.Value = Math.Round(Slider_Font_Blue_DD.Value);
            fontColorDate.B = Convert.ToByte(Math.Round(Slider_Font_Blue_DD.Value));
            SetFontColor_DD();
        }

        private void SetFontColor_DD()
        {
            this.SetFontColor_DD(fontColorDate);
        }

        public void SetFontColor_DD(Color color)
        {
            Digital_Date.Foreground = new SolidColorBrush(color);
        }

        public void SetBg(Color color)
        {
            Rectangle_Bg.Fill = new SolidColorBrush(color);
        }

        public void SetBg(string filename)
        {
            Rectangle_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
        }

        private void CalculateTimeDatePosition()
        {
            Digital_Date.Margin = new Thickness(0, this.Height / 3, 0, 0);
            Digital_Time.Margin = new Thickness(0, 0, 0, this.Height / 3);
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
            directory = System.IO.Path.Combine(directory, "Themes", "Digital");
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
                                this.Rectangle_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(imagepath)));
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

                        CheckBox_DD.IsChecked = (languageDictionary["DateFontEnabled"].ToString() == "true");

                        if (languageDictionary["TimeFontEnabled"].ToString() == "true")
                        {
                            fontColorTime.R = byte.Parse(languageDictionary["TimeFontR"].ToString());
                            fontColorTime.G = byte.Parse(languageDictionary["TimeFontG"].ToString());
                            fontColorTime.B = byte.Parse(languageDictionary["TimeFontB"].ToString());
                            Slider_Font_Red_DT.Value = fontColorTime.R;
                            Slider_Font_Green_DT.Value = fontColorTime.G;
                            Slider_Font_Blue_DT.Value = fontColorTime.B;
                            SetFontColor_DT(fontColorTime);
                        }

                        if (CheckBox_DD.IsChecked == true)
                        {
                            fontColorDate.R = byte.Parse(languageDictionary["DateFontR"].ToString());
                            fontColorDate.G = byte.Parse(languageDictionary["DateFontG"].ToString());
                            fontColorDate.B = byte.Parse(languageDictionary["DateFontB"].ToString());
                            Slider_Font_Red_DD.Value = fontColorDate.R;
                            Slider_Font_Green_DD.Value = fontColorDate.G;
                            Slider_Font_Blue_DD.Value = fontColorDate.B;
                            SetFontColor_DD(fontColorDate);
                        }
                    }
                }
            }
        }
    }
}
