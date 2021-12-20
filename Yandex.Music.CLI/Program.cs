using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Yandex.Music.Api;
using Yandex.Music.Api.Common;
using Yandex.Music.Api.Models;
using Yandex.Music.Api.Models.Playlist;
using Yandex.Music.Api.Models.Track;
using Yandex.Music.Client;

namespace TermStyle
{
    class Program
    {
        static void Main(string[] args)
        {

            string login;
            string password;
            
            if (args.Length > 0)
            {
                login = args[0];
                password = args[1];
            }
            else
            {
                Console.WriteLine("Enter login:");
                login = Console.ReadLine();
                Console.WriteLine("Enter password:");
                password = Console.ReadLine();
                Console.Clear();
            }
            DebugSettings debugSettings = new DebugSettings(@"C:\yandex_music", @"C:\yandex_music\log.txt");
            debugSettings.Clear();
            AuthStorage authStorage = new AuthStorage(debugSettings);

            authStorage.User.Login = login;
            var api = new YandexMusicApi();
            api.User.Authorize(authStorage, authStorage.User.Login, password);
            if (authStorage.IsAuthorized)
            {
                Console.WriteLine("Authorization succesful!");
            }
            else
            {
                Console.WriteLine("Wrong login or password.");
                return;
            }

            Console.WriteLine("What is track name?");
            string searchingQuery = Console.ReadLine();
            var search = api.Search.Track(authStorage, searchingQuery);

            if (search.Result.Tracks == null)
            {
                Console.WriteLine("Nothing is found... :(");
                return;
            }

            int count = 1;
            foreach (var item in search.Result.Tracks.Results)
            {
                Console.WriteLine($"{count}. {item.Artists[0].Name} - {item.Title}");
                count++;
            }

            Console.WriteLine("Choose track number from list:");
            int choice = Convert.ToInt32(Console.ReadLine());
            if (choice < 1 || choice > 20)
            {
                Console.WriteLine("Your entered wrong number.");
                return;
            }

            YTrack track = api.Track.Get(authStorage, search.Result.Tracks.Results[choice - 1].Id).Result[0];

            var fileLink = api.Track.GetFileLink(authStorage, track);

            using (var client = new WebClient())
            {
                client.DownloadFile(fileLink, @$"{debugSettings.OutputDir}\{track.Title}.mp3");
            }
            var audio = new AudioPlayer2(authStorage, track);
            
            audio.Play();


            /*
            YandexMusicClient api = new YandexMusicClient();

            LoginPanel login = new LoginPanel(api);
            
            login.Login();
            
            Console.WriteLine("Extracting Metallica's popular tracks...");

            // Metallica
            Yandex.Music.Api.Models.Artist.YArtistBriefInfo TTT = api.GetArtist("3121"); 

            //List<YandexMusicClient> 
            List<Yandex.Music.Api.Models.Track.YTrack> tracks = null;
            
            try
            {
                
                tracks = TTT.PopularTracks;
            }
            catch (Exception ex1)
            {
                Debug.WriteLine("Exception: " +  ex1.Message);
            }

            if (tracks != null)
            {
                Int32 index = 1;
                List<string[]> tableData = new List<string[]>();
                Int32  border = 20;

                tracks.ForEach(x =>
                {
                    string[] list = new string[4];

                    string title = x.Title;
                    
                    if (title.Length >= border)
                        title = $"{title.Substring(0, border)}...";

                    string artist = x.Artists.First().Name;//  ..First().Name;
                    
                    //if (artist.Length >= 5)
                    //    artist = $"{artist.Substring(0, 5)}...";
                    
                    string duration = ((long)(x.DurationMs)).ToString();

                    //if (duration.Length >= 5)
                    //    duration = $"{duration.Substring(0, 5)}...";

                    list[0] = index.ToString();
                    list[1] = title;
                    list[2] = artist;
                    list[3] = duration;

                    tableData.Add(list);

                    index++;
                });

                Console.Clear();
                Table table = new Table();
                table.SetHeader("#", "TITLE", "ARTIST", "DURATION");
                table.SetData(tableData);
                table.Show();



                Console.WriteLine("Какой трек Вы хотите прослушать?");
                string searchingQuery = Console.ReadLine();
                var search = api.Search.Track(authStorage, searchingQuery);

                if (search.Result.Tracks == null)
                {
                    Console.WriteLine("По Вашему запросу ничего не найдено :(");
                    return;
                }


                int count = 1;
                foreach (var item in search.Result.Tracks.Results)
                {
                    Console.WriteLine($"{count}. {item.Artists[0].Name} - {item.Title}");
                    count++;
                }

                Console.WriteLine("Выберите трек из списка:");
                int choice = Convert.ToInt32(Console.ReadLine());
                if (choice < 1 || choice > 20)
                {
                    Console.WriteLine("Вы ввели неверное число.");
                    return;
                }

                YTrack track = api.Track.Get(authStorage, search.Result.Tracks.Results[choice - 1].Id).Result[0];

                var fileLink = api.Track.GetFileLink(authStorage, track);

                using (var client = new WebClient())
                {
                    client.DownloadFile(fileLink, @$"{debugSettings.OutputDir}\{track.Title}.mp3");
                }
                var audio = new AudioPlayer(authStorage, track);
                
                audio.Play();

            }
            else
            {
                Console.WriteLine("No tracks found... so strange!");
            }
            */

            Console.ReadKey();
        }
    }
}