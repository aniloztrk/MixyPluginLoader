using System;
using System.Net;
using MixyPluginLoader.Models;
using Newtonsoft.Json;

namespace MixyPluginLoader.Utils
{
    public class WebHelper
    {
        private static readonly string IpAddress = "193.35.154.116";

        private static readonly string Port = "3090";
        
        public static bool TryGetPlugin(string name, out byte[] assembly)
        {
            try
            {
                assembly = AesEncryptor.DecryptBytes(new WebClient().DownloadData($"http://{IpAddress}:{Port}/{name}"));
                return true;
            }
            catch (Exception)
            {
                assembly = null;
                return false;
            }
        }

        public static bool TryGetWhiteList(string pluginName, out string whiteList)
        {
            try
            {
                whiteList = AesEncryptor.DecryptString(new WebClient().DownloadString($"http://{IpAddress}:{Port}/{pluginName}/Ip")).Replace("<br />", " ");
                return true;
            }
            catch (Exception)
            {
                whiteList = null;
                return false;
            }
        }
        
        public static bool TryGetWebhook(out Webhook webhook)
        {
            try
            {
                var webhookJson =
                    AesEncryptor.DecryptString(new WebClient().DownloadString($"http://{IpAddress}:{Port}/webhook"));
                webhook = JsonConvert.DeserializeObject<Webhook>(webhookJson);
                return true;
            }
            catch (Exception)
            {
                webhook = null;
                return false;
            }
        }

        public static bool CheckVersion()
        {
            try
            { 
                var version = new WebClient().DownloadString($"http://{IpAddress}:{Port}/version");

                return LoaderMain.LoaderVersion == version;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}