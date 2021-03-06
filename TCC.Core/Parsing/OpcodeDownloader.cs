﻿using System.IO;
using Newtonsoft.Json.Linq;

namespace TCC.Parsing
{
    public static class OpcodeDownloader
    {
        public static void DownloadOpcodesIfNotExist(uint version, string directory)
        {
            DownloadOpcode(version, directory);
        }
        public static void DownloadSysmsgIfNotExist(uint version, string directory, int revision = 0)
        {
            DownloadSysmsg(version, directory, revision);
        }
        private static bool IsFileValid(string filename)
        {
            if (!File.Exists(filename)) return false;
            if (!Settings.SettingsHolder.CheckOpcodesHash) return true;
            var localHash = Utils.GenerateFileHash(filename);
            if (localHash == "")
            {
                WindowManager.FloatingButton.NotifyExtended("TCC", "Failed to check opcode file hash.\n Skipping download...", Data.NotificationType.Warning);
                return true;
            }
            using (var c = Utils.GetDefaultWebClient())
            {
                try
                {
                    var st = c.OpenRead("https://raw.githubusercontent.com/caali-hackerman/tera-data/master/mappings.json");
                    if (st != null)
                    {
                        var sr = new StreamReader(st);
                        var sMappings = sr.ReadToEnd();
                        var jMappings = JObject.Parse(sMappings);
                        var reg = SessionManager.Server.Region;
                        var jReg = jMappings[reg];
                        var remoteHash = jReg["protocol_hash"].Value<string>();
                        if (localHash == remoteHash) return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
        private static void DownloadOpcode(uint version, string directory)
        {
            Directory.CreateDirectory(directory);

            var filename = directory + Path.DirectorySeparatorChar + version + ".txt";
            if (IsFileValid(filename)) return;
            filename = directory + Path.DirectorySeparatorChar + "protocol." + version + ".map";
            if (IsFileValid(filename)) return;
            try
            {
                Download("https://raw.githubusercontent.com/tera-toolbox/tera-data/master/map_base/protocol." + version + ".map", filename);
                return;
            }
            catch { /* ignored*/ }
            try
            {
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/protocol." + version + ".map", filename);
            }
            catch { /* ignored*/ }
        }

        public static bool DownloadSysmsg(uint version, string directory, int revision = 0)
        {
            Directory.CreateDirectory(directory);

            var filename = directory + Path.DirectorySeparatorChar + "sysmsg." + revision / 100 + ".map";
            if (File.Exists(filename)) return true;
            else
            {
                try
                {
                    Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg." + revision / 100 + ".map", filename);
                    Log.CW($"[DownloadSysmsg] file downloaded from TeraDpsMeterData (rev/100) repo");
                    return true;
                }
                catch { /* ignored*/ }
            }
            filename = directory + Path.DirectorySeparatorChar + "sysmsg." + version + ".map";
            if (File.Exists(filename)) return true;
            else
            {
                try
                {
                    Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg." + version + ".map", filename);
                    return true;
                }
                catch { /* ignored*/ }
            }
            return false;
        }

        private static void Download(string remote, string local)
        {
            using (var client = Utils.GetDefaultWebClient())
            {
                client.DownloadFile(remote, local);
            }
        }
    }
}
