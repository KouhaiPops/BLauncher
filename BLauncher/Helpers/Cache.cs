using BLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BLauncher.Helpers
{
    // Implement as singleton instead of static class
    // Implement as singleton instead of static class
    // Implement as singleton instead of static class
    public static class Cache
    {
        public delegate void Login(string account);
        public static ObservableDictionary<string, string> CachedLogIns { get; set; } = new();
        public static (string Account, string Cookie) CurrentlyLoggedIn { get; set; }


        // This is anti-pattern, use proper commands.
        public static event Login OnLogin;
        public static List<string> Cdns { get; } = new()
        {
            "Cdn",
        };

        internal static void NewLogin(string account, string cookie, bool rememberMe)
        {
            // Add the new cookie and account name
            try
            {
                CachedLogIns.Add(account, cookie);
            }
            catch(ArgumentException)
            {
                App.HandleException(new InvalidOperationException($"Account {account} is already logged in.\nPlease logout first and then login."));
                return;
            }
            
            // After a new account is appended to the dictionary, consider it the currently logged in account
            CurrentlyLoggedIn = (account, cookie);
            
            // Invoke the callback if it exists
            OnLogin?.Invoke(account);

            // Check if this pair should be saved to config file
            if(rememberMe)
            {
                App.config.LoggedIn[account] = cookie;
                App.config.CurrentlyLoggedIn = account;

                App.config.Save();
            }
        }
        internal static void UpdateLoggedIn(string account)
        {
            // If acocunt is null signal that this is 'soft` logout.
            if(account == null)
            {
                CurrentlyLoggedIn = default;
                return;
            }
            // This should be assumed to work, only ArgumentOutOfRangeException should be suppressed
            // Otherwise it's a fatal error that the outer handler should report
            try
            {
                CurrentlyLoggedIn = (account, CachedLogIns[account]);
            }
            catch(ArgumentOutOfRangeException)
            {

            }
        }
        internal static bool Logout()
        {
            App.config.LoggedIn.Remove(CurrentlyLoggedIn.Account);
            var result = CachedLogIns.Remove(CurrentlyLoggedIn.Account);
            CurrentlyLoggedIn = default;
            return result;
        }


    }
}
