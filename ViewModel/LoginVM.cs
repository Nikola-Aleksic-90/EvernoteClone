using EvernoteClone.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace EvernoteClone.ViewModel
{
    public class LoginVM
    {
		private User user;
		public User User
		{
			get { return user; }
			set { user = value; }
		}

		public RegisterCommand RegisterCommand { get; set; }
		public LoginCommand LoginCommand { get; set; }

		// konstruktor
		public LoginVM()
		{
			// prosledjujemo this jer konstruktor za nasu RegisterCommand klasu sada zahteva LoginVM instancu
			RegisterCommand = new RegisterCommand(this);
			LoginCommand = new LoginCommand(this);
		}
	}
}
