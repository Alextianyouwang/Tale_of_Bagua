using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string _dataDirPath, string _dataFileName) 
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
    }

    public GameData Load() 
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath)) 
        {
            try
            {
                loadedData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(fullPath));
                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data due to {e.Message} {e.StackTrace}");
                throw e;
            }
        }

        return loadedData;
    }

    public void Save(GameData data) 
    { 
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath)) 
            File.Delete(fullPath);
        try 
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
           
            string jsonData = JsonConvert.SerializeObject(data,Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
         
            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) 
            {
                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(jsonData);
                }
            }
        }
        catch (Exception e )
        {
            Debug.LogError($"Failed to save data due to {e.Message} {e.StackTrace}");
             throw e;
        }
    }

    public void Delete() 
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
