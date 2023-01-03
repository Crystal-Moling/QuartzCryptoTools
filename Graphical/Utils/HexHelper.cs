using System;
using System.Collections.Generic;
using System.IO;

namespace Graphical.Utils
{
    internal class HexHelper
    {
        public static List<String> ReadHex(String path)
        {
            List<String> fullStream = new List<String>();
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                long nBytesToRead = stream.Length;
                byte[] Buffer = new byte[nBytesToRead];
                int m = stream.Read(Buffer, 0, Buffer.Length);
                stream.Close();
                for (int i = 0; i < Buffer.Length; i++)
                { fullStream.Add(Buffer[i].ToString("X2")); }
            }
            catch (Exception) { }
            return fullStream;
        }

        public static void WriteHex(String path, List<String> hex)
        {
            byte[] Buffer = new byte[hex.Count];
            for (int i = 0; i < hex.Count; i++)
            { Buffer[i] = Convert.ToByte(hex[i], 16); }
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Buffer, 0, Buffer.Length);
            writer.Close();
            stream.Close();
        }
    }
}
