using System;
using System.IO;
using System.Xml.Serialization;
using Yandex.Music.Api;
using Yandex.Music.Client;

namespace TermStyle
{
    public class LoginPanel
    {
        public const string UserFile = "user.xml";
        //private YandexApi _api;
        private YandexMusicClient _api;
        
        public LoginPanel(YandexMusicClient api)//(YandexApi api)
        {
            _api = api;
        }

        public bool Login()
        {
            if (!IsLogin())
            {
                Console.Clear();
                Console.WriteLine("Your need authorize into Yandex.Music");
                Console.Write("Please write your login: ");

                var login = Console.ReadLine();

                Console.WriteLine("Thanks");
                Console.Write("Please write your password: ");

                var pass = Console.ReadLine();

                var user = new User
                {
                    Login = login,
                    Password = pass
                };
                
                Console.Clear();
                Console.WriteLine($"Authorize...");

                var isAuth = _api.Authorize(user.Login, user.Password);

                Console.WriteLine($"Authorize: {isAuth}");

                if (isAuth)
                {
                    using (var stream = new FileStream(UserFile, FileMode.Create))
                    {
                        var serializer = new XmlSerializer(typeof(User));

                        serializer.Serialize(stream, user);
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Error pass or login");
                    Login();
                }
            }
            else
            {
                var user = default(User);
                
                Console.WriteLine($"Authorize...");
                
                using (var stream = new FileStream(UserFile, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(User));

                    user = (User) serializer.Deserialize(stream);
                }
                
                var isAuth = _api.Authorize(user.Login, user.Password);
                Console.Clear();
                Console.WriteLine($"Authorize: {isAuth}");
                Console.WriteLine($"Hello, {user.Login}");
            }

            return true;
        }

        private bool IsLogin() => File.Exists(UserFile);
    }
}