using BLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BLauncher.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        public LoginViewModel LoginVM { get; set; } = new();
        public LauncherViewModel LauncherVM { get; set; } = new();
        private bool firstLaunch = true;

        public bool ShowLauncher
        {
            get 
            {
                if (Cache.CurrentlyLoggedIn.Account != null) 
                {
                    if (firstLaunch && LauncherVM.OnLoad.CanExecute(null))
                        LauncherVM.OnLoad.Execute(null); 
                    firstLaunch = false;
                    return true; 
                } 
                return false; 
            }
        }

        private string selectedAccount;
        public string SelectedAccount
        {
            get => selectedAccount;
            set
            {
                SetProperty(ref selectedAccount, value);
                Cache.UpdateLoggedIn(value);
            }
        }

        private bool isAddingNewAccount;
        public bool IsAddingNewAccount { get => isAddingNewAccount; set => SetProperty(ref isAddingNewAccount, value); }

        private BaseCommand logout;
        public ICommand Logout => logout ??= new(OnLogout);

        private BaseCommand addNewAccount;
        public ICommand AddNewAccount => addNewAccount ??= new(OnAddNewAccount);

        private BaseCommand back;
        public ICommand Back => back ??= new BaseCommand(OnBack);

        public MainWindowViewModel()
        {
            SelectedAccount = Cache.CurrentlyLoggedIn.Account;
            // This doesn't not actually update the value, but signals UI to get the new cached state
            Cache.OnLogin += (account) =>
            {
                UpdateBoundProperty(nameof(ShowLauncher));
                SelectedAccount = account;
            };

        }

        public void OnLogout()
        {
            if(Cache.Logout())
            {
                // Check if there are no accounts left
                if(Cache.CachedLogIns.Count == 0)
                {
                    SelectedAccount = null;
                    // Force UI to show login view
                    UpdateBoundProperty(nameof(ShowLauncher));
                }
            }
        }

        public void OnAddNewAccount()
        {
            Cache.CurrentlyLoggedIn = default;
            UpdateBoundProperty(nameof(ShowLauncher));
            IsAddingNewAccount = true;
            //SelectedAccount = null;
        }

        public void OnBack()
        {
            Cache.UpdateLoggedIn(SelectedAccount);
            UpdateBoundProperty(nameof(ShowLauncher));
            IsAddingNewAccount = false;
        }

    }
}
