using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace TheLastTime.Shared.Data
{
    public sealed class GoogleDrive : IDisposable
    {
        readonly HttpClient Http;

        readonly IAccessTokenProvider TokenProvider;

        readonly DataService DataService;

        Timer timer = new Timer() { AutoReset = false, Interval = 30 * 1000 };

        public GoogleDrive(HttpClient http, IAccessTokenProvider tokenProvider, DataService dataService)
        {
            Http = http;

            TokenProvider = tokenProvider;

            DataService = dataService;
            DataService.PropertyChanged += PropertyChanged;

            timer.Elapsed += async (object sender, ElapsedEventArgs e) => await Backup();
        }

        void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (DataService.Settings.BackupToGoogleDrive)
            {
                if (e.PropertyName == nameof(DataService.CategoryList) || e.PropertyName == nameof(DataService.HabitList) || e.PropertyName == nameof(DataService.TimeList))
                {
                    timer.Stop();
                    timer.Start();
                }
            }
        }

        private async Task Backup()
        {
            string jsonString = JsonSerializer.Serialize(DataService.CategoryList, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });

            await SaveFile(jsonString);
        }

        public void Dispose()
        {
            DataService.PropertyChanged -= PropertyChanged;
        }

        public async Task SaveFile(string content)
        {
            string folder = await GetFolder();

            if (string.IsNullOrEmpty(folder))
            {
                folder = await CreateFolder();
            }

            string file = await GetFile(folder);

            if (string.IsNullOrEmpty(file))
            {
                file = await CreateFile(folder, content);
            }
            else
            {
                await UpdateFile(file, content);
            }
        }

        private async Task<string> GetFolder()
        {
            string folderId = string.Empty;

            var tokenResult = await TokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                var q = "name = 'ididit' and mimeType = 'application/vnd.google-apps.folder'";
                var url = "https://www.googleapis.com/drive/v3/files?q=" + Uri.EscapeDataString(q);

                var requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var response = await Http.SendAsync(requestMessage);

                var responseStatusCode = response.StatusCode;

                //responseBody = await response.Content.ReadAsStringAsync();

                var data = await response.Content.ReadFromJsonAsync<JsonElement>();

                var files = data.GetProperty("files").EnumerateArray();

                while (files.MoveNext())
                {
                    var file = files.Current;

                    var name = file.GetProperty("name").GetString();

                    if (name == "ididit")
                    {
                        folderId = file.GetProperty("id").GetString() ?? string.Empty;
                    }
                }
            }

            return folderId;
        }

        private async Task<string> GetFile(string folderId)
        {
            string fileId = string.Empty;

            var tokenResult = await TokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                var q = $"'{folderId}' in parents";
                var url = "https://www.googleapis.com/drive/v3/files?q=" + Uri.EscapeDataString(q);

                var requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var response = await Http.SendAsync(requestMessage);

                var responseStatusCode = response.StatusCode;

                //responseBody = await response.Content.ReadAsStringAsync();

                var data = await response.Content.ReadFromJsonAsync<JsonElement>();

                var files = data.GetProperty("files").EnumerateArray();

                while (files.MoveNext())
                {
                    var file = files.Current;

                    var fileName = file.GetProperty("name").GetString();

                    fileId = file.GetProperty("id").GetString() ?? string.Empty;
                }
            }

            return fileId;
        }

        public async Task<string> GetFolders()
        {
            string folders = string.Empty;

            var tokenResult = await TokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                var q = "mimeType = 'application/vnd.google-apps.folder'";
                var url = "https://www.googleapis.com/drive/v3/files?q=" + Uri.EscapeDataString(q);

                var requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var response = await Http.SendAsync(requestMessage);

                var responseStatusCode = response.StatusCode;

                //responseBody = await response.Content.ReadAsStringAsync();

                var data = await response.Content.ReadFromJsonAsync<JsonElement>();

                var files = data.GetProperty("files").EnumerateArray();

                folders = string.Empty;

                while (files.MoveNext())
                {
                    var file = files.Current;

                    var name = file.GetProperty("name").GetString();

                    folders += name + Environment.NewLine;
                }
            }

            return folders;
        }

        private async Task<string> CreateFolder()
        {
            string folderId = string.Empty;

            var tokenResult = await TokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                var requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart"),
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var metaContent = JsonContent.Create(new { name = "ididit", description = "ididit backup", mimeType = "application/vnd.google-apps.folder" });

                var multipart = new MultipartContent { metaContent };

                requestMessage.Content = multipart;

                var response = await Http.SendAsync(requestMessage);

                var responseStatusCode = response.StatusCode;

                //responseBody = await response.Content.ReadAsStringAsync();

                var data = await response.Content.ReadFromJsonAsync<JsonElement>();

                folderId = data.GetProperty("id").GetString() ?? string.Empty;
            }

            return folderId;
        }

        private async Task<string> CreateFile(string folderId, string content)
        {
            string fileId = string.Empty;

            var tokenResult = await TokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                var requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    //RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=media"),
                    RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart"),
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                JsonContent metaContent;

                if (string.IsNullOrEmpty(folderId))
                    metaContent = JsonContent.Create(new { name = "ididit.json", description = "ididit backup" });
                else
                    metaContent = JsonContent.Create(new { name = "ididit.json", description = "ididit backup", parents = new[] { folderId } });

                //var data = new { Title = "Blazor POST Request Example" };
                //string content = JsonSerializer.Serialize(data);

                var fileContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
                //var fileContent = new StringContent(content);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
                //fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
                fileContent.Headers.ContentLength = content.Length;

                var multipart = new MultipartContent { metaContent, fileContent };

                requestMessage.Content = multipart;

                var response = await Http.SendAsync(requestMessage);

                var responseStatusCode = response.StatusCode;

                //responseBody = await response.Content.ReadAsStringAsync();

                var data = await response.Content.ReadFromJsonAsync<JsonElement>();

                fileId = data.GetProperty("id").GetString() ?? string.Empty;
            }

            return fileId;
        }

        private async Task UpdateFile(string fileId, string content)
        {
            var tokenResult = await TokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                var requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Patch,
                    //RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=media"),
                    RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files/" + fileId + "?uploadType=multipart"),
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                JsonContent metaContent = JsonContent.Create(new { name = "ididit.json", description = "ididit backup" });

                //var data = new { Title = "Blazor POST Request Example", DateTime = DateTime.Now };
                //string content = JsonSerializer.Serialize(data);

                var fileContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
                //var fileContent = new StringContent(content);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
                //fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
                fileContent.Headers.ContentLength = content.Length;

                var multipart = new MultipartContent { metaContent, fileContent };

                requestMessage.Content = multipart;

                var response = await Http.SendAsync(requestMessage);

                var responseStatusCode = response.StatusCode;

                //responseBody = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
