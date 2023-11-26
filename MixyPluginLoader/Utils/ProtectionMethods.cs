using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace MixyPluginLoader.Utils
{
    public class ProtectionMethods
    {
        public static Assembly AssemblyLoadRandom(byte[] assembly)
        {
            int rnd = new System.Random().Next(4);
            switch (rnd)
            {
                case 0:
                    return AppDomain.CurrentDomain.Load(assembly, null);
                case 1:
                    return Assembly.Load(assembly, null);
                case 2:
                    return Assembly.Load(assembly, null, null);
                case 3:
                    return Assembly.Load(assembly, null, System.Security.SecurityContextSource.CurrentAssembly);
                default:
                    return null;
            }
        }
        
        public static bool CheckPatches()
        {
            foreach (var method in PatchProcessor.GetAllPatchedMethods())
            {
                var ignoredMethods = new List<Type> { typeof(Assembly), typeof(AppDomain) };
                if (ignoredMethods.Contains(method.DeclaringType) || method.Name == "Load" || method.Name == "GetPatchInfo")
                {   
                    return true;
                }
            }
            return false;
        }

        public static bool CheckUnityVersion() => Application.unityVersion.StartsWith("2018") ||
                                                  Application.unityVersion.StartsWith("2017");

        public static bool CheckDnSpy() => Process.GetProcessesByName("dnSpy").Length > 0;
    }
}