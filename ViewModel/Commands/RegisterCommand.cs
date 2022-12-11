using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EvernoteClone.Model;

namespace EvernoteClone.ViewModel.Commands
{
    public class RegisterCommand : ICommand
    {
        public LoginVM ViewModel { get; set; }

        //konstruktor - ukucamo ctor i dva puta pritisnemo TAB
        public RegisterCommand(LoginVM vm)
        {
            ViewModel = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            User user = parameter as User;

            // necemo zahtevati Firstname i lastname za sada
            if (user == null) return false;
            if(string.IsNullOrEmpty(user.Username)) return false;
            if (string.IsNullOrEmpty(user.Password)) return false;
            if (string.IsNullOrEmpty(user.ConfirmPassword)) return false;
            if(user.Password != user.ConfirmPassword) return false;
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.Register();
        }
    }
}
