using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Application settings.
/// </summary>
public class SettingsHolder : GenericSingleton<SettingsHolder>
{
    [Serializable]
    public class Settings
    {
        public int rate = 30;
        public int maxLookupTableHours = 50000;
        public int maxLookupTableDays = 10000;
        public float writeFrequency = 2.0f;
        //in order to avoid weird behavior and excess resource usage, some bounds are set
        #region bounds
        private int minRate = 20;
        private int maxRate = 30;
        private float minWriteFrequency = 0.5f;
        private float maxWriteFrequency = 5.0f;
        private int minLookupTableSizeHours = 30000;
        private int maxLookupTableSizeHours = 100000;
        private int minLookupTableSizeDays = 5000;
        private int maxLookupTableSizeDays = 20000;
        #endregion

        public Settings() { rate = 30; maxLookupTableHours = 50000; maxLookupTableDays = 10000; writeFrequency = 1.0f; }
        public Settings(int rate, int maxLookupTableHours, int maxLookupTableDays, float writeFrequency)
        {
            this.rate = Mathf.Clamp(rate, minRate, maxRate);
            this.maxLookupTableHours = Mathf.Clamp(maxLookupTableHours, minLookupTableSizeHours, maxLookupTableSizeHours);
            this.maxLookupTableDays = Mathf.Clamp(maxLookupTableDays, minLookupTableSizeDays, maxLookupTableSizeDays);
            this.writeFrequency= Mathf.Clamp(writeFrequency, minWriteFrequency, maxWriteFrequency);
        }
        public Settings(string json)
        {
            Settings s = JsonConvert.DeserializeObject<Settings>(json);
            rate = Mathf.Clamp(s.rate, minRate, maxRate);
            maxLookupTableDays = Mathf.Clamp(s.maxLookupTableDays, minLookupTableSizeDays, maxLookupTableSizeDays);
            maxLookupTableHours = Mathf.Clamp(s.maxLookupTableHours, minLookupTableSizeHours,maxLookupTableSizeHours);
            writeFrequency = Mathf.Clamp(s.writeFrequency, minWriteFrequency, maxWriteFrequency); 
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
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "SPSSettings");
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
            settings = new Settings(json);
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
