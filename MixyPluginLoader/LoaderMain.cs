using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MixyPluginLoader.Config;
using MixyPluginLoader.Extensions;
using MixyPluginLoader.Utils;
using MixyWebhookBuilder;
using MixyWebhookBuilder.Models;
using MixyYamlHelper;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using SDG.Unturned;
using UnityEngine;

namespace MixyPluginLoader
{
    public class LoaderMain : RocketPlugin
    {
        internal const string LoaderVersion = "1.0.0.3";
        
        public static LoaderMain Instance { get; private set; }
        
        public ConfigRoot Config { get; private set; }

        private Dictionary<string, GameObject> LoadedPlugins { get; } = new Dictionary<string, GameObject>();

        protected override async void Load()
        {
            Instance = this;

            var configPath = Rocket.Core.Environment.PluginsDirectory + "/MixyPluginLoader/Licenses.yaml";

            if (!File.Exists(configPath))
            {
                new FileStream(configPath, FileMode.Create).Close();
                Config = new ConfigRoot();
                Json.TryWrite(configPath, Config);
                YamlHelper.TryWriteFile(Config, configPath);
            }

            if (YamlHelper.TryReadFile<ConfigRoot>(out var root, configPath))
                Config = root;
            
            else
            {
                Config = new ConfigRoot();
                ConsoleHelper.WriteLinePrefix("Lisans dosyasi sorunlu !!!", ConsoleColor.Red);
            }

            var pl = Config.Plugins.FirstOrDefault(p => p.License == "mixy-plugin");
            if (pl != null)
            {
                Config.Plugins.Remove(pl);
            }

            ThreadPool.QueueUserWorkItem(async (_) =>
            {
                var path = Rocket.Core.Environment.PluginsDirectory +
                           "/MixyPluginLoader/MixyPluginLoader.en.translation.xml";
                if (File.Exists(path))
                {
                    await Task.Delay(1000);
                    File.Delete(path);
                }
            });

            if (!WebHelper.CheckVersion())
            {
                ConsoleHelper.WriteLinePrefix("Loader versiyonu dogrulanamadi !", ConsoleColor.Red);
                return;
            }

            if (!WebHelper.TryGetWebhook(out var webhook))
            {
                ConsoleHelper.WriteLinePrefix("Sunucu yanit vermiyor !", ConsoleColor.Red);
                return; 
            }

            var webhookBuilder = new WebhookBuilder(webhook.Url);
            
            if (ProtectionMethods.CheckPatches())
            {
                var embed = new Embed($"**{Provider.serverName} {ServerIP()}**", "Harmony Patch Algılandı.", 15548997);
                var webhookContent = new WebhookContent(webhook.Username, webhook.AvatarUrl);
                webhookContent.AddEmbed(embed);
                webhookBuilder.SetContent(webhookContent);
                await webhookBuilder.SendContentTaskAsync();
                UnloadPlugin();
                return;
            }

            if (ProtectionMethods.CheckUnityVersion())
            {
                var embed = new Embed($"**{Provider.serverName} {ServerIP()}**", "Unity Versiyonu Doğrulanamadı.", 15548997);
                var webhookContent = new WebhookContent(webhook.Username, webhook.AvatarUrl);
                webhookContent.AddEmbed(embed);
                webhookBuilder.SetContent(webhookContent);
                await webhookBuilder.SendContentTaskAsync();
                UnloadPlugin();
                return;
            }

            if (ProtectionMethods.CheckDnSpy())
            {
                var embed = new Embed($"**{Provider.serverName} {ServerIP()}**", "DnSpy Algılandı.", 15548997);
                var webhookContent = new WebhookContent(webhook.Username, webhook.AvatarUrl);
                webhookContent.AddEmbed(embed);
                webhookBuilder.SetContent(webhookContent);
                await webhookBuilder.SendContentTaskAsync();
                UnloadPlugin();
                return;
            }
            
            ConsoleHelper.WriteLine(@"|----------------------------------------------------------------------------|", ConsoleColor.DarkCyan);
            ConsoleHelper.WriteLine(@"|  __  __   _                   ____    _                   _                |", ConsoleColor.Cyan);
            ConsoleHelper.WriteLine(@"| |  \/  | (_) __  __  _   _   |  _ \  | |  _   _    __ _  (_)  _ __    ___  |", ConsoleColor.Cyan);
            ConsoleHelper.WriteLine(@"| | |\/| | | | \ \/ / | | | |  | |_) | | | | | | |  / _` | | | | '_ \  / __| |", ConsoleColor.Cyan);
            ConsoleHelper.WriteLine(@"| | |  | | | |  >  <  | |_| |  |  __/  | | | |_| | | (_| | | | | | | | \__ \ |", ConsoleColor.Cyan);
            ConsoleHelper.WriteLine(@"| |_|  |_| |_| /_/\_\  \__, |  |_|     |_|  \__,_|  \__, | |_| |_| |_| |___/ |", ConsoleColor.Cyan);
            ConsoleHelper.WriteLine(@"|                      |___/                        |___/                    |", ConsoleColor.Cyan);
            ConsoleHelper.Write(@"|-------------------------------| ", ConsoleColor.DarkCyan);
            ConsoleHelper.Write("`Mixy#1232", ConsoleColor.Magenta);
            ConsoleHelper.WriteLine(@" |-------------------------------|", ConsoleColor.DarkCyan);

            if (Config.Plugins.Count == 0)
            {
                ConsoleHelper.WriteLinePrefix("Yuklenecek hicbir eklenti bulunamadi.", ConsoleColor.Red);
                return;
            }

            ConsoleHelper.WritePrefix($"{Config.Plugins.Count} ", ConsoleColor.Green);
            ConsoleHelper.WriteLine(" eklenti yukleniyor...", ConsoleColor.Yellow);

            var loadEmbed = new Embed($"**{Provider.serverName} {ServerIP()}**", $"{Config.Plugins.Count} adet plugin yükleniyor.", 1146986);
            loadEmbed.SetThumbnail(Provider.configData.Browser.Icon);

            foreach (var plugin in Config.Plugins)
            {
                if (!WebHelper.TryGetWhiteList(plugin.License, out string wl))
                {
                    ConsoleHelper.WriteLinePrefix($"{plugin.License} bulunamadi.", ConsoleColor.Red);
                    continue;
                }

                if (!wl.Contains(ServerIP()))
                {
                    ConsoleHelper.WriteLinePrefix($"{plugin.License} icin ip adresi kayitli degil.", ConsoleColor.Red);

                    loadEmbed.AddField($"**{plugin.License}**", "Ip-Onay ⛔️", true);
                    continue;       
                }

                if (!WebHelper.TryGetPlugin(plugin.License, out byte[] rawAssembly))
                {
                    ConsoleHelper.WriteLinePrefix($"{plugin.License} bulunamadi.", ConsoleColor.Red);
                    continue;           
                }
                
                try
                {
                    foreach (var rawPl in RocketHelper.GetTypesFromInterface(ProtectionMethods.AssemblyLoadRandom(rawAssembly), "IRocketPlugin"))
                    {
                        var gameObject = new GameObject(rawPl.Name, rawPl);
                        DontDestroyOnLoad(gameObject);
                        R.Plugins.RegisterPluginFromGameObject(gameObject);

                        if (!LoadedPlugins.ContainsKey(plugin.License))
                            LoadedPlugins.Add(plugin.License, gameObject);
                        else
                            LoadedPlugins[plugin.License] = gameObject; 
                        
                        ConsoleHelper.WritePrefix($"{rawPl.Assembly.GetName().Name}", ConsoleColor.Cyan);
                        ConsoleHelper.WriteLine(" yuklendi!", ConsoleColor.Yellow);
                        
                        loadEmbed.AddField($"**{plugin.License}**", "Ip-Onay ✅", true);
                    }
                }
                catch (Exception e)
                {
                    ConsoleHelper.WriteLinePrefix($"{plugin.License} yuklenemedi.", ConsoleColor.Red);
                    ConsoleHelper.WriteLinePrefix($"{e}", ConsoleColor.Red);
                }
            }

            var loadContent = new WebhookContent(webhook.Username, webhook.AvatarUrl);
            loadContent.AddEmbed(loadEmbed);
            webhookBuilder.SetContent(loadContent);
            await webhookBuilder.SendContentTaskAsync();
        }

        protected override void Unload()
        {
            Instance = null;

            foreach (var plugin in LoadedPlugins)
            {
                R.Plugins.UnRegisterPluginFromGameObject(plugin.Value);
                Destroy(plugin.Value);
            }
        }
        
        public string ServerIP()
        {
            string text = "";
            var webRequest = WebRequest.Create("http://checkip.dyndns.org/%22%22");
            using (var response = webRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    text = streamReader.ReadToEnd();
                }
            }
            int num = text.IndexOf("Address: ") + 9;
            int num2 = text.LastIndexOf("</body>");
            text = text.Substring(num, num2 - num);
            return text;
        }
    }
}