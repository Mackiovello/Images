using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Starcounter;

namespace Image {
    public class ImageUpload {
        public static void RegisterHandlers() {

            // Upload media
            Handle.POST("/Images/images", (Request request) => {
                string mime;
                string encoding;
                string data;

                ushort port = 8080;
                string host = request["Host"];
                if (host.IndexOf(':') == -1) {
                    host += ":" + port;
                }

                var xFile = request["x-file"];

                Images.JSON.xFile xfile = new Images.JSON.xFile();
                xfile.PopulateFromJson(xFile);

                ImageUpload.ParseDataUri(request.Body, out mime, out encoding, out data);

                if (encoding != "base64") {
                    return new Response() {
                        StatusCode = 422, //  Unprocessable Entity
                        StatusDescription = "Unfortunately, we don't support that file type.  Try again with a PNG, GIF, or JPG."
                    };
                }

                string[] supportedMimeTypes = { "image/gif", "image/jpeg", "image/png", "image/svg+xml" };
                bool bMimeTypeSupport = false;
                // Supported img mime types
                foreach (string mimetype in supportedMimeTypes) {
                    if (mime == mimetype) {
                        bMimeTypeSupport = true;
                        break;
                    }
                }

                if (bMimeTypeSupport == false) {
                    return new Response() {
                        StatusCode = (ushort)System.Net.HttpStatusCode.UnsupportedMediaType,
                        StatusDescription = "Unfortunately, we don't support " + mime + " file type.  Try again with a PNG, GIF, or JPG."
                    };
                }

                byte[] _ByteArray = Convert.FromBase64String(data);

                // Create Full filename path
                string extention = ImageUpload.GetDefaultExtension(mime);
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

                Response response = new Response();
                response.StatusCode = (ushort)System.Net.HttpStatusCode.Created;
                response["Location"] = "/media/" + fileName;
                response["x-file"] = System.Text.Encoding.Default.GetString(xfile.ToJsonUtf8());
                return response;
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
        public static void ParseDataUri(string input, out string mime, out string encoding, out string data) {

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
        public static string GetDefaultExtension(string mimeType) {

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
