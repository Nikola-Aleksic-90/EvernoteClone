using EvernoteClone.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EvernoteClone.ViewModel.Helpers
{
    public class FirebaseAuthHelper
    {
        // bice static umesto constant ( u odnosu na AccuWeather aplikaciju )
        private static string api_key = "AIzaSyAN6G5kZvUnPYSWyXka1UkJFsmkNrZztzc";

        // posto u async ne mozemo da radimo sa bool-om koristicemo Task<bool>
        public static async Task<bool> Register(User user)
        {
            using(HttpClient client = new HttpClient() )
            {
                // kreiramo new anonymus object
                var body = new
                {
                    // Firebase nam trazi sledece stvari
                    email = user.Username,
                    password = user.Password,
                    returnSecureToken = true    // uvek mora da je true
                };

                // Ovo gore ne mozemo da saljemo kao c# nego kao JSON
                // u WeatherApp smo deserijalizovali JSON objekte, sada suprotno
                // Zato opet koristimo Newtonsoft.Json NuGet package
                // Po default-u Intelisense daje JsonConverts (s na kraju), pre toga ukucaj JsonConvert i dodaj using statement
                string bodyJson = JsonConvert.SerializeObject(body);

                // Da bi ga poslali preko POST REQUEST treba nam string content
                var data = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                // Sada pravimo REQUEST
                // uvek mora https inace ce fail-ovati
                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:sugnUp?key={api_key}", data);

                // da vidimo da li je RESPONSE uspesan, tacnije da li je REQUEST uspesan
                // Dobicemo ID pod kojim je User kreiran sto je kljucno jer cemo to koristiti za kreiranje novih Notebook-ova
                if (response.IsSuccessStatusCode)
                {
                    string resultJson = await response.Content.ReadAsStringAsync();

                    // sada deserijalizujemo JSON
                    // to radimo preko JSON Utils kao u WeatherApp aplikaciji i rezultat je dat kao posebna FirebaseResult klasa ispod

                    // Nakon kreiranje FirebaseResult klase nastavljamo dalje

                    var result = JsonConvert.DeserializeObject<FirebaseResult>(resultJson);
                    App.UserId = result.localId;

                    return true;
                }
                // else slucaj je za kada nemamo dobar REQUEST ili RESPONSE
                // ili ako user postoji ako password nije dobar itd...
                else
                {
                    string errorJson = await response.Content.ReadAsStringAsync();

                    // klase za gresku su date ispod FirebaseResult klase i to ErrorDetails i Error
                    var error = JsonConvert.DeserializeObject<Error>(errorJson);
                    MessageBox.Show(error.error.message);

                    return false;
                }
            }
        }

        public class FirebaseResult
        {
            public string kind { get; set; }
            public string idToken { get; set; }
            public string email { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
            public string localId { get; set; }     // Id pod kojm je kreiran User
        }

        public class ErrorDetails
        {
            public int code { get; set; }
            public string message { get; set; }
        }

        public class Error
        {
            public ErrorDetails error { get; set; }
        }

    }
}
