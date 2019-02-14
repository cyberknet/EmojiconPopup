using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace EmojiconPopup
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window
    {
        bool IsClosing = false;
        bool Limited = true;
        List<Emojicon> Emojicons = new List<Emojicon>();
        List<Emojicon> FilteredEmojicons = new List<Emojicon>();
        public Browser()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string filename = System.IO.Path.GetFullPath("emojicon.json");
            LoadEmojiconJson(filename);

            UpdateDisplayList();
        }

        private void LoadEmojiconJson(string filename)
        {
            string fileText = System.IO.File.ReadAllText(filename);
            var emojicons = JsonConvert.DeserializeObject<List<Emojicon>>(fileText);
            Emojicons.AddRange(emojicons);
            FilteredEmojicons.AddRange(Emojicons);
        }

        private void UpdateDisplayList()
        {
            var emojiconList = new List<Emojicon>(Limited ? FilteredEmojicons.Take(10) : FilteredEmojicons);
            for (int i = ButtonStack.Children.Count - 1; i > 0; i--)
                ButtonStack.Children.RemoveAt(i);
            foreach(var emojicon in emojiconList)
            {
                ButtonStack.Children.Add(
                    CreateButton(emojicon.Text, 200, Button_Click)
                    );
            }
            if (emojiconList.Count == 0)
            {
                ButtonStack.Children.Add(CreateTextBlock("(งツ)ว  Hmmmm, I didn't find that...", 200));
            }
            else if (FilteredEmojicons.Count > emojiconList.Count())
            {
                var textBlock = CreateTextBlock("ʕ*̫͡*ʕ•͓͡•ʔ-̫͡-ʕ•̫͡•ʔ*̫͡*ʔ There's too many...", 200);
                ButtonStack.Children.Add(textBlock);
                ButtonStack.Children.Add(
                    CreateButton("(∩｀-´)⊃━☆ﾟ.*･｡ﾟ Away, limit!", 200, ButtonSeeAll_Click)
                    );
            }
            if (!Limited)
            {
                ScrollView.MaxHeight = 400;
            }
        }

        private Button CreateButton(string text, double textBlockMarginThickness, RoutedEventHandler buttonClickEvent)
        {
            var button = new Button();
            button.Content = CreateTextBlock(text, textBlockMarginThickness);
            button.Click += buttonClickEvent;
            button.PreviewMouseRightButtonUp += Button_PreviewMouseRightButtonUp;
            return button;
        }

        private TextBlock CreateTextBlock(string text, double paddingThickness)
        {
            var textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.MinWidth = paddingThickness;
            textBlock.Padding = new Thickness(3);
            return textBlock;
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
                .Replace("\n", "+{Enter}"); // replace enter with shift-enter for Skype4Business Compatibility
            System.Windows.Forms.SendKeys.SendWait(textToCopy);
            this.CloseAndDontDeactivate();
        }

        private void ButtonSeeAll_Click(object sender, RoutedEventArgs e)
        {
            Limited = false;
            UpdateDisplayList();
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
}
