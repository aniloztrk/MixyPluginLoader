namespace MixyPluginLoader.Models
{
    public class Plugin
    {
        public string License { get; set; }

        public Plugin(string license)
        {
            License = license;
        }
            
        public Plugin() { }
    }
}