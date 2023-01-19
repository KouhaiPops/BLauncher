using BLauncher.Helpers;
using BLauncher.Model;

using BLauncherLib;

using MahApps.Metro.Controls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BLauncher.ViewModels
{
    class LauncherViewModel : BaseViewModel
    {
        private bool isChecked = false;
        private bool recheck = false;
        private BurningHttpClient client = new();

        public ObservableProgressBar ProgressBar { get; } = new();

        private string status = LauncherStatus.Check;
        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private float speed = 0;

        public float Speed
        {
            get => speed;
            set => SetProperty(ref speed, value);
        }


        private bool isDropDownOpen = true;
        public bool IsDropDownOpen
        {
            get => isDropDownOpen;
            set => SetProperty(ref isDropDownOpen, value);
        }

        private int selectedUser;

        public int SelectedUser
        {
            get { return selectedUser; }
            set => SetProperty(ref selectedUser, value, onChanged: client.UpdateCookie);
        }


        // This should be moved to ExpandedSplitButton, too lazy
        private BaseCommand expand;
        public ICommand Expand => expand ??= new(OnExpand);
        private void OnExpand() => IsDropDownOpen = !IsDropDownOpen;

        private AsyncBaseCommand force;
        public ICommand Force => force ??= new(CheckVersion);

        private AsyncBaseCommand onLoad;
        public ICommand OnLoad => onLoad ??= new(ExecuteOnLoad);

        private AsyncBaseCommand launchGame;
        public ICommand LaunchGame => launchGame ??= new(Launch);

        public async Task ExecuteOnLoad()
        {
            if (isChecked)
                return;
            recheck = true;
            client.UpdateCookie();
            await CheckVersion();
        }

        private async Task Launch()
        {
            Status = "Launching";
            var serverInfo = await client.GetServerInfo();
            var auth = await client.GetGameAuth();
            var args = $"TOKEN:{auth.Token} HID:{auth.HID} IP:{serverInfo.GameServer} PORT:{serverInfo.GamePort} CHCODE:11";
            var processInfo = new ProcessStartInfo(App.config.GetFileInPath("burningsw.exe"), args)
            {
                WorkingDirectory = App.config.Gamepath
            };
            Process.Start(processInfo);
            Status = "Launched";
        }

        private async Task CheckVersion()
        {
            status = LauncherStatus.Check;
            (VersionFile vfile, string hash) = await client.GetVersionFile();
            var versionFilePath = App.config.GetFileInPath("version.bin");
            if (File.Exists(versionFilePath))
            {
                var bytes = await File.ReadAllBytesAsync(versionFilePath);
                recheck = HashProvider.QuickHash(bytes) != hash;
            }

            if (recheck)
            {
                await CheckFiles(vfile);
            }
            else
            {
                Status = "Ready";
                ProgressBar.Progress = 100;
            }
        }

        private async Task CheckFiles(VersionFile vfile)
        {
            var files = await CheckIntegrity(vfile);
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    VersionEntry file = files[i];
                    ProgressBar.Progress = 0;
                    Status = $"Downloading: [{i+1}/{files.Length}] [{file.Name}]";
                    await client.DownloadFile(file.Name, ProgressBar, (s) => Speed = s);
                }
                Status = LauncherStatus.Ready;
            }
            catch (Exception)
            {
                Status = LauncherStatus.Error;
                throw;
            }
            ProgressBar.Progress = 100;
        }

        private async Task<VersionEntry[]> CheckIntegrity(VersionFile vfile)
        {
            Status = LauncherStatus.Check;
            var filesToUpdate = new List<VersionEntry>();
            foreach(var file in vfile.Files)
            {
                var fullPath = App.config.GetFileInPath(file.Name);
                if ((await HashProvider.FileHash(fullPath)) is { } fileHash && fileHash == file.Hash)
                    continue;
                filesToUpdate.Add(file);
            }
            return filesToUpdate.ToArray();
        }
    }
}
