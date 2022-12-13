using EvernoteClone.ViewModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        LoginVM viewModel;

        // konstruktor
        public LoginWindow()
        {
            InitializeComponent();

            // kreiramo viewModel da imamo pristup njegovim public property-evima
            viewModel = Resources["vm"] as LoginVM;

            // event handler kreiramo teko sto upisemo += i pritisnemo TAB
            viewModel.Authenticated += ViewModel_Authenticated;
        }

        private void ViewModel_Authenticated(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
