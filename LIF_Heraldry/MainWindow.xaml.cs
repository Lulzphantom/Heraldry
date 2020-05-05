using LIF_Heraldry.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FontAwesome.WPF;
using System.Windows.Media.Animation;

namespace LIF_Heraldry
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int CurrName = 0;
        bool toggleDown = false;
        double lastHeight;
        static string cacheRoute = @"\game\eu\art\Textures\Heraldry\Cache";
        private MediaPlayer mediaPlayer = new MediaPlayer();
        ColorAnimation colorChangeAnimation = new ColorAnimation();

        public MainWindow()
        {
            InitializeComponent();

            Config.CheckConfig();
            Opacity = Config.Opacity;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Try to clear game cache data before read any
            clearCache();

            Point location;

            // Set default working area
            if (Config.Top == 0 || Config.Left == 0) 
            {
                location = new Point(SystemParameters.WorkArea.Width - Width, SystemParameters.WorkArea.Height - Height);
                Config.Left = location.X;
                Config.Top = location.Y;
                Config.SaveConfig();
            }
            else
            {
                location = new Point(Config.Left, Config.Top);
            }

            Left = location.X;
            Top = location.Y;

            // Set timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += timer_Tick;
            timer.Start();

            // Item color animation
            colorChangeAnimation.From = Color.FromArgb(200, 0, 0, 0);
            colorChangeAnimation.To = Color.FromArgb(200, 160, 0, 0);
            colorChangeAnimation.Duration = TimeSpan.FromMilliseconds(400);

            mediaPlayer.Open(new Uri("pack://application:,,,/Assets/alert.mp3"));
            
        }

        void clearCache()
        {
            try
            {
                Directory.Delete(Config.LifRoute + cacheRoute);
                Console.WriteLine("Cache deleted");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timeLb.Content = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";
            if (Directory.Exists(Config.LifRoute + cacheRoute))
            {
                string[] tabardFiles = Directory.GetFiles(Config.LifRoute + cacheRoute);

                for (int i = 0; i < tabardFiles.Length; i++)
                {
                    string fileName = System.IO.Path.GetFileName(tabardFiles[i]);
                    if (fileName.Contains("Tabard"))
                    {
                        bool exist = false;

                        foreach (var item in StackNotify.Children)
                        {
                            if (((Grid)item).Name == "Grid" + getTabardId(fileName))
                            {
                                exist = true;
                            }
                        }

                        if (!exist)
                        {
                            AddNotify(getTabardId(fileName), tabardFiles[i]);
                        }
                    }                                       
                }
            }           
        }

        public void AddNotify(string fileName, string fileRoute)
        {
            string NameID = Convert.ToString(CurrName + 1);
            CurrName += 1;

            Grid Pgrid = new Grid();
            Pgrid.Name = "Grid" + fileName;
            Pgrid.Height = 70;
            Pgrid.Opacity = 0.7;
            Pgrid.Background = new SolidColorBrush(Colors.Black);
            Pgrid.Margin = new Thickness(0, 5, 0, 0);

            Image Tabard = new Image();
            Tabard.Name = "Image" + NameID;
            Tabard.HorizontalAlignment = HorizontalAlignment.Left;
            Tabard.VerticalAlignment = VerticalAlignment.Center;
            Tabard.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            Tabard.Stretch = Stretch.UniformToFill;
            Tabard.Source = new BitmapImage(new Uri(fileRoute, UriKind.Absolute));
            RenderOptions.SetBitmapScalingMode(Tabard, BitmapScalingMode.Fant);
            Tabard.Width = 40;
            Tabard.Height = 70;
            Tabard.Margin = new Thickness(15, 0, 0, 0);

            Label DateLb = new Label();
            DateLb.Name = "DateLb" + NameID;
            DateLb.HorizontalAlignment = HorizontalAlignment.Right;
            DateLb.VerticalAlignment = VerticalAlignment.Top;
            DateLb.Content = DateTime.Now.ToShortTimeString();
            DateLb.Foreground = new SolidColorBrush(Colors.LightGray);
            DateLb.Margin = new Thickness(0, 3, 5, 0);

            TextBlock Content = new TextBlock();
            Content.Name = "TextBlock" + NameID;
            Content.HorizontalAlignment = HorizontalAlignment.Left;
            Content.VerticalAlignment = VerticalAlignment.Top;
            Content.Text = $"ID: {fileName}";
            Content.MaxHeight = 33;
            Content.TextWrapping = TextWrapping.Wrap;
            Content.TextTrimming = TextTrimming.CharacterEllipsis;
            Content.Foreground = new SolidColorBrush(Colors.White);
            Content.Margin = new Thickness(70, 25, 0, 0);

            ImageAwesome deleteIcon = new ImageAwesome();
            deleteIcon.Name = "Del" + NameID;
            deleteIcon.ToolTip = "Try to delete the cache file, if the other player is out range the file will successful deleted. (can't delete own file)";
            deleteIcon.Tag = System.IO.Path.GetFileName(fileRoute);
            deleteIcon.HorizontalAlignment = HorizontalAlignment.Right;
            deleteIcon.VerticalAlignment = VerticalAlignment.Bottom;
            deleteIcon.Margin = new Thickness(0, 0, 15, 20);            
            deleteIcon.Width = 15;
            deleteIcon.Height = 15;
            deleteIcon.MouseLeftButtonUp += DeleteIcon_MouseLeftButtonUp;
            deleteIcon.Cursor = Cursors.Hand;
            deleteIcon.Icon = FontAwesomeIcon.TrashOutline;
            deleteIcon.Foreground = new SolidColorBrush(Colors.Red);

            Pgrid.Children.Add(Tabard);
            Pgrid.Children.Add(Content);
            Pgrid.Children.Add(DateLb);
            //Pgrid.Children.Add(deleteIcon);

            StackNotify.Children.Add(Pgrid);

            if (Height < 258 && StackNotify.Children.Count > 1)
            {
                Top = Top - 76;
                Height = Height + 76;

                mediaPlayer.Open(new Uri(@"./alert.mp3", UriKind.Relative));
                
                mediaPlayer.Play();

                PropertyPath colorTargetPath = new PropertyPath("(Grid.Background).(SolidColorBrush.Color)");
                Storyboard CellBackgroundChangeStory = new Storyboard();
                Storyboard.SetTarget(colorChangeAnimation, Pgrid);
                Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
                CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
                CellBackgroundChangeStory.AutoReverse = true;
                CellBackgroundChangeStory.Begin();

                toggleBt.IsEnabled = true;
            }
            else
            {
                if (!toggleDown)
                {
                    toggleBt.IsEnabled = false;
                }
            }
            ScrollNotify.ScrollToBottom();
            
        }

        private void DeleteIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid selectedItem = null;

            ImageAwesome deleteIcon = (ImageAwesome)sender;
            foreach (var item in StackNotify.Children)
            {
                if (((Grid)item).Name == "Grid" + deleteIcon.Name.Replace("Del",""))
                {
                    selectedItem = ((Grid)item);
                }
            }

            if (selectedItem != null)
            {               
                try
                {
                    File.Delete($@"{Config.LifRoute}{cacheRoute}\{deleteIcon.Tag}");
                    StackNotify.Children.Remove(selectedItem);

                }
                catch (Exception) {  }
            }

        }

        private void DragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            Config.Left = Left;
            Config.Top = Top;
            Config.SaveConfig();

        }

        private string getTabardId(string filename)
        {
            return filename.Substring(filename.IndexOf('_') + 1, 14);
        }

        private void configBt_Click(object sender, RoutedEventArgs e)
        {
            new ConfigWindow().Show();
        }

        private void exitBt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InfoBt_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Developed by Lulzphantom {Environment.NewLine}Guild: Realis Regnum {Environment.NewLine}Check new releases: https://github.com/Lulzphantom/Heraldry/releases");
        }
        
        private void toggleBt_Click(object sender, RoutedEventArgs e)
        {
            lastHeight = !toggleDown ? Height : lastHeight;
            Top = !toggleDown ? Top + lastHeight - 100 : Top - lastHeight + 100;

            toggleDown = !toggleDown;
            toggleIcon.Icon = toggleDown ? FontAwesomeIcon.AngleUp : FontAwesomeIcon.AngleDown;
            Height = toggleDown ? 100 : lastHeight;
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            clearCache();
        }

        private void DragBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           // Console.WriteLine("UP");
        }

        private void DragBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("UP");
        }
    }
}
