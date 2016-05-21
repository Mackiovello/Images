using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Starcounter;
using Starcounter.Internal;

namespace Images {
    public static class UploadHandlers
    {
        public static string[] AllowedMimeTypes = { "image/gif", "image/jpeg", "image/png", "image/svg+xml" };
        public const string WebSocketGroupName = "SCFileUploadWSG";
        private static UploadTask _uploadingTask;
        private static readonly IllustrationHelper Helper = new IllustrationHelper();

        public static void GET(string urlGet, Action<UploadTask> uploadingAction)
        {
            var url = urlGet + "?{?}";
            RegiserSharedFolder();

            Handle.GET(url, (string parameters, Request request) => {
                string sessionId;
                string fileName;
                long fileSize;
                string error;

                if (!ResolveUploadParameters(parameters, out sessionId, out fileName, out fileSize, out error))
                {
                    return new Response
                    {
                        StatusCode = (ushort)System.Net.HttpStatusCode.BadRequest,
                        Body = error
                    };
                }

                if (!request.WebSocketUpgrade)
                {
                    return 404;
                }

                request.SendUpgrade(WebSocketGroupName);
                var task = new UploadTask(sessionId, fileName, fileSize, parameters);

                task.StateChange += (s, a) =>
                {
                    uploadingAction(s as UploadTask);
                };

                _uploadingTask = task;

                return HandlerStatus.Handled;
            }, new HandlerOptions { SkipRequestFilters = true });

            Handle.WebSocket(WebSocketGroupName, (data, ws) => {
                if (_uploadingTask == null)
                {
                    ws.Disconnect("Could not find correct socket to handle the incoming data.", WebSocket.WebSocketCloseCodes.WS_CLOSE_CANT_ACCEPT_DATA);
                    return;
                }

                var task = _uploadingTask;

                task.Write(data);

                if (task.FileSize > 0)
                {
                    var uploadedDirectory = Helper.GetUploadDirectory();
                    var filePath = task.FilePath.Substring(task.FilePath.IndexOf(uploadedDirectory, StringComparison.Ordinal) + uploadedDirectory.Length);
                    var progress = "{" +
                                        "\"progress\" : " + task.Progress + "," +
                                        "\"fileUrl\" : \"" + filePath.Replace("\\", "/") + "\"" +
                                   "}";

                    ws.Send(progress);
                }
            });

            Handle.WebSocketDisconnect(WebSocketGroupName, ws => {
                var task = _uploadingTask;
                task?.Close();
            });
        }

        private static bool ResolveUploadParameters(string parameters, out string sessionId, out string fileName, out long fileSize, out string error)
        {
            fileName = null;
            fileSize = -1;
            error = null;

            NameValueCollection values = HttpUtility.ParseQueryString(parameters);

            sessionId = values["sessionid"];

            if (string.IsNullOrEmpty(sessionId))
            {
                error = "Invalid or missing sessionid url parameter";
                return false;
            }

            fileName = values["filename"];

            if (string.IsNullOrEmpty(fileName))
            {
                error = "Invalid or missing filename url parameter";
                return false;
            }

            if (!long.TryParse(values["filesize"], out fileSize))
            {
                error = "Invalid or missing filesize url parameter";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add static folder for uploaded mediafiles so they can be accessable via the web
        /// </summary>
        public static void RegiserSharedFolder()
        {
            var folder = Helper.GetUploadDirectory();

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            AppsBootstrapper.AddStaticFileDirectory(folder);

        }

        public enum UploadTaskState
        {
            Connected,
            Uploading,
            Completed,
            Error
        }

        public class UploadTask
        {
            public event EventHandler StateChange;

            /// <summary>
            /// Size of uploaded file
            /// </summary>
            public long FileSize { get; protected set; }

            /// <summary>
            /// Name of uploaded file
            /// </summary>
            public string FileName { get; protected set; }

            /// <summary>
            /// Temporary path of uploaded file
            /// </summary>
            public string FilePath { get; protected set; }

            /// <summary>
            /// Starcounter session id
            /// </summary>
            public string SessionId { get; protected set; }

            /// <summary>
            /// Query string parameters
            /// </summary>
            public string QueryString { get; protected set; }

            /// <summary>
            /// Indicates current state of the task
            /// </summary>
            public UploadTaskState State { get; protected set; }

            protected FileStream FileStream;

            public UploadTask(string sessionId, string fileName, long fileSize, string queryString)
            {
                SessionId = sessionId;
                FileName = fileName;
                FileSize = fileSize;
                QueryString = queryString;

                State = UploadTaskState.Connected;

                var extention = FileName.Substring(FileName.LastIndexOf(".", StringComparison.Ordinal));
                var path = Path.GetRandomFileName() + extention;
                var filePath = Helper.GetUploadDirectory();

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                FilePath = Path.Combine(filePath, path);
                FileStream = new FileStream(FilePath, FileMode.Append);

                Handle.AddOutgoingHeader("x-file-location", "/" + Helper.GetUploadDirectory() + "/" + path);
            }

            public string TempFileName => FileStream?.Name;

            public int Progress
            {
                get
                {
                    if (FileSize < 1 || FileStream == null)
                    {
                        return 0;
                    }

                    switch (State)
                    {
                        case UploadTaskState.Completed:
                            return 100;
                        case UploadTaskState.Error:
                            return -1;
                        case UploadTaskState.Connected:
                        case UploadTaskState.Uploading:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var progress = (int) (100.0*FileStream.Position/FileSize);

                    return progress;
                }
            }

            public void Write(byte[] data)
            {
                State = UploadTaskState.Uploading;
                FileStream.Write(data, 0, data.Length);
                FileStream.Flush(true);
                OnUploading();
            }

            public void Close()
            {
                State = Progress >= 100 ? UploadTaskState.Completed : UploadTaskState.Error;
                FileStream?.Dispose();
                OnUploading();
            }

            protected void OnUploading()
            {
                StateChange?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
