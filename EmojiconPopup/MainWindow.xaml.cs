using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NHotkey;
using NHotkey.Wpf;

namespace EmojiconPopup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HotkeyManager.Current.AddOrReplace("Show", Key.Y, ModifierKeys.Windows, OnHotKey);
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnHotKey(object sender, HotkeyEventArgs e)
        {
            ShowBrowser();
            e.Handled = true;
        }

        Browser browser = null;
        private void ShowBrowser()
        {
            if (browser != null) browser.Close();
            browser = new Browser();
            browser.Show();
            browser.Activate();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
