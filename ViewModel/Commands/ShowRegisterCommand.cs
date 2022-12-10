using EvernoteClone.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class ShowRegisterCommand : ICommand
    {
        public LoginVM ViewModel { get; set; }

        public event EventHandler? CanExecuteChanged;

        // konstruktor
        public ShowRegisterCommand(LoginVM vm)
        {
            ViewModel= vm;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.SwitchViews();
        }
    }
}
