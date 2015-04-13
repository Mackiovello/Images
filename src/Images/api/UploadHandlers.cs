﻿using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Win32;
using Starcounter;
using Starcounter.Internal;

namespace Images {
    internal class UploadHandlers {
        //Maximum file size in bytes.
        public static int MaxFileSize = 1048576; //1mb
        public static string[] AllowedMimeTypes = { "image/gif", "image/jpeg", "image/png", "image/svg+xml" };

        public void Register() {

            // Upload media
            Handle.POST("/images/images", (Request request) => {
                string mime;
                string encoding;
                string data;

                ushort port = StarcounterEnvironment.Default.UserHttpPort;
                string host = request["Host"];

                if (!host.Contains(":")) {
                    host += ":" + port;
                }

                string fileJson = Handle.IncomingRequest["x-file"];

                xFile xfile = new xFile();
                xfile.PopulateFromJson(fileJson);

                ParseDataUri(request.Body, out mime, out encoding, out data);

                if (encoding != "base64") {
                    Handle.SetOutgoingStatusCode(422); //Unprocessable Entity
                    Handle.SetOutgoingStatusDescription("Unfortunately, we don't support that file type. Try again with a PNG, GIF, or JPG.");

                    return 422;
                }

                if (!AllowedMimeTypes.Contains(mime)) {
                    Handle.SetOutgoingStatusCode((ushort)System.Net.HttpStatusCode.UnsupportedMediaType);
                    Handle.SetOutgoingStatusDescription("Unfortunately, we don't support " + mime + " file type. Try again with a PNG, GIF, or JPG.");

                    return (ushort)System.Net.HttpStatusCode.UnsupportedMediaType;
                }

                byte[] _ByteArray = Convert.FromBase64String(data);

                // Create Full filename path
                string extention = GetDefaultExtension(mime);
                string fileName = System.IO.Path.GetRandomFileName() + extention;

                // NOTE: The filename should not contain an ')'
                // If so then the client will not properly parse out the text

                string filePath = System.IO.Path.Combine(Starcounter.Application.Current.WorkingDirectory, "media");

                if (!Directory.Exists(filePath)) {
                    Directory.CreateDirectory(filePath);
                }

                filePath = System.IO.Path.Combine(filePath, fileName);

                // Save data to file
                System.IO.FileStream _FileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);
                _FileStream.Close();

                Handle.AddOutgoingHeader("x-file-location", "/media/" + fileName);
                Handle.AddOutgoingHeader("x-file", System.Text.Encoding.Default.GetString(xfile.ToJsonUtf8()));

                return System.Net.HttpStatusCode.Created;
            });
        }

        /// <summary>
        /// Parse Data URI
        /// FORMAT: data:[<mime type>][;charset=<charset>][;base64],<encoded data>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mime"></param>
        /// <param name="encoding"></param>
        /// <param name="data"></param>
        public void ParseDataUri(string input, out string mime, out string encoding, out string data) {

            var regex = new Regex(@"data:(?<mime>[\w/\-\.]+);(?<encoding>\w+),(?<data>.*)", RegexOptions.Compiled);
            var match = regex.Match(input);

            mime = match.Groups["mime"].Value;
            encoding = match.Groups["encoding"].Value;
            data = match.Groups["data"].Value;
        }

        /// <summary>
        /// Get File Extention assosiated to a mime type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public string GetDefaultExtension(string mimeType) {

            string result;
            RegistryKey key;
            object value;

            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }

    }
}