using System;
using System.Net;
using System.Threading;
using System.IO.Compression; // Add this namespace for GZipStream


class Program
{
    static void Main(string[] args)
    {
        string directoryPath = @"D:\Tickets\1623586\ServerFiles"; // Replace with your directory path

        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add("http://localhost:8080/"); // Replace with desired URL and port
            listener.Start();

            Console.WriteLine("Local file server started. Listening on http://localhost:8080/");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    HandleRequest(context, directoryPath);
                }, context);
            }
        }
    }

    private static void HandleRequest(HttpListenerContext context, string directoryPath)
    {
        HttpListenerResponse response = context.Response;

        string route = context.Request.Url.AbsolutePath;

        string filePath = string.Empty;
        if (route == "/json")
        {
            filePath = "file.json";
        }
        else if (route == "/pdf")
        {
            filePath = "file.pdf";
        }
        else if (route == "/txt")
        {
            filePath = "file.txt";
        }
        else if (route == "/noext")
        {
            filePath = "file";
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            response.Close();
            return;
        }

        string fullPath = Path.Combine(directoryPath, filePath);
        if (File.Exists(fullPath))
        {
            byte[] buffer = File.ReadAllBytes(fullPath);

            response.ContentType = GetMimeType(Path.GetExtension(fullPath));

            string acceptEncoding = context.Request.Headers["Accept-Encoding"];
            if (acceptEncoding != null && (acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate")))
            {
                if (acceptEncoding.Contains("gzip"))
                {
                    response.Headers.Add("Content-Encoding", "gzip");
                    using (MemoryStream compressedStream = new MemoryStream())
                    {
                        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                        {
                            gzipStream.Write(buffer, 0, buffer.Length);
                        }
                        buffer = compressedStream.ToArray();
                    }
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    response.Headers.Add("Content-Encoding", "deflate");
                    using (MemoryStream compressedStream = new MemoryStream())
                    {
                        using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress))
                        {
                            deflateStream.Write(buffer, 0, buffer.Length);
                        }
                        buffer = compressedStream.ToArray();
                    }
                }
            }

            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        response.Close();
    }


    private static string GetMimeType(string extension)
    {
        switch (extension.ToLower())
        {
            case ".html":
                return "text/html";
            case ".css":
                return "text/css";
            case ".js":
                return "application/javascript";
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".gif":
                return "image/gif";
            case ".json":
                return "application/json"; // JSON MIME type
            case ".pdf":
                return "application/pdf"; // PDF MIME type
            default:
                return "application/octet-stream";
        }
    }
}
