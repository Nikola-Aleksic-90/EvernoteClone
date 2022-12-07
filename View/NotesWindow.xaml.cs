using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        public NotesWindow()
        {
            InitializeComponent();

            // Dodat je spisak za Combobox za izbor fontova
            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            fontFamilyComboBox.ItemsSource = fontFamilies;

            // Dodat je spisak za Combobox za izbor velicine fonta.
            // U XAML-u je stavljeno da je IsEditeble=true da korisnik moze da unese svoju velicinu fonta, npr 13 kojeg nema na spisku ispod
            List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 24, 32 };
            fontSizeComboBox.ItemsSource = fontSizes;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void contentRichTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amountCharacters = (new TextRange(contentRichTextbox.Document.ContentStart, contentRichTextbox.Document.ContentEnd)).Text.Length;
            statusTextBlock.Text = $"Document length: {amountCharacters} characters";
        }

        private void boldButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonChecked)
                contentRichTextbox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            else
                contentRichTextbox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
        }

        private void contentRichTextbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = contentRichTextbox.Selection.GetPropertyValue(FontWeightProperty);
            boldButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue && selectedWeight.Equals(FontWeights.Bold));

            var selectedStyle = contentRichTextbox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            italicButton.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            var selecteDecoration = contentRichTextbox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineButton.IsChecked = (selecteDecoration != DependencyProperty.UnsetValue) && (selecteDecoration.Equals(TextDecorations.Underline));

            fontFamilyComboBox.SelectedItem = contentRichTextbox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            fontSizeComboBox.Text = (contentRichTextbox.Selection.GetPropertyValue(Inline.FontSizeProperty)).ToString();
        }

        private void italicButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonChecked)
                contentRichTextbox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            else
                contentRichTextbox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
        }

        private void underlineButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonChecked)
                contentRichTextbox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            else
            {
                TextDecorationCollection textDecorations;
                (contentRichTextbox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out textDecorations);
                contentRichTextbox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(fontFamilyComboBox.SelectedItem != null)
            {
                contentRichTextbox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
            }
        }

        private void fontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            contentRichTextbox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);
        }
    }
}
