using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace AvaBot
{
    [Serializable]
    public class Settings
    {
        private string path;
        private Dictionary<ulong, GuildSettings> guildSettings { get; set; }


        public Settings()
        {
            this.path = "settings.ser";
            guildSettings = new Dictionary<ulong, GuildSettings>();

            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    Console.WriteLine("Settings created");
                    SaveSettings();
                }
                else
                    LoadSettings();
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e);
            }
        }

        public void SaveSettings()
        {
            var formatter = new BinaryFormatter();
            var stream = File.Open(path, FileMode.Create);
            formatter.Serialize(stream, this);
            Console.WriteLine("Settings saved");
            stream.Close();
        }

        public void LoadSettings()
        {
            var formatter = new BinaryFormatter();
            var stream = File.Open(path, FileMode.Open);
            var settings = (Settings)formatter.Deserialize(stream);
            Console.WriteLine("Settings loaded");

            this.guildSettings = settings.guildSettings;
            stream.Close();
        }

        public GuildSettings Get(ulong key)
        {
            GuildSettings value;
            if (guildSettings.TryGetValue(key, out value))
                return value; 
            else
            {
                guildSettings.Add(key, new GuildSettings());
                Console.WriteLine("Update settings for " + key);
                return guildSettings[key];
            }
        }
    }

    [Serializable]
    public class GuildSettings
    {
        public bool modpackScan { get; set; }
        public bool chehScan { get; set; }
        public bool gf1Scan { get; set; }
        public bool ineScan { get; set; }

        public GuildSettings()
        {
            this.modpackScan = true;
            this.chehScan = true;
            this.gf1Scan = true;
            this.ineScan = true;

        }
    }
}
