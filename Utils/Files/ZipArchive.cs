using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QuartzCryptoTools.Utils.Files
{
    internal class ZipArchive
    {
        private String archiveStream = null;
        private String archiveString = null;

        private String archiveName = null;
        private String archiveSize = null;

        private StringBuilder archiveFileBlocksStream = new StringBuilder();
        private int archiveFileBlocksLength = 0;

        private List<int> archiveFileDirIndex = new List<int>();
        private int archiveFileDirCount = 0;

        public ZipArchive(String path)
        {
            // Get archive stream
            archiveStream = HexStream.GetStream(path)[0];
            archiveString = HexStream.GetString(path);
            // Get archive info
            archiveName = path.Split('\\')[path.Split('\\').Length - 1];
            archiveSize = HexStream.GetStream(path)[1];
            // Get file blocks stream
            String fileDirString = Regex.Matches(archiveString, @"504B0102\S*504B0506")[0].Value;
            for (int i = 0; i < fileDirString.Length; i += 2)
            {
                archiveFileBlocksStream.Append(fileDirString.Substring(i, 2) + " ");
                archiveFileBlocksLength++;
            }
            // Get file blocks count
            int index = -8;
            while ((index = archiveString.IndexOf("504B0102", index + 8)) > -1)
            {
                archiveFileDirIndex.Add(index);
                archiveFileDirCount++;
            }
        }

        public String Stream()
        {
            return archiveStream;
        }
        public String String()
        {
            return archiveString;
        }
        public String Name()
        {
            return archiveName;
        }
        public String Size()
        {
            return archiveSize;
        }
        public String FileBlocksStream()
        {
            return archiveFileBlocksStream.ToString();
        }
        public int FileBlocksLength()
        {
            return archiveFileBlocksLength;
        }
        public List<int> FileDirIndex()
        {
            return archiveFileDirIndex;
        }
        public int FileDirCount()
        {
            return archiveFileDirCount;
        }
    }
}
