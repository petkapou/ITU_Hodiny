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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double lenght_m = 0.8;
        public TextBlock[] hourTextArray = new TextBlock[12];
        string activeBoard = "HideMenu";
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        private Color bgColor = new Color();
        private Color fontColorAT = new Color();//analog time
        private Color fontColorDT = new Color();//digital time
        private Color fontColorAD = new Color();//analog date
        private Color fontColorDD = new Color();//digital date
        private Color handColor = new Color();

        private double hourLenght = 0.2;
        private double minuteLenght = 0.3;
        private double secondLenght = 0.4;

        private List<String> locales = new List<String>();

        public MainWindow()
        {
            InitializeComponent();
            DateTime date = DateTime.Now;
            TimeZone time = TimeZone.CurrentTimeZone;
            TimeSpan difference = time.GetUtcOffset(date);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            timer.Enabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < hourTextArray.Length; i++)
            {
                hourTextArray[i] = new TextBlock();
                Grid_AnalogClock.Children.Add(hourTextArray[i]);
            }

            InitializeAnalogClockTextBlockArray();

            AddLanguages();

            Slider_Transparency.Value = 1;            
            ComboBox_Style.SelectedIndex = 0;            
            ComboBox_Lang.SelectedIndex = 0;
            ComboBox_Bg_Type.SelectedIndex = 0;            
            ComboBox_Hand.SelectedIndex = 0;
            Slider_Lenght.Value = 0.8;

            bgColor.A = 255;
            Slider_Red.Value = bgColor.R = 128;
            Slider_Green.Value = bgColor.G = 128;
            Slider_Blue.Value = bgColor.B = 128;

            Slider_Font_Size.Value = 12;

            fontColorAT.A = 255;
            Slider_Font_Red.Value = fontColorAT.R = 255;
            Slider_Font_Green.Value = fontColorAT.G = 255;
            Slider_Font_Blue.Value = fontColorAT.B = 255;

            fontColorDT.A = 255;
            Slider_Font_Red_DT.Value = fontColorDT.R = 255;
            Slider_Font_Green_DT.Value = fontColorDT.G = 255;
            Slider_Font_Blue_DT.Value = fontColorDT.B = 255;
            Digital_Time.FontSize = 50;
            Slider_Font_Size_DT.Value = 50;

            fontColorAD.A = 255;
            Slider_Font_Red_AD.Value = fontColorAD.R = 255;
            Slider_Font_Green_AD.Value = fontColorAD.G = 255;
            Slider_Font_Blue_AD.Value = fontColorAD.B = 255;
            Analog_Date.FontSize = 15;
            Slider_Font_Size_AD.Value = 15;

            fontColorDD.A = 255;
            Slider_Font_Red_DD.Value = fontColorDD.R = 255;
            Slider_Font_Green_DD.Value = fontColorDD.G = 255;
            Slider_Font_Blue_DD.Value = fontColorDD.B = 255;
            Digital_Date.FontSize = 15;
            Slider_Font_Size_DD.Value = 15;

            fontColorDD.A = 255;
            Slider_Font_Red.Value = fontColorDD.R = 255;
            Slider_Font_Green.Value = fontColorDD.G = 255;
            Slider_Font_Blue.Value = fontColorDD.B = 255;

            CalculateTimeDatePosition();

            FillFontComboBox(ComobBox_Fonts);
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

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                secondHandRotate.Angle = DateTime.Now.Second * 6;
                minuteHandRotate.Angle = DateTime.Now.Minute * 6;
                hourHandRotate.Angle = (DateTime.Now.Hour * 30) + (DateTime.Now.Minute * 0.5);
                /*switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                {
                    case 1:
                        Analog_Date.Content = "Mo ";
                        break;
                    case 2:
                        Analog_Date.Content = "Tu ";
                        break;
                    case 3:
                        Analog_Date.Content = "We ";
                        break;
                    case 4:
                        Analog_Date.Content = "Th ";
                        break;
                    case 5:
                        Analog_Date.Content = "Fr ";
                        break;
                    case 6:
                        Analog_Date.Content = "Sa ";
                        break;
                    case 0:
                    case 7:
                        Analog_Date.Content = "Su ";
                        break;
                    default:
                        break;
                }*/
                Analog_Date.Content = DateTime.Now.Date;
                Digital_Date.Content = DateTime.Now.Date;
                //Digital_Time.Content = DateTime.Now.TimeOfDay; // Tiskne setiny, fakt hnus
                string tmp_time = DateTime.Now.Hour + ":";
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
                    //Digital_Time.Content = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                }
                Digital_Time.Content = tmp_time;
            }
            ));
        }

        private void Show_Sec_Actualize(object sender, RoutedEventArgs e)
        {
            if (Digital_Time == null)
            {
                return;
            }
            string tmp_time = DateTime.Now.Hour + ":";
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
                //Digital_Time.Content = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            }
            Digital_Time.Content = tmp_time;
        }

        private void FillFontComboBox(ComboBox comboBoxFonts)
        {
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                comboBoxFonts.Items.Add(fontFamily.Source);
            }
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                ComobBox_Fonts_DT.Items.Add(fontFamily.Source);
            }
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                ComobBox_Fonts_AD.Items.Add(fontFamily.Source);
            }
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                ComobBox_Fonts_DD.Items.Add(fontFamily.Source);
            }

            ComobBox_Fonts_DD.SelectedIndex = 0;
            ComobBox_Fonts_AD.SelectedIndex = 0;
            ComobBox_Fonts_DT.SelectedIndex = 0;
            comboBoxFonts.SelectedIndex = 0;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            activeBoard = "HideMenu";
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
                    Grid_AnalogClock.Visibility = Visibility.Visible;
                    Grid_DigitalClock.Visibility = Visibility.Hidden;
                    if (CheckBox_AT.IsChecked == false)
                    {
                        this.HideAnalogClockFont();
                    }
                    else
                    {
                        this.ShowAnalogClockFont();
                    }
                    TabItem_Fonts_Analog.IsEnabled = true;
                    TabItem_Fonts_Analog.Visibility = Visibility.Visible;
                    TabItem_Fonts_Analog.MaxWidth = 10000;
                    TabItem_Fonts_Digital.IsEnabled = false;
                    TabItem_Fonts_Digital.Visibility = Visibility.Hidden;
                    TabItem_Fonts_Digital.MaxWidth = 0;
                    TabItem_AnalogClock.IsEnabled = true;
                    TabItem_AnalogClock.Visibility = Visibility.Visible;
                    TabItem_AnalogClock.MaxWidth = 10000;
                    TabItem_DigitalClock.IsEnabled = false;
                    TabItem_DigitalClock.Visibility = Visibility.Hidden;
                    TabItem_DigitalClock.MaxWidth = 0;
                    break;

                case 1:
                    Grid_AnalogClock.Visibility = Visibility.Hidden;
                    Grid_DigitalClock.Visibility = Visibility.Visible;
                    this.HideAnalogClockFont();
                    TabItem_DigitalClock.IsEnabled = true;
                    TabItem_DigitalClock.Visibility = Visibility.Visible;
                    TabItem_DigitalClock.MaxWidth = 10000;
                    TabItem_AnalogClock.IsEnabled = false;
                    TabItem_AnalogClock.Visibility = Visibility.Hidden;
                    TabItem_AnalogClock.MaxWidth = 0;
                    TabItem_Fonts_Digital.IsEnabled = true;
                    TabItem_Fonts_Digital.Visibility = Visibility.Visible;
                    TabItem_Fonts_Digital.MaxWidth = 10000;
                    TabItem_Fonts_Analog.IsEnabled = false;
                    TabItem_Fonts_Analog.Visibility = Visibility.Hidden;
                    TabItem_Fonts_Analog.MaxWidth = 0;
                    this.HideAnalogClockFont();
                    break;

                default:
                    break;
            }
        }

        private void ComboBox_Bg_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_Bg_Type.SelectedIndex)
            {
                case 0:
                    Label_Color.IsEnabled = true;
                    Label_Color.Visibility = Visibility.Visible;

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

                    Label_Image.IsEnabled = false;
                    Label_Image.Visibility = Visibility.Hidden;
                    TextBox_File.IsEnabled = false;
                    TextBox_File.Visibility = Visibility.Hidden;
                    Button_File.IsEnabled = false;
                    Button_File.Visibility = Visibility.Hidden;
                    break;

                case 1:
                    Label_Color.IsEnabled = false;
                    Label_Color.Visibility = Visibility.Hidden;

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

                    Label_Image.IsEnabled = true;
                    Label_Image.Visibility = Visibility.Visible;
                    TextBox_File.IsEnabled = true;
                    TextBox_File.Visibility = Visibility.Visible;
                    Button_File.IsEnabled = true;
                    Button_File.Visibility = Visibility.Visible;
                    break;

                default:
                    break;
            }
        }

        private void Slider_Lenght_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            Slider_Lenght.Value = Math.Round(Slider_Lenght.Value * 100) / 100;
            this.lenght_m = Slider_Lenght.Value;
            this.InitializeAnalogClockTextBlockArray();
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
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                TextBox_File.Text = filename;
                if (ComboBox_Style.SelectedIndex == 0)
                    this.Ellipse_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
                if (ComboBox_Style.SelectedIndex == 1)
                    this.Rectangle_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
            }
        }

        // ANALOG TIME FONTS

        private void CheckBox_AT_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowAnalogClockFont();
        }

        private void CheckBox_AT_Unchecked(object sender, RoutedEventArgs e)
        {
            this.HideAnalogClockFont();
        }

        private void CheckBox_Italic_Checked_AT(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontStyle = FontStyles.Italic;
            }
        }

        private void CheckBox_Bold_Checked_AT(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontWeight = FontWeights.Bold;
            }
        }

        private void CheckBox_Italic_Unchecked_AT(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontStyle = FontStyles.Normal;
            }
        }

        private void CheckBox_Bold_Unchecked_AT(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontWeight = FontWeights.Normal;
            }
        }

        private void ComboBox_SelectionChanged_AT(object sender, SelectionChangedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontFamily = new FontFamily(ComobBox_Fonts.Text);
            }
        }

        private void Slider_Font_Size_ValueChanged_AT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                Slider_Font_Size.Value = Math.Round(Slider_Font_Size.Value);
                foreach (TextBlock element in this.hourTextArray)
                {
                    element.FontSize = Math.Round(Slider_Font_Size.Value);
                }
            }
        }

        private void Slider_Font_Red_ValueChanged_AT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Red.Value = Math.Round(Slider_Font_Red.Value);
            fontColorAT.R = Convert.ToByte(Math.Round(Slider_Font_Red.Value));
            SetFontColor_AT();
        }

        private void Slider_Font_Green_ValueChanged_AT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green.Value = Math.Round(Slider_Font_Green.Value);
            fontColorAT.G = Convert.ToByte(Math.Round(Slider_Font_Green.Value));
            TextBox_Font_Green.Text = Math.Round(Slider_Font_Green.Value).ToString();
            SetFontColor_AT();
        }

        private void Slider_Font_Blue_ValueChanged_AT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue.Value = Math.Round(Slider_Font_Blue.Value);
            fontColorAT.B = Convert.ToByte(Math.Round(Slider_Font_Blue.Value));
            SetFontColor_AT();
        }

        private void SetFontColor_AT()
        {
            this.SetFontColor_AT(fontColorAT);
        }
        
        public void SetFontColor_AT(Color color)
        {
            foreach (TextBlock element in hourTextArray)
            {
                element.Foreground = new SolidColorBrush(color);
            }
        }

        // ANALOG DATE FONTS

        private void CheckBox_AD_Checked(object sender, RoutedEventArgs e)
        {
            if (Analog_Date != null)
            {
                Analog_Date.Visibility = Visibility.Visible;
            }
        }

        private void CheckBox_AD_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Analog_Date != null)
            {
                Analog_Date.Visibility = Visibility.Hidden;
            }
        }

        private void CheckBox_Italic_Checked_AD(object sender, RoutedEventArgs e)
        {
            Analog_Date.FontStyle = FontStyles.Italic;
        }

        private void CheckBox_Bold_Checked_AD(object sender, RoutedEventArgs e)
        {
            Analog_Date.FontWeight = FontWeights.Bold;
        }

        private void CheckBox_Italic_Unchecked_AD(object sender, RoutedEventArgs e)
        {
            Analog_Date.FontStyle = FontStyles.Normal;
        }

        private void CheckBox_Bold_Unchecked_AD(object sender, RoutedEventArgs e)
        {
            Analog_Date.FontWeight = FontWeights.Normal;
        }

        private void ComboBox_SelectionChanged_AD(object sender, SelectionChangedEventArgs e)
        {
            Analog_Date.FontFamily = new FontFamily(ComobBox_Fonts_AD.Text);
        }

        private void Slider_Font_Size_ValueChanged_AD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Size_AD.Value = Math.Round(Slider_Font_Size_AD.Value);
            if (this.IsLoaded)
            {
                Analog_Date.FontSize = Math.Round(Slider_Font_Size_AD.Value);
            }
        }

        private void Slider_Font_Red_ValueChanged_AD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Red_AD.Value = Math.Round(Slider_Font_Red_AD.Value);
            fontColorAD.R = Convert.ToByte(Math.Round(Slider_Font_Red_AD.Value));
            SetFontColor_AD();
        }

        private void Slider_Font_Green_ValueChanged_AD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green_AD.Value = Math.Round(Slider_Font_Green_AD.Value);
            fontColorAD.G = Convert.ToByte(Math.Round(Slider_Font_Green_AD.Value));
            TextBox_Font_Green_AD.Text = Math.Round(Slider_Font_Green_AD.Value).ToString();
            SetFontColor_AD();
        }

        private void Slider_Font_Blue_ValueChanged_AD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue_AD.Value = Math.Round(Slider_Font_Blue_AD.Value);
            fontColorAD.B = Convert.ToByte(Math.Round(Slider_Font_Blue_AD.Value));
            SetFontColor_AD();
        }

        private void SetFontColor_AD()
        {
            this.SetFontColor_AD(fontColorAD);
        }

        public void SetFontColor_AD(Color color)
        {
            Analog_Date.Foreground = new SolidColorBrush(color);
        }

        // DIGITAL TIME FONTS

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
            fontColorDT.R = Convert.ToByte(Math.Round(Slider_Font_Red_DT.Value));
            SetFontColor_DT();
        }

        private void Slider_Font_Green_ValueChanged_DT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green_DT.Value = Math.Round(Slider_Font_Green_DT.Value);
            fontColorDT.G = Convert.ToByte(Math.Round(Slider_Font_Green_DT.Value));
            TextBox_Font_Green_DT.Text = Math.Round(Slider_Font_Green_DT.Value).ToString();
            SetFontColor_DT();
        }

        private void Slider_Font_Blue_ValueChanged_DT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue_DT.Value = Math.Round(Slider_Font_Blue_DT.Value);
            fontColorDT.B = Convert.ToByte(Math.Round(Slider_Font_Blue_DT.Value));
            SetFontColor_DT();
        }

        private void SetFontColor_DT()
        {
            this.SetFontColor_DT(fontColorDT);
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
            fontColorDD.R = Convert.ToByte(Math.Round(Slider_Font_Red_DD.Value));
            SetFontColor_DD();
        }

        private void Slider_Font_Green_ValueChanged_DD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green_DD.Value = Math.Round(Slider_Font_Green_DD.Value);
            fontColorDD.G = Convert.ToByte(Math.Round(Slider_Font_Green_DD.Value));
            TextBox_Font_Green_DD.Text = Math.Round(Slider_Font_Green_DD.Value).ToString();
            SetFontColor_DD();
        }

        private void Slider_Font_Blue_ValueChanged_DD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue_DD.Value = Math.Round(Slider_Font_Blue_DD.Value);
            fontColorDD.B = Convert.ToByte(Math.Round(Slider_Font_Blue_DD.Value));
            SetFontColor_DD();
        }

        private void SetFontColor_DD()
        {
            this.SetFontColor_DD(fontColorDD);
        }

        public void SetFontColor_DD(Color color)
        {
            Digital_Date.Foreground = new SolidColorBrush(color);
        }

        // END FONTS

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CalculateTimeDatePosition();
            if (Ellipse_Bg.Visibility == Visibility.Visible)
            {
                if (sizeInfo.WidthChanged)
                {
                    this.Width = sizeInfo.NewSize.Height;
                }
                else
                {
                    this.Height = sizeInfo.NewSize.Width;
                }
                CalculateHandLenght(42, 0);
                InitializeAnalogClockTextBlockArray();
            }
        }

        public void InitializeAnalogClockTextBlockArray()
        {
            if (this.IsLoaded)
            {
                double rotateAngle = (Math.PI * 2) / Convert.ToDouble(hourTextArray.Length);
                double drawAngle = 2 * rotateAngle;
                double lenght = (this.Width / 2) * lenght_m;
                int hour = 1;

                foreach (TextBlock element in hourTextArray)
                {
                    TranslateTransform position = new TranslateTransform();
                    Point center = new Point
                    {
                        X = 0.5,
                        Y = 0.5
                    };
                    position.X = (Math.Cos(drawAngle) * lenght);
                    position.Y = -(Math.Sin(drawAngle) * lenght);

                    element.Text = hour.ToString();
                    element.HorizontalAlignment = HorizontalAlignment.Center;
                    element.VerticalAlignment = VerticalAlignment.Center;
                    element.RenderTransformOrigin = center;
                    element.RenderTransform = position;
                    drawAngle -= rotateAngle;

                    hour++;
                }
            }
        }

        public void SetBg(Color color)
        {
            Ellipse_Bg.Fill = new SolidColorBrush(color);
            Rectangle_Bg.Fill = new SolidColorBrush(color);
        }

        public void SetBg(string filename)
        {
            Ellipse_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
            Rectangle_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
        }

        public void HideAnalogClockFont()
        {
            foreach (TextBlock element in hourTextArray)
            {
                if (element != null)
                {
                    element.Visibility = Visibility.Hidden;
                }
            }
        }

        public void ShowAnalogClockFont()
        {
            foreach (TextBlock element in hourTextArray)
            {
                if (element != null)
                {
                    element.Visibility = Visibility.Visible;
                }
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activeBoard == "HideMenu") activeBoard = "ShowMenu";
            else activeBoard = "HideMenu";
            Storyboard sb = Resources[activeBoard] as Storyboard;
            sb.Begin(pnlLeftMenu);
        }

        private void Slider_Hand_Lenght_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Hand_Lenght.Value = Math.Round(Slider_Hand_Lenght.Value * 100) / 100;
            CalculateHandLenght(ComboBox_Hand.SelectedIndex, Slider_Hand_Lenght.Value);
        }

        private void Slider_Hand_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Hand_Red.Value = Math.Round(Slider_Hand_Red.Value);
            handColor.R = (byte)Slider_Hand_Red.Value;
            SetHandColor();
        }

        private void Slider_Hand_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Hand_Green.Value = Math.Round(Slider_Hand_Green.Value);
            handColor.G = (byte)Slider_Hand_Green.Value;
            SetHandColor();
        }

        private void Slider_Hand_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Hand_Blue.Value = Math.Round(Slider_Hand_Blue.Value);
            handColor.B = (byte)Slider_Hand_Blue.Value;
            SetHandColor();
        }

        private void CalculateHandLenght(int index, double value)
        {
            switch (index)
            {
                case 0:
                    hourLenght = value;
                    break;
                case 1:
                    minuteLenght = value;
                    break;
                case 2:
                    secondLenght = value;
                    break;
                default:
                    break;
            }

            rectangleHour.Height = this.Width * hourLenght;
            rectangleHour.Width = this.Width / 65;
            hourHandTransform.Y = -rectangleHour.Height / 2;

            rectangleMinute.Height = this.Width * minuteLenght;
            rectangleMinute.Width = this.Width / 65;
            minuteHandTransform.Y = -rectangleMinute.Height / 2;

            rectangleSecond.Height = this.Width * secondLenght;
            rectangleSecond.Width = this.Width / 65;
            secondHandTransform.Y = -rectangleSecond.Height / 2;
        }

        private void CalculateTimeDatePosition()
        {
            Analog_Date.Margin = new Thickness(0, this.Height / 3, 0, 0);
            Digital_Date.Margin = new Thickness(0, this.Height / 3, 0, 0);
            Digital_Time.Margin = new Thickness(0, 0, 0, this.Height / 3);
        }

        private void ComboBox_Hand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_Hand.SelectedIndex)
            {
                case 0:
                    Slider_Hand_Lenght.Value = hourLenght;
                    handColor = (rectangleHour.Fill as SolidColorBrush).Color;
                    break;
                case 1:
                    Slider_Hand_Lenght.Value = minuteLenght;
                    handColor = (rectangleMinute.Fill as SolidColorBrush).Color;
                    break;
                case 2:
                    Slider_Hand_Lenght.Value = secondLenght;
                    handColor = (rectangleSecond.Fill as SolidColorBrush).Color;
                    break;
                default:
                    break;
            }

            Slider_Hand_Red.Value = handColor.R;
            Slider_Hand_Green.Value = handColor.G;
            Slider_Hand_Blue.Value = handColor.B;
        }

        private void SetHandColor()
        {
            switch (ComboBox_Hand.SelectedIndex)
            {
                case 0:
                    rectangleHour.Fill = new SolidColorBrush(handColor);
                    break;
                case 1:
                    rectangleMinute.Fill = new SolidColorBrush(handColor);
                    break;
                case 2:
                    rectangleSecond.Fill = new SolidColorBrush(handColor);
                    break;
                default:
                    break;
            }
        }
    }
}
