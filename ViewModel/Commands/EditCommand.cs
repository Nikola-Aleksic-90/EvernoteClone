using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;     // kada ukucamo : ICommand moramo da dodamo ovaj using statement pa da implementiramo interfejs

namespace EvernoteClone.ViewModel.Commands
{
    public class EditCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public NotesVM ViewModel { get; set; }

        //konstruktor
        public EditCommand(NotesVM vm)
        {
            ViewModel= vm;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.StartEditing();
        }
    }
}
