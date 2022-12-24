using EvernoteClone.ViewModel.Commands;
using EvernoteClone.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EvernoteClone.Model;

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

		private Note selectedNote;
		public Note SelectedNote
		{
			get { return selectedNote; }
			set 
			{
                selectedNote = value;
                OnPropertyChanged("SelectedNote");
				SelectedNoteChanged?.Invoke(this, new EventArgs());
            }
		}

		private Visibility isVisible;
		public Visibility IsVisible
		{
			get { return isVisible; }
			set 
			{ 
				isVisible = value;
				OnPropertyChanged("IsVisible");
			}
		}

		// dobicemo Notes svaki put kada se SelectedNotebook promeni i sacuvacemo ih u ovoj ObservableCollection
		public ObservableCollection<Note> Notes { get; set; }

		public NewNotebookCommand NewNotebookCommand { get; set; }
		public NewNoteCommand NewNoteCommand { get; set; }
		public EditCommand EditCommand { get; set; }
		public EndEditingCommand EndEditingCommand { get; set; }

		// Implementacija interfejsa INotifyPropertyChanged
		public event PropertyChangedEventHandler? PropertyChanged;

		public event EventHandler SelectedNoteChanged;

        // konstruktor
        public NotesVM()
		{
			NewNotebookCommand = new NewNotebookCommand(this);
			NewNoteCommand = new NewNoteCommand(this);
			EditCommand = new EditCommand(this);
			EndEditingCommand= new EndEditingCommand(this);

			// pri pokretanju zelimo da je Collapsed
			IsVisible = Visibility.Collapsed;

			/* Zelimo da imamo inicijalnu vrednost, tj. zelimo da se pri pokretanju
			   prikazu sve kreirane Notebooks */
			Notebooks = new ObservableCollection<Notebook>();
			Notes = new ObservableCollection<Note>();
			GetNotebooks();
		}

		public async void CreateNotebook()
		{
			Notebook newNotebook = new Notebook()
			{
				Name = "New notebook",
				UserId = App.UserId
			};

            await DatabaseHelper.Insert(newNotebook);

			GetNotebooks();
        }

		public async void CreateNote(string notebookId)
        {
			Note newNote = new Note()
			{
				NotebookId = notebookId,
				CreatedTime = DateTime.Now,
				UpdatedTime = DateTime.Now,
				Title = $"Note for {DateTime.Now.ToString()}"
			};

			await DatabaseHelper.Insert(newNote);

			GetNotes();
		}

		public async void GetNotebooks()
        {
            var notebooks = (await DatabaseHelper.Read<Notebook>()).Where(n => n.UserId == App.UserId).ToList();

            Notebooks.Clear();
			foreach(var notebook in notebooks)
			{
				Notebooks.Add(notebook);
			}
		}

        private async void GetNotes()
        {
			if(SelectedNotebook != null)
			{
                /* U sustini ista metoda kao GetNotebooks() iznad ove metode
				* Ne zelimo da prikazemo sve notes,
				* vec samo one koje pripadaju trenutno izabranom Notebook-u.
				* Zbog toga koristimo LINQ keyword Where.
				* Nakon toga vidimo da je var notes lokalna varijabla IEnumerable<Note> notes.
				* Zbog toga stavljamo .ToList() 
				* Mozda ima bolji ili laksi nacin ali JBG */
                var notes = (await DatabaseHelper.Read<Note>()).Where(n => n.NotebookId == SelectedNotebook.Id).ToList();

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

		// metoda da se preimenuju nazivi Notebook-ova
		public void StartEditing()
		{
			IsVisible = Visibility.Visible;
		}

		// Metoda da se snime izmene naziva Notebook-ova
        public void StopEditing(Notebook notebook)
        {
            IsVisible = Visibility.Collapsed;
			DatabaseHelper.Update(notebook);
            GetNotebooks();
        }
    }
}
