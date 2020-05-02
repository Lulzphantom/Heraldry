using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LIF_Heraldry.Windows
{
    /// <summary>
    /// Lógica de interacción para ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        Window main;
        double _currentOpacity;

        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            opacitySlider.Value = Config.Opacity * 10;
            _currentOpacity = Config.Opacity;
            pathTB.Text = Config.LifRoute;
            main = Application.Current.Windows[0];
        }

        private void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            opacityLabel.Content = $"{Math.Floor(opacitySlider.Value * 10)}%";
            Config.Opacity = opacitySlider.Value / 10;

            if (main != null)
            {
                main.Opacity = Config.Opacity;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (main != null)
            {
                Config.Opacity = _currentOpacity;
                main.Opacity = Config.Opacity;
            }
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (pathTB.Text != "")
            {
                _currentOpacity = Config.Opacity;
                Config.SaveConfig();
                Close();
            }
            
        }

        private void pathBt_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                Config.LifRoute = dialog.FileName;
                pathTB.Text = Config.LifRoute;
            }
        }
    }
}
