using System.Collections.Generic;
using MixyPluginLoader.Models;

namespace MixyPluginLoader.Config
{
    public class ConfigRoot
    {
        public List<Plugin> Plugins { get; set; }
        
        public ConfigRoot()
        {
            Plugins = new List<Plugin>
            {
                new Plugin("mixy-plugin")
            };
        }
    }
}