using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Discord;

namespace AvaBot
{
    [Serializable]
    public class Utils
    {
        private static string path;
        private static Dictionary<ulong, GuildSettings> guildSettings { get; set; }


        public static void Init()
        {
            path = "data.ser";
            guildSettings = new Dictionary<ulong, GuildSettings>();

            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    Console.WriteLine("Data file created");
                    SaveData();
                }
                else
                    LoadData();
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e);
            }
        }

        public static void SaveData()
        {
            var formatter = new BinaryFormatter();
            var stream = File.Open(path, FileMode.Create);
            formatter.Serialize(stream, guildSettings);
            Utils.LogAsync("Data saved");
            stream.Close();
        }

        public static void LoadData()
        {
            var formatter = new BinaryFormatter();
            var stream = File.Open(path, FileMode.Open);
            guildSettings = (Dictionary<ulong, GuildSettings>)formatter.Deserialize(stream);
            Utils.LogAsync("Data loaded");

            stream.Close();
        }

        public static GuildSettings GetSettings(ulong key)
        {
            GuildSettings value;
            if (guildSettings.TryGetValue(key, out value))
                return value; 
            else
            {
                guildSettings.Add(key, new GuildSettings());
                Utils.LogAsync("Add settings for " + key);
                return guildSettings[key];
            }
        }

        public static Task LogAsync(LogMessage log)
        {
            //Console.WriteLine(log.ToString());
            //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss:fff") + " " + log.Severity + "] " + log.Source + " : " + log.Message + (log.Exception != null ? "\n\t" + log.Exception : ""));
            LogAsync(log.Source + " : " + log.Message + (log.Exception != null ? "\n\t" + log.Exception : ""), log.Severity.ToString());
            return Task.CompletedTask;
        }

        public static Task LogAsync(String message, string severity = "Info")
        {
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss:fff") + " " + severity + "] " + message);
            return Task.CompletedTask;
        }

    }

    [Serializable]
    public class GuildSettings
    {
        public Dictionary<ulong, DateTime> muted { get; set; }

        public bool modpackScan { get; set; }
        public bool chehScan { get; set; }
        public bool gf1Scan { get; set; }
        public bool ineScan { get; set; }

        public bool admin_mute { get; set; }

        public GuildSettings()
        {
            this.muted = new Dictionary<ulong, DateTime>();

            this.modpackScan = true;
            this.chehScan = true;
            this.gf1Scan = true;
            this.ineScan = true;

            this.admin_mute = true;

        }

        public bool IsMuted(ulong id)
        {
            DateTime date;
            if (muted.TryGetValue(id, out date))
            {
                if (DateTime.Now > date)
                {
                    muted.Remove(id);
                    return false;
                }
                else
                    return true;
            }
            else
                return false;
        }

    }
}
