#define COOKIE
using BLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BLauncher.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        private const string LOGIN_URL = "https://burningsw.com/login";
        //public LoginViewModel(PasswordBox passwordBox)
        //{
        //    this.passwordBox = passwordBox;
        //}

        private PasswordBox passwordBox;
        public PasswordBox PasswordBox
        {
            get => passwordBox;
            set
            {
                passwordBox ??= value;
            }

        }

        private string account;
        public string Account
        {
            get => account;
            set => SetProperty(ref account, value);
        }

        private bool isConnecting;

        public bool IsConnecting
        {
            get { return isConnecting; }
            set { isConnecting = value; loginCommand.OnCanExecuteChanged(this); }
        }
        private BaseCommand loginCommand;
        public ICommand LoginCommand => loginCommand ??= new BaseCommand(OnLogin, () => !IsConnecting);

        private bool rememberMe = true;

        public bool RememberMe
        {
            get => rememberMe;
            set => SetProperty(ref rememberMe, value);
        }


        private async void OnLogin()
        {
            // All exceptions are handled by the global propegated to the global exception handler
            // This is an anti-pattern and should be avoided
            try
            {
#if COOKIE
                // Check if account name wasn't provided
                if(string.IsNullOrWhiteSpace(Account))
                {
                    throw new Exception("Username is empty");
                }
                var password = passwordBox.Password;
                // Check if password wasn't provided
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new Exception("Password is empty");
                }

                // Set is connecting to true
                IsConnecting = true;

                // initialize http/s client handler
                var handler = new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    UseCookies = false
                };

                // Should use a singleton instead of create a new instance
                using var client = new HttpClient(handler);

                var form = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"username", account}, {"password", password}
                });

                var request = new HttpRequestMessage(HttpMethod.Post, LOGIN_URL)
                {
                    Content = form
                };

                // Send the request
                var response = await client.SendAsync(request);

                // Set is connecting to false, even if the connection failed
                IsConnecting = false;

                // Throw an exception if the login request doesn't redirect
                if(response.StatusCode != HttpStatusCode.Moved)
                {
                    throw new Exception("Invalid Account or Password");
                }

                // Get cookies header

                var cookie = response.Headers.SingleOrDefault(header => String.Equals(header.Key, "set-cookie", StringComparison.OrdinalIgnoreCase)).Value.FirstOrDefault();
                if (cookie == null)
                {
                    throw new Exception("Invalid Account or Password");
                }
#else

#endif
                if(!cookie.Contains('='))
                {
                    throw new Exception($"Unknown cookie struct: {cookie}");
                }
                if (!rememberMe
                    && MessageBox.Show(
                        "Remember me isn't checked, the next time you'd have to re-enter credentials\nCheck the box?",
                        "Remember me",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes,
                        MessageBoxOptions.DefaultDesktopOnly) == MessageBoxResult.Yes)
                {
                    rememberMe = true;
                }
                Cache.NewLogin(Account, cookie, rememberMe);
            }
            catch(Exception e)
            {
                App.HandleException(e);
                isConnecting = false;
            }
        }

    }
}
