using System;
using System.IO;
using MixyPluginLoader.Config;
using Newtonsoft.Json;

namespace MixyPluginLoader.Utils
{
    public class Json
    {
        public static bool TryWrite(string path, object obj)
        {
            try
            {
                var jsonObj = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(path, jsonObj);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryRead(string path, out ConfigRoot obj)
        {
            try
            {
                obj = JsonConvert.DeserializeObject<ConfigRoot>(File.ReadAllText(path));
                return true;
            }
            catch (Exception)
            {
                obj = null;
                return false;
            }
        }
    }
}