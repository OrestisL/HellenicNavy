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
        public int maxLookupTableDays = 10000;
        #region bounds
        private int minRate = 20;
        private int maxRate = 30;
        private int minLookupTableSizeHours = 30000;
        private int maxLookupTableSizeHours = 100000;
        private int minLookupTableSizeDays = 5000;
        private int maxLookupTableSizeDays = 20000;
        #endregion

        public Settings() { rate = 30; maxLookupTableHours = 50000; maxLookupTableDays = 10000; }
        public Settings(int rate, int maxLookupTableHours, int maxLookupTableDays)
        {
            this.rate = Mathf.Clamp(rate, minRate, maxRate);
            this.maxLookupTableHours = Mathf.Clamp(maxLookupTableHours, minLookupTableSizeHours, maxLookupTableSizeHours);
            this.maxLookupTableDays = Mathf.Clamp(maxLookupTableDays, minLookupTableSizeDays, maxLookupTableSizeDays);
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
