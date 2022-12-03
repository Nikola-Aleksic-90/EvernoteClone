using EvernoteClone.ViewModel.Commands;
using EvernoteClone.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace EvernoteClone.ViewModel
{
    public class NotesVM
    {
        public ObservableCollection<Notebook> Notebooks { get; set; }

		private Notebook selectedNotebook;
		public Notebook SelectedNotebook
		{
			get { return selectedNotebook; }
			set 
			{ 
				selectedNotebook = value;
				//TODO: get notes
			}
		}

		// dobicemo Notes svaki put kada se SelectedNotebook promeni i sacuvacemo ih u ovoj ObservableCollection
		public ObservableCollection<Note> Notes { get; set; }

		public NewNotebookCommand NewNotebookCommand { get; set; }
		public NewNoteCommand NewNoteCommand { get; set; }

		// konstruktor
		public NotesVM()
		{
			NewNotebookCommand = new NewNotebookCommand(this);
			NewNoteCommand = new NewNoteCommand(this);
		}

		public void CreateNotebook()
		{
			Notebook newNotebook = new Notebook()
			{
				Name = "New notebook"
			};

            DatabaseHelper.Insert(newNotebook);
        }

		public void CreateNote( int notebookId)
		{
			Note newNote = new Note()
			{
				NotebookId = notebookId,
				CreatedTime = DateTime.Now,
				UpdatedTime = DateTime.Now,
				Title = "New note"
			};

			DatabaseHelper.Insert(newNote);

		}
	}
}
