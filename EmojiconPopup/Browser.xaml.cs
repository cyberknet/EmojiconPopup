using Newtonsoft.Json;
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
using System.Windows.Shapes;

namespace AsciiBrowser
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window
    {
        bool IsClosing = false;
        List<Emojicon> Emojicons = new List<Emojicon>();
        List<Emojicon> FilteredEmojicons = new List<Emojicon>();
        public Browser()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string fileText = System.IO.File.ReadAllText("emojicon.json");
            var emojicons = JsonConvert.DeserializeObject<List<Emojicon>>(fileText);
            Emojicons.AddRange(emojicons);
            FilteredEmojicons.AddRange(Emojicons);

            UpdateDisplayList();
        }

        private void UpdateDisplayList()
        {
            for (int i = ButtonStack.Children.Count - 1; i > 0; i--)
                ButtonStack.Children.RemoveAt(i);
            foreach(var emojicon in FilteredEmojicons)
            {
                var button = new Button();
                var textBlock = new TextBlock();
                textBlock.Text = emojicon.Text;
                button.Content = textBlock;
                button.Click += Button_Click;
                button.PreviewMouseRightButtonUp += Button_PreviewMouseRightButtonUp;
                button.Padding = new Thickness(3);
                button.MinWidth = 200;
                ButtonStack.Children.Add(button);
            }
            if (FilteredEmojicons.Count == 0)
            {
                var textBlock = new TextBlock();
                textBlock.Text = "(´；ω；`);   waaaa - I couldn't find it!";
                textBlock.MinWidth = 200;
                textBlock.Padding = new Thickness(3);
                ButtonStack.Children.Add(textBlock);
            }
        }

        private void Button_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            CloseAndDontDeactivate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var textBlock = (TextBlock)button.Content;
            this.Hide();
            var textToCopy = textBlock.Text
                .Replace("+", "{+}")
                .Replace("^", "{^}")
                .Replace("~", "{~}")
                .Replace("(", "{(}")
                .Replace(")", "{)}")
                .Replace("\n", "+{Enter}");
            System.Windows.Forms.SendKeys.SendWait(textToCopy);
            this.CloseAndDontDeactivate();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseAndDontDeactivate();
            }
        }

        private void CloseAndDontDeactivate()
        {
            IsClosing = true;
            Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Debug.WriteLine("Deactivated!");
            if (!IsClosing)
                Close();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = Search.Text;
            var filteredList = Emojicons.Where(emojicon => emojicon.Keywords.Where(keyword => keyword.Contains(text, StringComparison.InvariantCultureIgnoreCase)).Count() > 0);
            FilteredEmojicons.Clear();
            FilteredEmojicons.AddRange(filteredList);
            UpdateDisplayList();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Search.Focus();
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}
