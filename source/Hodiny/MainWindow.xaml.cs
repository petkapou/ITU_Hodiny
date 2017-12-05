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
        private Color fontColor = new Color();
        private Color handColor = new Color();

        private double hourLenght = 0.2;
        private double minuteLenght = 0.3;
        private double secondLenght = 0.4;

        public MainWindow()
        {
            InitializeComponent();
            DateTime date = DateTime.Now;
            TimeZone time = TimeZone.CurrentTimeZone;
            TimeSpan difference = time.GetUtcOffset(date);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
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

            Slider_Transparency.Value = 1;

            ComboBox_Style.Items.Add("Analog");
            ComboBox_Style.Items.Add("Digital");
            ComboBox_Style.SelectedIndex = 0;

            ComboBox_Lang.Items.Add("English (EN)");
            ComboBox_Lang.Items.Add("Czech (CZ)");
            ComboBox_Lang.Items.Add("Slovak (SK)");
            ComboBox_Lang.SelectedIndex = 0;

            ComboBox_Bg_Type.Items.Add("Color");
            ComboBox_Bg_Type.Items.Add("Image");
            ComboBox_Bg_Type.SelectedIndex = 0;

            ComboBox_Hand.Items.Add("Hour");
            ComboBox_Hand.Items.Add("Minute");
            ComboBox_Hand.Items.Add("Second");
            ComboBox_Hand.SelectedIndex = 0;

            Slider_Lenght.Value = 0.8;

            bgColor.A = 255;
            Slider_Red.Value = bgColor.R = 128;
            Slider_Green.Value = bgColor.G = 128;
            Slider_Blue.Value = bgColor.B = 128;

            Slider_Font_Size.Value = 12;

            fontColor.A = 255;
            Slider_Font_Red.Value = fontColor.R = 255;
            Slider_Font_Green.Value = fontColor.G = 255;
            Slider_Font_Blue.Value = fontColor.B = 255;

            CalculateTimeDatePosition();

            FillFontComboBox(ComobBox_Fonts);
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
                Digital_Time.Content = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            }
            ));
        }
        private void FillFontComboBox(ComboBox comboBoxFonts)
        {
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                comboBoxFonts.Items.Add(fontFamily.Source);
            }

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

        private void tabItem_Exit_Clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void ComboBox_Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_Lang.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        private void ComboBox_Style_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_Style.SelectedIndex)
            {
                case 0:
                    Grid_AnalogClock.Visibility = Visibility.Visible;
                    Grid_DigitalClock.Visibility = Visibility.Hidden;
                    CheckBox_Font.Visibility = Visibility.Visible;
                    if (CheckBox_Font.IsChecked == false) {
                        TabItem_Fonts.Visibility = Visibility.Hidden;
                        TabItem_Fonts.MaxWidth = 0;
                        this.HideAnalogClockFont();
                    }
                    else {
                        TabItem_Fonts.Visibility = Visibility.Visible;
                        TabItem_Fonts.MaxWidth = 10000;
                        this.ShowAnalogClockFont();
                    }
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
                    CheckBox_Font.Visibility = Visibility.Hidden;
                    this.HideAnalogClockFont();
                    TabItem_DigitalClock.IsEnabled = true;
                    TabItem_DigitalClock.Visibility = Visibility.Visible;
                    TabItem_DigitalClock.MaxWidth = 10000;
                    TabItem_AnalogClock.IsEnabled = false;
                    TabItem_AnalogClock.Visibility = Visibility.Hidden;
                    TabItem_AnalogClock.MaxWidth = 0;
                    TabItem_Fonts.IsEnabled = true;
                    TabItem_Fonts.Visibility = Visibility.Visible;
                    TabItem_Fonts.MaxWidth = 10000;
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
            this.lenght_m = Slider_Lenght.Value;
            this.InitializeAnalogClockTextBlockArray();
        }

        private void Slider_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bgColor.R = Convert.ToByte(Math.Round(Slider_Red.Value));
            SetColor();
        }

        private void Slider_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bgColor.G = Convert.ToByte(Math.Round(Slider_Green.Value));
            SetColor();
        }

        private void Slider_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bgColor.B = Convert.ToByte(Math.Round(Slider_Blue.Value));
            SetColor();
        }

        private void SetColor()
        {
            this.SetBg(bgColor);
        }

        private void Button_File_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

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

        private void CheckBox_Italic_Checked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontStyle = FontStyles.Italic;
            }
        }

        private void CheckBox_Bold_Checked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontWeight = FontWeights.Bold;
            }
        }

        private void CheckBox_Italic_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontStyle = FontStyles.Normal;
            }
        }

        private void CheckBox_Bold_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontWeight = FontWeights.Normal;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TextBlock element in this.hourTextArray)
            {
                element.FontFamily = new FontFamily(ComobBox_Fonts.Text);
            }
        }

        private void Slider_Font_Size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                foreach (TextBlock element in this.hourTextArray)
                {
                    element.FontSize = Math.Round(Slider_Font_Size.Value);
                }
            }
        }

        private void Slider_Font_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fontColor.R = Convert.ToByte(Math.Round(Slider_Font_Red.Value));
            SetFontColor();
        }

        private void Slider_Font_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fontColor.G = Convert.ToByte(Math.Round(Slider_Font_Green.Value));
            TextBox_Font_Green.Text = Math.Round(Slider_Font_Green.Value).ToString();
            SetFontColor();
        }

        private void Slider_Font_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fontColor.B = Convert.ToByte(Math.Round(Slider_Font_Blue.Value));
            SetFontColor();
        }

        private void SetFontColor()
        {
            this.SetFontColor(fontColor);
        }

        private void CheckBox_Font_Checked(object sender, RoutedEventArgs e)
        {
            if (TabItem_Fonts != null)
            {
                TabItem_Fonts.Visibility = Visibility.Visible;
                TabItem_Fonts.IsEnabled = true;
                TabItem_Fonts.MaxWidth = 10000;
                this.ShowAnalogClockFont();
            }
        }

        private void CheckBox_Font_Unchecked(object sender, RoutedEventArgs e)
        {
            if (TabItem_Fonts != null)
            {
                TabItem_Fonts.Visibility = Visibility.Hidden;
                TabItem_Fonts.IsEnabled = false;
                TabItem_Fonts.MaxWidth = 0;
                this.HideAnalogClockFont();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
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
                CalculateTimeDatePosition();
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
                    Point center = new Point();
                    center.X = 0.5;
                    center.Y = 0.5;
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

        public void SetFontColor(Color color)
        {
            foreach (TextBlock element in hourTextArray)
            {
                element.Foreground = new SolidColorBrush(color);
            }
        }

        public void HideAnalogClockFont()
        {
            foreach (TextBlock element in hourTextArray)
            {
                element.Visibility = Visibility.Hidden;
            }
        }

        public void ShowAnalogClockFont()
        {
            foreach (TextBlock element in hourTextArray)
            {
                element.Visibility = Visibility.Visible;
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
            CalculateHandLenght(ComboBox_Hand.SelectedIndex, Slider_Hand_Lenght.Value);
        }

        private void Slider_Hand_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            handColor.R = (byte)Slider_Hand_Red.Value;
            SetHandColor();
        }

        private void Slider_Hand_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            handColor.G = (byte)Slider_Hand_Green.Value;
            SetHandColor();
        }

        private void Slider_Hand_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
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
