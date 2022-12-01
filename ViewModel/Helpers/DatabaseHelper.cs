using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteClone.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        // Environment.CurrentDirectory pokazuje na lokaciju gde ce se nalaziti nas projekat
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "notesDb.db3");

        // Genericka metoda za ubacivanje bilo kog objekta u svoju tabelu
        // Ovakva metoda se moze koristiti i za User i za Note i za Notebook
        public static bool Insert<T>(T item)
        {
            bool result = false;

            // using statement je da se kreira konekcija, obavljaju potrebne radnje i na kraju zatvori conn konekcija
            using(SQLiteConnection conn= new SQLiteConnection(dbFile))
            {
                conn.CreateTable<T>();

                //Insert metoda je tipa int koja vraca broj dodatih redova u tabeli.
                //Ako je kreiran neki red onda znaci da result treba da postane true
                int rows = conn.Insert(item);
                if (rows > 0)
                {
                    result = true;
                }
            }

            return result;
        }


        public static bool Update<T>(T item)
        {
            bool result = false;

            using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            {
                conn.CreateTable<T>();

                //Update metoda je tipa int koja vraca broj azuriranih redova u tabeli
                int rows = conn.Update(item);
                if (rows > 0)
                {
                    result = true;
                }
            }

            return result;
        }


        public static bool Delete<T>(T item)
        {
            bool result = false;

            using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            {
                conn.CreateTable<T>();

                //Delete metoda je tipa int koja vraca broj izbrisanih redova u tabeli
                int rows = conn.Delete(item);
                if (rows > 0)
                {
                    result = true;
                }
            }

            return result;
        }


        public static List<T> Read<T>() where T : new()
        {
            List<T> items;

            using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            {
                conn.CreateTable<T>();
                items = conn.Table<T>().ToList();
            }

            return items;
        }

    }
}
