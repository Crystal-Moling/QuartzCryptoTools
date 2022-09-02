using System;
using System.IO;
using System.Text;

namespace QuartzCryptoTools.Utils
{
    internal class HexStream
    {
        public static byte[] Read(String path)
        {
            byte[] returnBytes = null;
			try
			{
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                long nBytesToRead = stream.Length;
                byte[] Buffer = new byte[nBytesToRead];
                int m = stream.Read(Buffer, 0, Buffer.Length);
                stream.Close();
                returnBytes = Buffer;
            }
			catch (Exception) { }
            return returnBytes;
        }

        public static void Write(String path, String hexString)
        {
            byte[] Buffer = new byte[hexString.Length / 2];
            int byteIndex = 0;
            for (int i = 0; i < hexString.Length; i++)
            {
                if (byteIndex < hexString.Length)
                {
                    Buffer[i] = Convert.ToByte(hexString.Substring(byteIndex, 2), 16);
                    byteIndex += 2;
                }
            }
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Buffer, 0, Buffer.Length);
            writer.Close();
            stream.Close();
        }

        public static String[] GetStream(String path)
        {
            byte[] Buffer = HexStream.Read(path);
            StringBuilder fullStream = new StringBuilder();
            int streamLength = 0;
            for (int i = 0; i < Buffer.Length; i++)
            {
                fullStream.Append(Buffer[i].ToString("X2") + " ");
                streamLength++;
            }
            return new string[] { fullStream.ToString(), streamLength.ToString() };
        }

        public static String GetString(String path)
        {
            return GetStream(path)[0].Replace(" ", "");
        }
    }
}
