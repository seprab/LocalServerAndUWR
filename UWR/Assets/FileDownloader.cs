using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Diagnostics;
using System.IO;

public class FileDownloader : MonoBehaviour
{
    private string serverUrl = "http://localhost:8080/";
    private string[] fileNames = { "json", "pdf", "txt", "noext" }; // The route names for the files

    private void gzip()
    {
        DownloadFiles("gzip");
    }
    private void deflate()
    {
        DownloadFiles("deflate");
    }
    private void empty()
    {
        DownloadFiles("");
    }

    private void DownloadFiles(string encoding)
    {
        foreach (string fileName in fileNames)
        {
            StartCoroutine(DownloadFile(fileName, encoding));
        }
    }
    private IEnumerator DownloadFile(string fileName, string encoding)
    {
        string url = serverUrl + fileName;
        string savePath = Path.Combine(Application.persistentDataPath, fileName);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            if(encoding != "default") request.SetRequestHeader("Accept-Encoding", encoding);
            yield return request.SendWebRequest();

            stopwatch.Stop();
            long downloadTimeMs = stopwatch.ElapsedMilliseconds;

            if (request.result != UnityWebRequest.Result.Success)
            {
                UnityEngine.Debug.LogError("Error downloading " + fileName + ": " + request.error);
            }
            else
            {
                File.WriteAllBytes(savePath, request.downloadHandler.data);
                string contEncoding = request.GetResponseHeader("Content-Encoding");
                UnityEngine.Debug.Log("Downloaded: " + fileName + " to " + savePath + " and content encoding was " + contEncoding);
                UnityEngine.Debug.Log("Download time for " + fileName + ": " + downloadTimeMs + " ms");
            }
        }
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 250, 30), "Download with gzip")) gzip();
        if (GUI.Button(new Rect(10, 50, 250, 30), "Download with deflate")) deflate();
        if (GUI.Button(new Rect(10, 90, 250, 30), "Download with no encoding")) empty();
        if (GUI.Button(new Rect(10, 130, 250, 30), "Download with default encoding")) empty();
    }
}
