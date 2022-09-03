using QuartzCryptoTools.Utils;
using QuartzCryptoTools.Utils.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace QuartzCryptoTools.Modules
{
    internal class ZipFakeCrypto
    {
        private static String FileLocation = null;
        private static ZipArchive zip;
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
                    zip = new ZipArchive(FileLocation);
                    // Search file head code
                    if (zip.String().Substring(0, 8) == FileHead.Zip)
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
                        
                        switch (input)
                        {
                            case "i":
                                Program.PrintHead();
                                GetInfo();
                                break;
                            case "e":
                                Program.PrintHead();
                                Crypt("0","9","Crypted");
                                break;
                            case "d":
                                Program.PrintHead();
                                Crypt("9", "0", "Decrypted");
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

        private static void GetInfo()
        {
            Console.WriteLine("                 zip fake crypto");
            Console.WriteLine("================================================");
            Console.WriteLine("[ File Name: ]");
            Console.WriteLine(zip.Name());
            Console.WriteLine("[ Total Stream: ]");
            Console.WriteLine(zip.Stream());
            Console.WriteLine("[ Total: " + zip.Size() + " Bytes ]");
            Console.WriteLine("[ File Blocks: ]");
            Console.WriteLine(zip.FileBlocksStream());
            Console.WriteLine("[ Total: " + zip.FileBlocksLength() + " Bytes ]");
            Console.WriteLine("[ File Block count: " + zip.FileDirCount() + " ]");
            Console.WriteLine("Press any key to return");
            Console.ReadLine();
            Overview();
        }

        private static void Crypt(String oldStr, String newStr, String method)
        {
            String fakeCryptedLocation = FileLocation.Split('.')[0] + "." + method + ".zip";

            Console.WriteLine("                 zip fake crypto");
            Console.WriteLine("================================================");
            StringBuilder CryptTotalString = new StringBuilder(zip.String());
            StringBuilder CryptTotalStream = new StringBuilder();

            // Replace crypt sign
            for (int i = 0; i < zip.FileDirIndex().Count; i++)
            {
                CryptTotalString.Replace(oldStr, newStr, zip.FileDirIndex()[i] + 17, 1);
            }

            // Build result Stream
            for (int i = 0; i < CryptTotalString.ToString().Length; i += 2)
            {
                CryptTotalStream.Append(CryptTotalString.ToString().Substring(i, 2) + " ");
            }

            // Print File Stream
            Console.WriteLine("[ " + method + " stream: ]");
            Console.WriteLine(CryptTotalStream.ToString());

            // Write file with Hex
            HexStream.Write(fakeCryptedLocation, CryptTotalString.ToString());

            // Print output path
            Console.WriteLine("[ " + method + " file path: ]");
            Console.WriteLine(fakeCryptedLocation);

            Console.WriteLine("Press any key to return");
            Console.ReadLine();
            Program.MainMenu();
        }
    }
}
