using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Threading.Tasks;
using System.Threading;
using Google.Apis.Upload;

namespace _1911_Anti_Cheat
{
    static class ZipUploader
    {
        // TODO: fill these in.
        private const string AccessToken = "";
        private const string ServiceAccountEmail = "";
        private const string DirectoryId = "";


        public static void DoIt(string path, string playerName)
        {
            string zipPath = ZipDirectory(path, playerName);
            // TODO: Fix Google Drive upload not working
            // UploadToGoogleAsync(zipPath);
        }


        static string ZipDirectory(string path, string playerName)
        {
            string zipPath = Path.Combine(Path.GetTempPath(), playerName + "_" + AntiCheatForm.GetTimestamp + ".zip");
            ZipFile.CreateFromDirectory(path, zipPath);
            return zipPath;
        }


        static async Task UploadToGoogleAsync(string zipPath)
        {
            string zipFileName = Path.GetFileName(zipPath);

            // Load the Service account credentials and define the scope of its access.
            var credential = GoogleCredential.FromAccessToken(AccessToken)
                .CreateScoped(DriveService.ScopeConstants.Drive);

            // Create the  Drive service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            // Upload file Metadata
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = zipFileName,
                Parents = new List<string> { }
            };

            string uploadedFileId;

            // Create a new file on Google Drive
            await using (var fsSource = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            {
                // Create a new file, with metadata and stream.
                var request = service.Files.Create(fileMetadata, fsSource, "application/zip");
                request.Fields = "*";
                var results = await request.UploadAsync(CancellationToken.None);

                if (results.Status == UploadStatus.Failed)
                {
                    Console.WriteLine($"Error uploading file: {results.Exception.Message}");
                }

                // the file id of the new file we created
                uploadedFileId = request.ResponseBody?.Id;
            }
        }
    }
}
