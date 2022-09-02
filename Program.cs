using QuartzCryptoTools.Modules;
using System;

namespace QuartzCryptoTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void PrintHead()
        {
            Console.Clear();
            Console.WriteLine(" ██████╗  ██████╗████████╗");
            Console.WriteLine("██╔═══██╗██╔════╝╚══██╔══╝");
            Console.WriteLine("██║   ██║██║        ██║   ");
            Console.WriteLine("██║▄▄ ██║██║        ██║   ");
            Console.WriteLine("╚██████╔╝╚██████╗   ██║      Quartz Crypto Tools");
            Console.WriteLine(" ╚══█═╝   ╚═════╝   ╚═╝         @Crystal-Moling");
            Console.WriteLine();
        }

        public static void MainMenu()
        {
            PrintHead();
            Console.WriteLine("================================================");
            Console.WriteLine("Please select action:");
            Console.WriteLine("(1) Hex Viewer");
            Console.WriteLine("(2) Zip fake encrypt");
            Console.WriteLine("(3) LSB Steganography (Feature)");
            Console.Write(">");
            switch (Console.ReadLine())
            {
                case "1":
                    HexViewer.Overview();
                    break;
                case "2":
                    ZipFakeCrypto.Overview();
                    break;
                default:
                    MainMenu();
                    break;
            }
        }
    }
}
