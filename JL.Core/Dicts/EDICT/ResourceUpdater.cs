﻿using System.IO.Compression;
using System.Net;

namespace JL.Core.Dicts.EDICT
{
    public static class ResourceUpdater
    {
        public static async Task<bool> UpdateResource(string resourcePath, Uri resourceDownloadUri, string resourceName,
            bool isUpdate, bool noPrompt)
        {
            if (!isUpdate ||
                Storage.Frontend.ShowYesNoDialog($"Do you want to download the latest version of {resourceName}?",
                    "Update dictionary?"))
            {
                HttpRequestMessage request = new(HttpMethod.Get, resourceDownloadUri);

                if (File.Exists(Path.Join(Storage.ApplicationPath, resourcePath)))
                    request.Headers.IfModifiedSince =
                        File.GetLastWriteTime(Path.Join(Storage.ApplicationPath, resourcePath));

                if (!noPrompt)
                    Storage.Frontend.ShowOkDialog(
                        $"This may take a while. Please don't shut down the program until {resourceName} is downloaded.",
                        "");

                HttpResponseMessage response = await Storage.Client.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    await GzipStreamDecompressor(responseStream, Path.Join(Storage.ApplicationPath, resourcePath))
                        .ConfigureAwait(false);

                    if (!noPrompt)
                        Storage.Frontend.ShowOkDialog($"{resourceName} has been downloaded successfully.",
                            "");

                    return true;
                }

                else if (response.StatusCode == HttpStatusCode.NotModified && !noPrompt)
                {
                    Storage.Frontend.ShowOkDialog($"{resourceName} is up to date.",
                        "");
                }

                else if (!noPrompt)
                {
                    Storage.Frontend.ShowOkDialog($"Unexpected error while downloading {resourceName}.",
                        "");
                }
            }

            return false;
        }

        private static async Task GzipStreamDecompressor(Stream stream, string filePath)
        {
            using FileStream decompressedFileStream = File.Create(filePath);
            using GZipStream decompressionStream = new(stream, CompressionMode.Decompress);
            await decompressionStream.CopyToAsync(decompressedFileStream).ConfigureAwait(false);
        }
    }
}