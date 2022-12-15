using EvernoteClone.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteClone.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        // Environment.CurrentDirectory pokazuje na lokaciju gde ce se nalaziti nas projekat
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "notesDb.db3");

        // Od sada koristimo Google Firebase Realtime Database
        private static string dbPath = "https://notes-app-wpf-191ef-default-rtdb.europe-west1.firebasedatabase.app/";

        // Genericka metoda za ubacivanje bilo kog objekta u svoju tabelu
        // Ovakva metoda se moze koristiti i za User i za Note i za Notebook
        // Kod ispod je za lokalnu SQLite database i bice izkomentarisan jer od sada koristimo Google Firebase
        /*
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
        */

        // Google Firebase 
        public static async Task<bool> Insert<T>(T item)
        {
            string jsonBody = JsonConvert.SerializeObject(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            using(var client = new HttpClient())
            {
                // item.GetType().Name.ToLower() koristimo jer ne znamo sta prosledjujemo (Notebook, Note ...) a ToLower je da se sve pise malim slovima
                var result = await client.PostAsync($"{dbPath}{item.GetType().Name.ToLower()}.json", content);

                if (result.IsSuccessStatusCode) return true;
                else return false;
            }
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

        // Sqlite
        /* Posto je u conn.Table<T>() potrebno da je T "non-abstract type with a public parameterless constructor"
           kako bi smo koristili parametar T u generic metodi SQLiteConection.Table<T> onda cemo na pocetku metode dodati : new() */
        /*
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
        */

        // Google Firebase
        // Vidi lekciju 109 za objasnjenja oko Dictionary
        public static async Task<List<T>> Read<T>() where T : HasId
        {
            using( var client = new HttpClient())
            {
                var result = await client.GetAsync($"{dbPath}{typeof(T).Name.ToLower()}.json");
                var jsonResult = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    var objects = JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonResult);

                    List<T> list = new List<T>();
                    foreach(var o in objects)
                    {
                        o.Value.Id = o.Key;
                        list.Add(o.Value);
                    }

                    return list;
                }
                else return null;
            }
        }

    }
}
