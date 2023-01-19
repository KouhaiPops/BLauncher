using BLauncher.Helpers;

using BLauncherLib;

using S2Dotnet;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLauncher.Model
{
    public class BurningHttpClient
    {
        private const string VersionUrl = "version.bin";
        private const string BaseCDN = "https://cdn.burningsw.com/";
        private const string BaseUrl = "https://burningsw.com/";
        // Should reuse client instead of instantiating a new one
        private HttpClient client = new();
        private HttpRequestMessage request = new(HttpMethod.Get, "");

        public int SelectedCdn { get; set; }


        public void UpdateCookie()
        {
            request.Headers.Remove("Cookie");
            request.Headers.Add("Cookie", Cache.CurrentlyLoggedIn.Cookie);
        }
        internal async Task<GameAuth> GetGameAuth()
        {
            request = new();
            UpdateCookie();
            request.Method = HttpMethod.Post;
            request.RequestUri = new(BaseUrl + "api/generate_token");
            using var req = await client.SendAsync(request);
            var auth = (await req.Content.ReadAsStringAsync()).Split("=");
            return new GameAuth(auth[0][..auth[0].IndexOf("&")], auth[1]);

        }

        internal async Task<ServerInfo?> GetServerInfo()
        {
            return await client.GetFromJsonAsync<ServerInfo>("https://launcher.burningsw.com/info.json");
        }
        internal async Task DownloadFile(string name, IProgress<float> progress, Action<float> onReportDownloadSpeed)
        {
            using var req = await client.GetAsync(BaseCDN + name, HttpCompletionOption.ResponseHeadersRead);
            var stream = await req.Content.ReadAsStreamAsync();
            if (!req.IsSuccessStatusCode)
                return;
            var contentLength = req.Content.Headers.ContentLength.Value;
            IProgress<long> relativeProgress = new Progress<long>(totalBytes => progress.Report(MathF.Min(((float)totalBytes / contentLength)*100, 100f)));
            var reportedStream = new ReporterStream(stream);
            reportedStream.Report += onReportDownloadSpeed;
            var tempPath = App.config.GetFileInPath(name + ".tmp");
            var tempFilestream = File.Create(tempPath);
            await EncryptedFile.DownloadFile(reportedStream, relativeProgress, tempFilestream);
            tempFilestream.Dispose();
            
            using var fileStream = File.Create(App.config.GetFileInPath(name));
            var res = await EncryptedFile.DecryptFileAndHash(File.Open(tempPath, FileMode.Open), relativeProgress, fileStream, default);
            File.Delete(tempPath);
        }

        internal async Task<(VersionFile VFile, string Hash)> GetVersionFile()
        {
            var bytes = await client.GetByteArrayAsync(BaseCDN + VersionUrl);
            var versionPath = App.config.GetFileInPath("version.bin.temp");
            //var bytes = await File.ReadAllBytesAsync(versionPath);
            var vfile = Parser.ParseEncVersionFile(bytes);
            await File.WriteAllBytesAsync(versionPath, bytes);
            return (vfile, HashProvider.QuickHash(bytes));

        }

        private void Check()
        {
            // Initialize the version request

        }

        //private async ValueTask<byte[]> GetVersion()
        //{
        //    UpdateCDNRequest(VersionUrl);
        //    var response = await client.SendAsync(request);
        //}

        private void UpdateCDNRequest(string subUrl)
        {
            request.RequestUri = new($"https://cdn{SelectedCdn}.burningsw.to/${subUrl}");
        }

        private void UpdateRequest(string url)
        {
            request.RequestUri = new Uri(url);
        }

    }
}
