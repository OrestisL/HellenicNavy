using UnityEngine.UI;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using System;

public class dialogueTest : MonoBehaviour
{
    public Button selectTargetButton, selectFileButton, copyFileButton;
    public static string filePath;
    public static string targetPath;

    public event FileBrowser.OnSuccess onSuccessSelectFolder = delegate (string[] paths)
    {
        //[] has length 1 because multiselect is false
        dialogueTest.targetPath = paths[0];

        Debug.Log(targetPath);
    };

    public event FileBrowser.OnSuccess onSuccessSelectFile = delegate (string[] paths)
    {
        dialogueTest.filePath = paths[0];
        Debug.Log(filePath);
    };

    public event FileBrowser.OnCancel onCancel = delegate
    {
        Debug.Log("dialogue cancel");
    };
    private void Start()
    {
        selectTargetButton.onClick.AddListener(ShowDialogueSelectFolder);
        selectFileButton.onClick.AddListener(ShowDialogueSelectFile);
        copyFileButton.onClick.AddListener(CopyFile);
    }

    void ShowDialogueSelectFolder()
    {
        if (FileBrowser.ShowLoadDialog(onSuccessSelectFolder, onCancel, FileBrowser.PickMode.Folders))
        {
            Debug.Log("dialogue select folder open");
        }
    }

    void ShowDialogueSelectFile()
    {
        if (FileBrowser.ShowLoadDialog(onSuccessSelectFile, onCancel, FileBrowser.PickMode.Files))
        {
            Debug.Log("dialogue select file open");
        }
    }

    void CopyFile()
    {
        if (string.IsNullOrEmpty(filePath) | string.IsNullOrEmpty(targetPath))
        {
            Debug.Log("file or target paths are null or empty");
            return;
        }
        //copy expects full file path, including name
        string name = filePath.Substring(filePath.LastIndexOf('\\') + 1);
        System.IO.File.Copy(filePath, Path.Combine(filePath, name));
        //also handle file exists (replace)
    }

}
