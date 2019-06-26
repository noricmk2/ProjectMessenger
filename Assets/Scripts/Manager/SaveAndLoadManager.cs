using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveAndLoadManager
{
    static readonly string SAVE_FILE_PATH = Application.persistentDataPath + "/Save";
    static readonly string SAVE_FILE_NAME_USER = "/userdata";
    static readonly string SAVE_FILE_NAME = "/save_";

    public static void Save(object saveData, int saveIdx = 0)
    {
        var directory = new DirectoryInfo(SAVE_FILE_PATH);
        if (directory.Exists == false)
            directory.Create();
        var formatter = new BinaryFormatter();

        var fileName = "";
        if (saveData is UserInfo._UserData)
            fileName = SAVE_FILE_PATH + SAVE_FILE_NAME_USER;
        else
            fileName = SAVE_FILE_PATH + SAVE_FILE_NAME + saveIdx.ToString();

        using (var file = File.Create(fileName))
        {
            formatter.Serialize(file, saveData);
        }

        PlayerPrefs.SetInt(MSUtil.ConstValue.LAST_CHAPTER_SAVE_KEY, saveIdx);
    }

    public static T Load<T>(int loadIdx = 0) where T : class, new()
    {
        T result = null;
        var path = "";

        if (typeof(T) == typeof(UserInfo._UserData))
            path = SAVE_FILE_PATH + SAVE_FILE_NAME_USER;
        else
            path = SAVE_FILE_PATH + SAVE_FILE_NAME + loadIdx.ToString();

        var formatter = new BinaryFormatter();
        var directoryInfo = new DirectoryInfo(SAVE_FILE_PATH);
        if (directoryInfo.Exists == false)
            directoryInfo.Create();

        var fileInfo = new FileInfo(path);
        if(fileInfo.Exists)
        {
            using (var file = File.Open(path, FileMode.Open))
            {
                if (file != null && file.Length > 0)
                    result = formatter.Deserialize(file) as T;
            }
        }
        return result;
    }
}
