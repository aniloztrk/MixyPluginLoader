using System;

namespace MixyPluginLoader.Utils
{
    public class ConsoleHelper
    {
        public static string Prefix = "Mixy`s Plugins";
        
        public static void WriteLine(string msg, ConsoleColor color)
        {
            Console.WriteLine(msg, Console.ForegroundColor = color);
            Console.ResetColor();
        }
        
        public static void Write(string msg, ConsoleColor color)
        {
            Console.Write(msg, Console.ForegroundColor = color);
            Console.ResetColor();
        }
        
        public static void WriteLinePrefix(string msg, ConsoleColor color)
        {
            Console.Write("[", Console.ForegroundColor = ConsoleColor.DarkGray);
            Console.Write(Prefix, Console.ForegroundColor = ConsoleColor.Cyan);
            Console.Write("]", Console.ForegroundColor = ConsoleColor.DarkGray);
            Console.Write(" >> ", Console.ForegroundColor = ConsoleColor.Gray);
            Console.WriteLine(msg, Console.ForegroundColor = color);
            Console.ResetColor();
        }
        
        public static void WritePrefix(string msg, ConsoleColor color)
        {
            Console.Write("[", Console.ForegroundColor = ConsoleColor.DarkGray);
            Console.Write(Prefix, Console.ForegroundColor = ConsoleColor.Cyan);
            Console.Write("]", Console.ForegroundColor = ConsoleColor.DarkGray);
            Console.Write(" >> ", Console.ForegroundColor = ConsoleColor.Gray);
            Console.Write(msg, Console.ForegroundColor = color);
            Console.ResetColor();
        }
    }
}