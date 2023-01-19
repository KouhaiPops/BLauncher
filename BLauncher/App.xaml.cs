using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using BLauncher.Model;
using Microsoft.Win32;

namespace BLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static string COOKIE;
        public static readonly Config config = Config.Load();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bool isValidConfig = config.IsValid();


            if (!isValidConfig)
            {
                while (true)
                {
                    // Show message to inform user
                    var prompt = System.Windows.MessageBox.Show("Game folder wasn't found in config, specifiy game folder", "Game folder", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, System.Windows.MessageBoxOptions.DefaultDesktopOnly);

                    if (prompt == MessageBoxResult.No)
                    {
                        Environment.Exit(1);
                    }

                    // Dialog used to get folder
                    using var dialog = new FolderBrowserDialog()
                    {
                        Description = "Game folder",
                        ShowNewFolderButton = true
                    };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        // Add path to config
                        config.Gamepath = dialog.SelectedPath;

                        // If path is valid save it and show main window
                        if(config.IsValid())
                        {
                            config.Save();
                            break;
                        }
                    }
                }
            }


#if OLD
                // Query if offical launcher is installed
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\burningsw\shell\open\command");
                if(key == null)
                {
                    HandleException(new Exception("Couldn't find game folder, specify a folder to install to"));
                    Environment.Exit(1);
                }
                var valNames = key.GetValue("");
                if(valNames is string val && !string.IsNullOrWhiteSpace(val))
                {
                    var gamePath = string.Join('\\', val[0..^5].Split('\\')[..^1]);
                    if(Directory.Exists(gamePath))
                    {
                        config = new Config()
                        {
                            Gamepath = gamePath
                        };
                        File.WriteAllText("config.json", JsonSerializer.Serialize<Config>(config));
                    }
                    else
                    {
                        HandleException(new Exception("Couldn't find game folder, specify a folder to install to"));
                        System.Environment.Exit(1);
                    }
                }
            }
#endif
            SetupHandler();
        }

        private void SetupHandler()
        {

            AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException((Exception)e.ExceptionObject);
            DispatcherUnhandledException += (_, e) =>
            {
                HandleException(e.Exception);
                e.Handled = true;
            };
            TaskScheduler.UnobservedTaskException += (_, e) => HandleException(e.Exception);
        }
        
        public static void HandleException(Exception e)
        {
            System.Windows.MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
