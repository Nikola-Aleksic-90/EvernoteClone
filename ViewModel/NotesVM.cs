using EvernoteClone.ViewModel.Commands;
using EvernoteClone.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace EvernoteClone.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {
        public ObservableCollection<Notebook> Notebooks { get; set; }

		private Notebook selectedNotebook;
        public Notebook SelectedNotebook
		{
			get { return selectedNotebook; }
			set 
			{ 
				selectedNotebook = value;
				OnPropertyChanged("SelectedNotebook");
				GetNotes();
			}
		}

		// dobicemo Notes svaki put kada se SelectedNotebook promeni i sacuvacemo ih u ovoj ObservableCollection
		public ObservableCollection<Note> Notes { get; set; }

		public NewNotebookCommand NewNotebookCommand { get; set; }
		public NewNoteCommand NewNoteCommand { get; set; }

		// Implementacija interfejsa INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        // konstruktor
        public NotesVM()
		{
			NewNotebookCommand = new NewNotebookCommand(this);
			NewNoteCommand = new NewNoteCommand(this);

			/* Zelimo da imamo inicijalnu vrednost, tj. zelimo da se pri pokretanju
			   prikazu sve kreirane Notebooks */
			Notebooks = new ObservableCollection<Notebook>();
			Notes = new ObservableCollection<Note>();
			GetNotebooks();
		}

		public void CreateNotebook()
		{
			Notebook newNotebook = new Notebook()
			{
				Name = "New notebook"
			};

            DatabaseHelper.Insert(newNotebook);

			GetNotebooks();
        }

		public void CreateNote( int notebookId)
		{
			Note newNote = new Note()
			{
				NotebookId = notebookId,
				CreatedTime = DateTime.Now,
				UpdatedTime = DateTime.Now,
				Title = $"Note for {DateTime.Now.ToString()}"
			};

			DatabaseHelper.Insert(newNote);

			GetNotes();
		}

		private void GetNotebooks()
		{
			var notebooks = DatabaseHelper.Read<Notebook>();

			Notebooks.Clear();
			foreach(var notebook in notebooks)
			{
				Notebooks.Add(notebook);
			}
		}

        private void GetNotes()
        {
			/* U sustini ista metoda kao GetNotebooks() iznad ove metode
			 * Ne zelimo da prikazemo sve notes,
			 * vec samo one koje pripadaju trenutno izabranom Notebook-u.
			 * Zbog toga koristimo LINQ keyword Where.
			 * Nakon toga vidimo da je var notes lokalna varijabla IEnumerable<Note> notes.
			 * Zbog toga stavljamo .ToList() 
			 * Mozda ima bolji ili laksi nacin ali JBG */

			if(SelectedNotebook != null)
			{
				// veliki komentar iznad se odnosi na sledeci red
                var notes = DatabaseHelper.Read<Note>().Where(n => n.NotebookId == SelectedNotebook.Id).ToList();

                Notes.Clear();
                foreach (var note in notes)
                {
                    Notes.Add(note);
                }
            }
            
        }

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
