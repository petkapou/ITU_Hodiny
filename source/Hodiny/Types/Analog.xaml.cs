﻿using System;
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
    public partial class Analog : Window
    {
        public double lenght_m = 0.8;
        public TextBlock[] hourTextArray = new TextBlock[12];
        string activeBoard = "HideMenu";
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        private Color bgColor;
        private Color fontColorTime = new Color();
        private Color fontColorDate = new Color();
        private Color handColor = new Color();

        private double hourLenght = 0.2;
        private double minuteLenght = 0.3;
        private double secondLenght = 0.4;

        private List<String> locales = new List<String>();
        private Action<ClockTypes> mCallback;

        public Analog(Color bgColor, List<String> locale, Action<ClockTypes> callback)
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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < hourTextArray.Length; i++)
            {
                hourTextArray[i] = new TextBlock();
                Grid_AnalogClock.Children.Add(hourTextArray[i]);
            }

            ComboBox_Style.SelectionChanged -= ComboBox_Style_SelectionChanged;
            ComboBox_Style.SelectedIndex = 0;
            ComboBox_Style.SelectionChanged += ComboBox_Style_SelectionChanged;

            InitializeAnalogClockTextBlockArray();

            AddLanguages();

            Slider_Transparency.Value = 1;
            ComboBox_Lang.SelectedIndex = 0;
            ComboBox_Bg_Type.SelectedIndex = 0;
            ComboBox_Hand.SelectedIndex = 0;
            Slider_Lenght.Value = 0.8;

            bgColor.A = 255;
            Slider_Red.Value = bgColor.R;
            Slider_Green.Value = bgColor.G;
            Slider_Blue.Value = bgColor.B;

            Slider_Font_Size.Value = 12;

            Slider_Font_Red_AD.Value = 255;
            Slider_Font_Green_AD.Value = 255;
            Slider_Font_Blue_AD.Value = 255;
            Analog_Date.FontSize = 15;
            Slider_Font_Size_AD.Value = 15;

            CalculateTimeDatePosition();

            FillFontComboBox(ComobBox_Fonts);

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
            directory = System.IO.Path.Combine(directory, "Themes", "Analog");
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

        private void Actualize_Date(object sender)
        {
            if (Analog_Date == null)
            {
                return;
            }
            switch (ComboBox_Date_Format.SelectedIndex)
            {
                case 0:
                    Analog_Date.Content = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
                    break;
                case 1:
                    Analog_Date.Content = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                    break;
                case 2:
                    Analog_Date.Content = DateTime.Now.Month + "." + DateTime.Now.Day + "." + DateTime.Now.Year;
                    break;
                case 3:
                    Analog_Date.Content = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year;
                    break;
                case 4:
                    Analog_Date.Content = DateTime.Now.Day + "." + DateTime.Now.Month;
                    break;
                case 5:
                    Analog_Date.Content = DateTime.Now.Day + "/" + DateTime.Now.Month;
                    break;
                case 6:
                    Analog_Date.Content = DateTime.Now.Month + "." + DateTime.Now.Day;
                    break;
                case 7:
                    Analog_Date.Content = DateTime.Now.Month + "/" + DateTime.Now.Day;
                    break;
                default:
                    break;
            }
        }

        private void Actualize_Date(object sender, RoutedEventArgs e)
        {
            Actualize_Date(sender);
        }


        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                secondHandRotate.Angle = DateTime.Now.Second * 6;
                minuteHandRotate.Angle = DateTime.Now.Minute * 6;
                hourHandRotate.Angle = (DateTime.Now.Hour * 30) + (DateTime.Now.Minute * 0.5);

                Analog_Date.Content = DateTime.Now.Date;
                Actualize_Date(sender);
            }
            ));
        }

        private void FillFontComboBox(ComboBox comboBoxFonts)
        {
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                comboBoxFonts.Items.Add(fontFamily.Source);
            }
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                ComobBox_Fonts_AD.Items.Add(fontFamily.Source);
            }

            ComobBox_Fonts_AD.SelectedIndex = 0;
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
            ComboBox_Style.SelectedIndex = 0;
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
                Filter = "Image files (*.jpg, *.jpeg, *.gif, *.png) | *.jpg; *.jpeg; *.gif; *.png"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                TextBox_File.Text = filename;
                this.Ellipse_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
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
            fontColorTime.R = Convert.ToByte(Math.Round(Slider_Font_Red.Value));
            SetFontColor_AT();
        }

        private void Slider_Font_Green_ValueChanged_AT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green.Value = Math.Round(Slider_Font_Green.Value);
            fontColorTime.G = Convert.ToByte(Math.Round(Slider_Font_Green.Value));
            //TextBox_Font_Green.Text = Math.Round(Slider_Font_Green.Value).ToString();
            SetFontColor_AT();
        }

        private void Slider_Font_Blue_ValueChanged_AT(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue.Value = Math.Round(Slider_Font_Blue.Value);
            fontColorTime.B = Convert.ToByte(Math.Round(Slider_Font_Blue.Value));
            SetFontColor_AT();
        }

        private void SetFontColor_AT()
        {
            this.SetFontColor_AT(fontColorTime);
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
            fontColorDate.R = Convert.ToByte(Math.Round(Slider_Font_Red_AD.Value));
            SetFontColor_AD();
        }

        private void Slider_Font_Green_ValueChanged_AD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Green_AD.Value = Math.Round(Slider_Font_Green_AD.Value);
            fontColorDate.G = Convert.ToByte(Math.Round(Slider_Font_Green_AD.Value));
            TextBox_Font_Green_AD.Text = Math.Round(Slider_Font_Green_AD.Value).ToString();
            SetFontColor_AD();
        }

        private void Slider_Font_Blue_ValueChanged_AD(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Font_Blue_AD.Value = Math.Round(Slider_Font_Blue_AD.Value);
            fontColorDate.B = Convert.ToByte(Math.Round(Slider_Font_Blue_AD.Value));
            SetFontColor_AD();
        }

        private void SetFontColor_AD()
        {
            this.SetFontColor_AD(fontColorDate);
        }

        public void SetFontColor_AD(Color color)
        {
            Analog_Date.Foreground = new SolidColorBrush(color);
        }

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
        }

        public void SetBg(string filename)
        {
            Ellipse_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(filename)));
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

        private void ComboBox_Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Theme.SelectedIndex == -1)
                return;
            String directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            directory = System.IO.Path.Combine(directory, "Themes", "Analog");
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
                        switch(languageDictionary["Type"].ToString())
                        {
                            case "Image":
                                imagepath = System.IO.Path.Combine(directory, languageDictionary["ImageFileName"].ToString());
                                this.Ellipse_Bg.Fill = new ImageBrush(new BitmapImage(new Uri(imagepath)));
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

                        CheckBox_AT.IsChecked = (languageDictionary["TimeFontEnabled"].ToString() == "true");
                        CheckBox_AD.IsChecked = (languageDictionary["DateFontEnabled"].ToString() == "true");

                        if (CheckBox_AT.IsChecked == true)
                        {
                            fontColorTime.R = byte.Parse(languageDictionary["TimeFontR"].ToString());
                            fontColorTime.G = byte.Parse(languageDictionary["TimeFontG"].ToString());
                            fontColorTime.B = byte.Parse(languageDictionary["TimeFontB"].ToString());
                            Slider_Font_Red.Value = fontColorTime.R;
                            Slider_Font_Green.Value = fontColorTime.G;
                            Slider_Font_Blue.Value = fontColorTime.B;
                            SetFontColor_AT(fontColorTime);
                        }

                        if (CheckBox_AD.IsChecked == true)
                        {
                            fontColorDate.R = byte.Parse(languageDictionary["DateFontR"].ToString());
                            fontColorDate.G = byte.Parse(languageDictionary["DateFontG"].ToString());
                            fontColorDate.B = byte.Parse(languageDictionary["DateFontB"].ToString());
                            Slider_Font_Red_AD.Value = fontColorDate.R;
                            Slider_Font_Green_AD.Value = fontColorDate.G;
                            Slider_Font_Blue_AD.Value = fontColorDate.B;
                            SetFontColor_AD(fontColorDate);
                        }
                    }
                }
            }
        }
    }
}
