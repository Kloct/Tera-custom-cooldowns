﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Parsing;
using TCC.TeraCommon;
using TCC.TeraCommon.Game.Services;
using TCC.Utilities.Extensions;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Test
{
    public static class Tester
    {
        public static bool Enabled = false;

        public static void ShowDebugWindow()
        {
            new DebugWindow().Show();
        }
        public static void SendFakeUsageStat(string region = "EU", int server = 27, string account = "foglio", string version = "TCC v1.3.19")
        {
            try
            {
                using (var c = Utils.GetDefaultWebClient())
                {
                    c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    var data = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(new JObject
                    {
                        { "region", region },
                        { "server", server},
                        { "account", account },
                        { "tcc_version", version}
                    }.ToString()));

                    c.Encoding = Encoding.UTF8;
                    c.UploadStringAsync(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/usage_stat"), data);
                }
            }
            catch { }
        }
        public static void AddTccTestMessages(int count)
        {
            var i = 0;
            while (i < count)
            {
                ChatWindowManager.Instance.AddTccMessage($"Test {i++}");
            }
        }
        public static void ParsePacketFromHexString<PacketType>(string hex)
        {

            var msg = new Message(DateTime.Now, MessageDirection.ServerToClient, new ArraySegment<byte>(hex.ToByteArrayHex()));
            var fac = new MessageFactory();
            var del = MessageFactory.Contructor<Func<TeraMessageReader, PacketType>>();
            var reader = new TeraMessageReader(msg, null, fac, null);
            del.DynamicInvoke(reader);
        }
        public static void Login(Class c)
        {
            SessionManager.Logged = true;
            SessionManager.LoadingScreen = false;
            SessionManager.CurrentPlayer.Class = c;
            WindowManager.ClassWindow.VM.CurrentClass = SessionManager.CurrentPlayer.Class;
            WindowManager.CooldownWindow.VM.LoadSkills(c);
        }
        public static void ForceEncounter(bool val = true)
        {
            SessionManager.Combat = val;
            SessionManager.Encounter = val;
        }
        public static void StartDeadlyGambleCooldown(uint cd)
        {
            Utils.CurrentClassVM<WarriorLayoutVM>()?.DeadlyGamble.Cooldown.Start(cd);
        }
        public static void AddFakeGroupMember(int id, Class c, Laurel l)
        {
            WindowManager.GroupWindow.VM.AddOrUpdateMember(new User(WindowManager.GroupWindow.VM.GetDispatcher())
            {
                Alive = true,
                Awakened = true,
                CurrentHp = 0,
                MaxHp = 1000,
                EntityId = Convert.ToUInt64(id),
                ServerId = Convert.ToUInt32(id),
                PlayerId = Convert.ToUInt32(id),
                UserClass = c,
                Online = true,
                Laurel = l
            });

        }
        public static void SpawnNpcAndUpdateHP(ushort zoneId, uint templateId)
        {
            EntityManager.SpawnNPC(zoneId, templateId, 11, true, false, 36);
            var t = new System.Timers.Timer { Interval = 1000 };
            var hp = 1000;
            t.Elapsed += (_, __) =>
            {
                hp -= 10;
                if (hp <= 900) hp = 1000;
                EntityManager.UpdateNPC(11, hp, 1000, 0);
            };
            t.Start();

        }
        public static void AddFakeCuGuilds()
        {
            var r = new Random();
            for (var i = 0; i < 30; i++)
            {
                WindowManager.CivilUnrestWindow.VM.AddGuild(new CityWarGuildInfo(1, (uint)i, 0, 0, (float)r.Next(0, 100) / 100));
                WindowManager.CivilUnrestWindow.VM.SetGuildName((uint)i, "Guild " + i);
                WindowManager.CivilUnrestWindow.VM.AddDestroyedGuildTower((uint)r.Next(0, 29));
            }
        }
        public static void AddFakeLfgAndShowWindow()
        {
            WindowManager.LfgListWindow.ShowWindow();
            var l = new Listing
            {
                LeaderId = 10,
                Message = "SJG exp only",
                LeaderName = "Foglio"
            };
            l.Players.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = 10, IsLeader = true, Online = true });
            l.Applicants.Add(new User(WindowManager.LfgListWindow.Dispatcher) { PlayerId = 1, Name = "Applicant", Online = true, UserClass = Class.Priest });
            WindowManager.LfgListWindow.VM.Listings.Add(l);
        }
        public static void AddFakeGroupMembers(int count)
        {
            var r = App.Random;
            for (var i = 0; i <= count; i++)
            {
                AddFakeGroupMember(i, (Class)r.Next(0, 12), (Laurel)r.Next(0, 6));
            }
        }
        public static void UpdateFakeMember(ulong eid)
        {
            WindowManager.GroupWindow.VM.TryGetUser(eid, out User l);
            var ut = new User(WindowManager.GroupWindow.VM.GetDispatcher())
            {
                Name = l.Name,
                PlayerId = l.PlayerId,
                ServerId = l.ServerId,
                EntityId = l.EntityId,
                Online = false,
                Laurel = l.Laurel,
                HasAggro = l.HasAggro,
                Alive = l.Alive,
                UserClass = l.UserClass,
                Awakened = l.Awakened,
            };
            Task.Delay(2000).ContinueWith(t => WindowManager.GroupWindow.VM.AddOrUpdateMember(ut));
        }
        public static void ProfileThreadsUsage()
        {
            var _t = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            var dispatchers = new List<Dispatcher>
            {
                 App.BaseDispatcher                                ,
                 WindowManager.BossWindow.Dispatcher               ,
                 WindowManager.BuffWindow.Dispatcher               ,
                 WindowManager.CharacterWindow.Dispatcher          ,
                 WindowManager.GroupWindow.Dispatcher              ,
                 WindowManager.CooldownWindow.Dispatcher           ,
                 WindowManager.ClassWindow.Dispatcher              ,
            };
            var threadIdToName = new ConcurrentDictionary<int, string>();
            foreach (var disp in dispatchers)
            {
                disp.Invoke(() =>
                {
                    var myId = Utils.GetCurrentThreadId();
                    threadIdToName[myId] = disp.Thread.ManagedThreadId == 1 ? "Main" : disp.Thread.Name;
                });
            }
            threadIdToName[PacketAnalyzer.AnalysisThreadId] = PacketAnalyzer.AnalysisThread.Name;

            var stats = new Dictionary<int, ThreadInfo>();
            _t.Tick += (_, __) =>
            {
                var p = Process.GetCurrentProcess();
                foreach (ProcessThread th in p.Threads)
                {
                    if (threadIdToName.ContainsKey(th.Id))
                    {
                        if (!stats.ContainsKey(th.Id))
                        {
                            stats.Add(th.Id, new ThreadInfo
                            {
                                Name = threadIdToName[th.Id],
                                Id = th.Id,
                                TotalTime = th.TotalProcessorTime.TotalMilliseconds,
                                Priority = threadIdToName[th.Id] == "Anls"
                                    ? PacketAnalyzer.AnalysisThread.Priority
                                    : dispatchers.FirstOrDefault(d => d.Thread.Name == threadIdToName[th.Id]).Thread
                                        .Priority
                            });
                        }
                        else stats[th.Id].TotalTime = th.TotalProcessorTime.TotalMilliseconds;
                    }
                }
                foreach (var item in stats)
                {
                    Console.WriteLine($"{threadIdToName[item.Key]} ({(int)item.Value.Priority}):\t\t{item.Value.TotalTime:0}\t\t{item.Value.DiffTime / 1000:P}\t");
                }
                Console.WriteLine("----------------------------------");
            };
            _t.Start();

        }
        public static void TestWebhook()
        {
            var url = "";
            var msg = new JObject
            {
                {"content", $"**Markdown** `or not?`"},
                {"username", "TCC Update" },
                {"avatar_url", "http://i.imgur.com/8IltuVz.png" }
            };

            using (var client = Utils.GetDefaultWebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.UploadString(url, "POST", msg.ToString());
            }
        }

        public static async void FireWebhook(string username)
        {
            var canFire = true;
            var wh = "";
            var whHash = Utils.GenerateHash(wh);
            var req = new JObject
            {
                { "webhook" , whHash},
                {"user", username }

            };
            using (var c = Utils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                try
                {

                    var res = await c.UploadStringTaskAsync(new Uri("https://us-central1-tcc-global-events.cloudfunctions.net/fire_webhook"),
                                                Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));

                    var jRes = JObject.Parse(res);
                    canFire = jRes["canFire"].Value<bool>();
                }
                catch (WebException)
                {
                    Console.WriteLine($"{username} failed");
                }

                if (!canFire)
                {
                    Console.WriteLine($"- Webhook refused for {username}");
                    return;
                }
                Console.WriteLine($"+ Webhook fired for {username}");
                var msg = new JObject
                {
                    {"content", $"Test from {username}"},
                    {"username", "TCC" },
                    {"avatar_url", "http://i.imgur.com/8IltuVz.png" }
                };
                using (var client = Utils.GetDefaultWebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.UploadString(wh, "POST", msg.ToString());
                }

            }

        }

        public async static void RegisterWebhook(string username)
        {
            var wh = "";
            var whHash = Utils.GenerateHash(wh);
            var r = new Random(DateTime.Now.Millisecond);
            var req = new JObject
            {
                {"webhook", whHash},
                {"user", username},
                {"online", r.NextDouble() >= 0.9 }
            };
            using (var c = Utils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                try
                {
                    await c.UploadStringTaskAsync(
                        new Uri("http://localhost:5001/tcc-global-events/us-central1/register_webhook"),
                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));
                }
                catch
                {
                    Console.WriteLine($"Failed to register webhook for {username}");
                }
            }
        }

        public static void AddWhispers(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ChatChannel.ReceivedWhisper, "Test", $"Test {i}"));
            }
        }

        private static Timer _t;
        public static void AddMobs()
        {
            _t = new Timer { Interval = 1 };
            _t.Elapsed += T_Elapsed;
            _t.Start();
        }

        public static void AddNpcAndAbnormalities()
        {
            EntityManager.SpawnNPC(950, 4000, 1, true, false, 36);
            var t = new Timer(1);
            var id = 0U;
            var a = true;
            t.Elapsed += (_, __) =>
            {
                if (true) AbnormalityManager.BeginAbnormality(id++, 1, 0, 500000, 1);
                else AbnormalityManager.EndAbnormality(1, 100800);
                a = !a;
            };
            t.Start();
        }
        private static ulong _eid = 1;
        private static void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            EntityManager.SpawnNPC(9, 700, _eid++, true, false, 0);
            if (_eid != 10000) return;
            EntityManager.ClearNPC();
            _t.Stop();
        }
    }
}
