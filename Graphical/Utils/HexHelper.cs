using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Graphical.Utils
{
    internal class HexHelper
    {
        public static List<byte> ReadHex(String path)
        {
            List<byte> fullStream = new List<byte>();
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                long nBytesToRead = stream.Length;
                byte[] Buffer = new byte[nBytesToRead];
                stream.Read(Buffer, 0, Buffer.Length);
                stream.Close();
                fullStream = Buffer.ToList();
            }
            catch (Exception) { }
            return fullStream;
        }

        public static void WriteHex(String path, List<byte> hex)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate));
            writer.Write(hex.ToArray(), 0, hex.Count);
            writer.Close();
        }
    }
}
