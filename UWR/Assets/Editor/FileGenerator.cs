using UnityEditor;
using UnityEngine;
using System.IO;

public class FileGenerator : EditorWindow
{
    private string outputPath = "../ServerFiles"; // Default output folder
    private long fileSizeInBytes = 10 * 1024 * 1024; // 10 MB

    [MenuItem("Tools/Generate Files")]
    static void Init()
    {
        FileGenerator window = (FileGenerator)EditorWindow.GetWindow(typeof(FileGenerator));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("File Generator", EditorStyles.boldLabel);

        outputPath = EditorGUILayout.TextField("Output Folder", outputPath);
        fileSizeInBytes = EditorGUILayout.LongField("File Size (Bytes)", fileSizeInBytes);

        if (GUILayout.Button("Generate Files"))
        {
            GenerateFiles();
        }
    }

    private void GenerateFiles()
    {
        string[] fileExtensions = { ".pdf", ".json", ".txt", "" };

        foreach (string extension in fileExtensions)
        {
            string fileName = "file" + extension;
            string filePath = Path.Combine(outputPath, fileName);

            GenerateFile(filePath, fileSizeInBytes);

            Debug.Log($"Generated {fileName} ({extension}) with size {fileSizeInBytes} bytes.");
        }
    }

    private void GenerateFile(string filePath, long sizeInBytes)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            fs.SetLength(sizeInBytes);
        }

        AssetDatabase.Refresh();
    }
}
