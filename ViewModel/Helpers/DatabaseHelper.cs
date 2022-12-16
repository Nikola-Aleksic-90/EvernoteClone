using EvernoteClone.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteClone.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        // Google Firebase Realtime Database
        private static string dbPath = "https://notes-app-wpf-191ef-default-rtdb.europe-west1.firebasedatabase.app/";

        public static async Task<bool> Insert<T>(T item)
        {
            string jsonBody = JsonConvert.SerializeObject(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // using statement je da se kreira konekcija, obavljaju potrebne radnje i na kraju zatvori conn konekcija
            using (var client = new HttpClient())
            {
                // item.GetType().Name.ToLower() koristimo jer ne znamo sta prosledjujemo (Notebook, Note ...) a ToLower je da se sve pise malim slovima
                var result = await client.PostAsync($"{dbPath}{item.GetType().Name.ToLower()}.json", content);

                if (result.IsSuccessStatusCode) return true;
                else return false;
            }
        }

        public static async Task<bool> Update<T>(T item) where T : HasId
        {
            string jsonBody = JsonConvert.SerializeObject(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                // item.GetType().Name.ToLower() koristimo jer ne znamo sta prosledjujemo (Notebook, Note ...) a ToLower je da se sve pise malim slovima
                var result = await client.PatchAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);

                if (result.IsSuccessStatusCode) return true;
                else return false;
            }
        }

        public static async Task<bool> Delete<T>(T item) where T : HasId
        {
            using (var client = new HttpClient())
            {
                // item.GetType().Name.ToLower() koristimo jer ne znamo sta prosledjujemo (Notebook, Note ...) a ToLower je da se sve pise malim slovima
                var result = await client.DeleteAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");

                if (result.IsSuccessStatusCode) return true;
                else return false;
            }
        }

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
