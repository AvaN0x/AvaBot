using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

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
                    Utils.LogAsync("Data file created");
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

        public static GuildSettings GetSettings(ulong guildId)
        {
            GuildSettings value;
            if (guildSettings.TryGetValue(guildId, out value))
                return value; 
            else
            {
                guildSettings.Add(guildId, new GuildSettings());
                Utils.LogAsync("Add settings for " + guildId);
                return guildSettings[guildId];
            }
        }

        public static Task LogAsync(LogMessage log)
        {
            LogAsync(log.Source + " : " + log.Message + (log.Exception != null ? "\n\t" + log.Exception : ""), log.Severity.ToString());
            return Task.CompletedTask;
        }

        public static Task LogAsync(String message, string severity = "Info")
        {
            //TODO log to file
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss:fff") + " " + severity + "] " + message);
            return Task.CompletedTask;
        }

    }

    [Serializable]
    public class GuildSettings
    {
        public ulong? adminRoleId { get; set; }

        public Dictionary<ulong, DateTime> muted { get; set; }

        public bool chehScan { get; set; }
        public bool gf1Scan { get; set; }
        public bool ineScan { get; set; }
        public bool reactToUserScan { get; set; }

        public bool admin_mute { get; set; }

        public GuildSettings()
        {
            this.adminRoleId = null;

            this.muted = new Dictionary<ulong, DateTime>();

            this.chehScan = true;
            this.gf1Scan = true;
            this.ineScan = true;
            this.reactToUserScan = false;

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



        //[OnDeserializing]
        //private void SetMissingDefault(StreamingContext sc)
        //{
        //    reactToUserScan = false;
        //}
    }
}
