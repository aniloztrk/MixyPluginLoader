using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MixyPluginLoader.Utils
{
    public class AesEncryptor
    {
        public static byte[] DecryptBytes(byte[] assembly)
        {
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes("}p8_[b57!6&@c7b^(4)5^%87L=90AtlZ0|{5M_S{z(QSSvtbaK", new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x31 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(assembly, 0, assembly.Length);
                        cs.Close();
                    }
                    assembly = ms.ToArray();
                }
            }
            return assembly;
        }
        
        public static string DecryptString(string text)
        {
            byte[] cipherBytes = Convert.FromBase64String(text);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes("}p8_[b57!6&@c7b^(4)5^%87L=90AtlZ0|{5M_S{z(QSSvtbaK", new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x31 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    text = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return text;
        }
    }
}