using Azure.Storage.Blobs;
using EvernoteClone.ViewModel;
using EvernoteClone.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
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
        NotesVM viewModel;

        public NotesWindow()
        {
            InitializeComponent();

            viewModel = Resources["vm"] as NotesVM;
            viewModel.SelectedNoteChanged += ViewModel_SelectedNoteChanged;


            // Dodat je spisak za Combobox za izbor fontova
            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            fontFamilyComboBox.ItemsSource = fontFamilies;

            // Dodat je spisak za Combobox za izbor velicine fonta.
            // U XAML-u je stavljeno da je IsEditeble=true da korisnik moze da unese svoju velicinu fonta, npr 13 kojeg nema na spisku ispod
            List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 24, 32 };
            fontSizeComboBox.ItemsSource = fontSizes;
        }

        /*
        App klasa je uvek dostupna kada aplikacija radi tako da uvek imamo pristup varijablama koje su tamo. 
        Da bi otvorili Login kada korisnik nije ulogovan u App.xaml.cs kreiracemo public varijablu UserId kao empty string i pristupicemo iz NotesWindow-a. 
        Tamo cemo videti kroz if petlju da li je string null ili ako je prazan. 
        Ovako znamo da korisnik nije ulogovan te cemo prikazati LoginWindow.
        */
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (string.IsNullOrEmpty(App.UserId))
            {
                LoginWindow loginWindow= new LoginWindow();
                loginWindow.ShowDialog();

                viewModel.GetNotebooks();
            }
        }

        private async void ViewModel_SelectedNoteChanged(object sender, EventArgs e)
        {
            contentRichTextbox.Document.Blocks.Clear();

            if (viewModel.SelectedNote != null)
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedNote.FileLocation))
                {
                    // Downloadujemo Note da lokalno radimo sa njim
                    string downloadPath = $"{viewModel.SelectedNote.Id}.rtf";
                    await new BlobClient(new Uri(viewModel.SelectedNote.FileLocation)).DownloadToAsync(downloadPath);

                    using (FileStream fileStream = new FileStream(downloadPath, FileMode.Open))
                    {
                        TextRange range = new TextRange(contentRichTextbox.Document.ContentStart, contentRichTextbox.Document.ContentEnd);
                        range.Load(fileStream, DataFormats.Rtf);
                    }
                }
            }            
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

        // Sacuvaj Note
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string fileName = $"{viewModel.SelectedNote.Id}.rtf";
            string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory,fileName);
            
            using (FileStream fileStream = new FileStream(rtfFile, FileMode.Create))
            {
                var contents = new TextRange(contentRichTextbox.Document.ContentStart, contentRichTextbox.Document.ContentEnd);
                contents.Save(fileStream, DataFormats.Rtf);
            }

            viewModel.SelectedNote.FileLocation = await UpdateFile(rtfFile, fileName);
            await DatabaseHelper.Update(viewModel.SelectedNote);
        }

        private async Task<string> UpdateFile(string rtfFilePath , string fileName)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=azurestoragewpfapp;AccountKey=e8iXz8yhqC2wy/Z/Z8ZF1cNovSADFxGlMHWHRMbBe5SBq3rYrdrjuBVnOwmPLN6M5Eyo301V6H60+AStL2C9Iw==;EndpointSuffix=core.windows.net";
            string containerName = "notes"; // MORA da se poklapa sa nazivom container-a na Azure Storage

            var container = new BlobContainerClient(connectionString, containerName);
            // container.CreateIfNotExistsAsync();      // znamo da container postoji tako da ovo necemo koristiti

            var blob = container.GetBlobClient(fileName);
            await blob.UploadAsync(rtfFilePath);

            return $"https://azurestoragewpfapp.blob.core.windows.net/notes/{fileName}";
        }
    }
}
