using System.Collections.Generic;
using System.Reflection;
using Rocket.Core.Plugins;
using UnityEngine;

namespace MixyPluginLoader.Extensions
{
    public static class RocketPluginManagerExtensions
    {
        public static void RegisterPluginFromGameObject(this RocketPluginManager rocketPluginManager, GameObject gameObject)
        {
            var plugins = (List<GameObject>)rocketPluginManager.GetType().GetField("plugins", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(rocketPluginManager);
            
            if (!plugins.Contains(gameObject))
                plugins.Add(gameObject);
        }
        
        public static void UnRegisterPluginFromGameObject(this RocketPluginManager rocketPluginManager, GameObject gameObject)
        {
            var plugins = (List<GameObject>)rocketPluginManager.GetType().GetField("plugins", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(rocketPluginManager);
            
            if (plugins.Contains(gameObject))
                plugins.Remove(gameObject);
        }
    }
}