using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SettingsHolder : GenericSingleton<SettingsHolder>
{
    [Serializable]
    public class Settings
    {
        public int rate = 30;
        public int maxLookupTableHours = 50000;

        public Settings() { rate = 30; maxLookupTableHours = 50000; }
        public Settings(int rate, int maxLookupTableHours)
        {
            this.rate = rate;
            this.maxLookupTableHours = maxLookupTableHours;
        }
        public string ToJson() 
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public Settings settings;

    public override void Awake()
    {
        base.Awake();
        LoadFromJson("settings");
    }

    void LoadFromJson(string name)
    {
        //ensure path exists (path is next to the exe)
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "ApplicationSettings");
        if (!Directory.Exists(directoryPath))
        {
            Debug.Log("Settings directory does not exist, creating...");
            Directory.CreateDirectory(directoryPath);
        }

        //ensure json file is at path
        string jsonPath = Path.Combine(directoryPath, string.Format("{0}.txt", name));
        if (File.Exists(jsonPath))
        {
            Debug.Log("Reading settings from file...");
            string json = "";
            using (StreamReader reader = new StreamReader(jsonPath))
            {
                json = reader.ReadToEnd();
            }
            settings = JsonConvert.DeserializeObject<Settings>(json);
        }
        else
        {
            Debug.Log("Settings file does not exist, creating with default values...");
            settings = new Settings();
            File.Create(jsonPath).Close();
            using (StreamWriter writer = new StreamWriter(jsonPath)) 
            {
                writer.Write(JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
        }
    }

}
