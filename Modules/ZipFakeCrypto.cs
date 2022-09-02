using QuartzCryptoTools.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace QuartzCryptoTools.Modules
{
    internal class ZipFakeCrypto
    {
        private static int fileDirCount = 0;
        private static int fileBlocksLength = 0;
        private static String FileFullName = null;
        private static String FileLocation = null;
        private static String totalString = null;
        private static StringBuilder fileDirStream = new StringBuilder();
        private static String[] totalStream = new string[2];
        private static List<int> fileDirIndex = new List<int>();
        public static void Overview()
        {
            Program.PrintHead();
            Console.WriteLine("                 zip fake crypto");
            if (FileLocation == null)
            {
                Console.WriteLine("================================================");
                Console.WriteLine("Please type location of .zip archive");
                Console.WriteLine("Type \"q\" to return");
                Console.Write(">");
                FileLocation = Console.ReadLine();
            }
            if (FileLocation == "q")
            {
                FileLocation = null;
                Program.MainMenu();
            }
            else
            {
                // Find if file exists
                if (File.Exists(FileLocation))
                {
                    totalString = HexStream.GetString(FileLocation);
                    // Search file head code
                    if (totalString.Substring(0, 8) == FileHead.Zip)
                    {
                        Program.PrintHead();
                        Console.WriteLine("                 zip fake crypto");
                        Console.WriteLine("================================================");
                        Console.WriteLine("Please select action");
                        Console.WriteLine("(i)nfo      -   Show info of selected file");
                        Console.WriteLine("(e)ncrypt   -   Fake encrypt selected file");
                        Console.WriteLine("(d)ecrypt   -   Decrypt selected fake-encrypted file");
                        Console.WriteLine("(q)uit      -   Return to main menu");
                        Console.Write(">");
                        String input = Console.ReadLine(); // C:\Users\CloverMoling\Desktop\text.zip

                        // Get file name and stream
                        String[] FileLocationArr = FileLocation.Split('\\');
                        FileFullName = FileLocationArr[FileLocationArr.Length - 1];
                        totalStream = HexStream.GetStream(FileLocation);

                        // Get file blocks stream
                        String fileDirString = Regex.Matches(totalString, @"504B0102\S*504B0506")[0].Value;
                        fileBlocksLength = 0;
                        for (int i = 0; i < fileDirString.Length; i += 2)
                        {
                            fileDirStream.Append(fileDirString.Substring(i, 2) + " ");
                            fileBlocksLength++;
                        }

                        // Get file blocks count
                        fileDirCount = 0;
                        int index = -8;
                        while ((index = totalString.IndexOf("504B0102", index + 8)) > -1)
                        {
                            fileDirIndex.Add(index);
                            fileDirCount++;
                        }

                        switch (input)
                        {
                            case "i":
                                Program.PrintHead();
                                GetInfo(FileLocation);
                                break;
                            case "e":
                                Program.PrintHead();
                                Encrypt();
                                break;
                            case "d":
                                Program.PrintHead();
                                Decrypt();
                                break;
                            case "q":
                                FileLocation = null;
                                Program.MainMenu();
                                break;
                            default:
                                Overview();
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("================================================");
                        Console.WriteLine("File type not supported");
                        Console.ReadLine();
                        FileLocation = null;
                        Overview();
                    }
                }
                else
                {
                    Console.WriteLine("================================================");
                    Console.WriteLine("Selected File not exists! Press any key to retype");
                    Console.ReadLine();
                    FileLocation = null;
                    Overview();
                }
            }
        }

        private static void GetInfo(String path)
        {
            Console.WriteLine("                 zip fake crypto");
            Console.WriteLine("================================================");
            Console.WriteLine("[ File Name: ]");
            Console.WriteLine(FileFullName);
            Console.WriteLine("[ Total Stream: ]");
            Console.WriteLine(totalStream[0]);
            Console.WriteLine("[ Total: " + totalStream[1] + " Bytes ]");
            Console.WriteLine("[ File Blocks: ]");
            Console.WriteLine(fileDirStream.ToString());
            Console.WriteLine("[ Total: " + fileBlocksLength + " Bytes ]");
            Console.WriteLine("[ File Block count: " + fileDirCount + " ]");
            Console.WriteLine("Press any key to return");
            Console.ReadLine();
            Overview();
        }

        private static void Encrypt()
        {
            String fakeCryptedLocation = FileLocation.Split('.')[0] + ".fakeCrypt.zip";

            Console.WriteLine("                 zip fake crypto");
            Console.WriteLine("================================================");
            StringBuilder fakeCryptedTotalString = new StringBuilder(totalString);
            StringBuilder fakeCryptedTotalStream = new StringBuilder();
            for (int i = 0; i < fileDirIndex.Count; i++)
            {
                fakeCryptedTotalString.Replace("0", "9", fileDirIndex[i] + 17, 1);
            }
            for (int i = 0; i < fakeCryptedTotalString.ToString().Length; i += 2)
            {
                fakeCryptedTotalStream.Append(fakeCryptedTotalString.ToString().Substring(i, 2) + " ");
            }
            Console.WriteLine("[ Fake crypted stream: ]");
            Console.WriteLine(fakeCryptedTotalStream.ToString());
            HexStream.Write(fakeCryptedLocation, fakeCryptedTotalString.ToString());

            Console.WriteLine("[ Fake crypted file path: ]");
            Console.WriteLine(fakeCryptedLocation);
            Console.Read();
        }

        private static void Decrypt()
        {
            String decryptedLocation = FileLocation.Split('.')[0] + ".decrypted.zip";

            Console.WriteLine("                 zip fake crypto");
            Console.WriteLine("================================================");
            StringBuilder fakeCryptedTotalString = new StringBuilder(totalString);
            StringBuilder fakeCryptedTotalStream = new StringBuilder();
            for (int i = 0; i < fileDirIndex.Count; i++)
            {
                fakeCryptedTotalString.Replace("9", "0", fileDirIndex[i] + 17, 1);
            }
            for (int i = 0; i < fakeCryptedTotalString.ToString().Length; i += 2)
            {
                fakeCryptedTotalStream.Append(fakeCryptedTotalString.ToString().Substring(i, 2) + " ");
            }
            Console.WriteLine("[ Decrypted stream: ]");
            Console.WriteLine(fakeCryptedTotalStream.ToString());
            HexStream.Write(decryptedLocation, fakeCryptedTotalString.ToString());

            Console.WriteLine("[ Decrypted file path: ]");
            Console.WriteLine(decryptedLocation);
            Console.Read();
        }
    }
}
