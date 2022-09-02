using QuartzCryptoTools.Utils;
using System;
using System.IO;

namespace QuartzCryptoTools.Modules
{
    internal class HexViewer
    {
        private static String FileLocation = null;
        public static void Overview()
        {
            Program.PrintHead();
            Console.WriteLine("                    Hex Viewer");
            if (FileLocation == null)
            {
                Console.WriteLine("================================================");
                Console.WriteLine("Please type location of file");
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
                if (File.Exists(FileLocation))
                {
                    Program.PrintHead();
                    Console.WriteLine("                    Hex Viewer");
                    Console.WriteLine("================================================");
                    String[] totalStream = HexStream.GetStream(FileLocation);
                    Console.WriteLine("[ Total Stream: ]");
                    Console.WriteLine(totalStream[0]);
                    Console.WriteLine("[ Total: " + totalStream[1] + " Bytes ]");
                    Console.WriteLine("Press any key to return");
                    Console.ReadLine();
                    Program.MainMenu();
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
    }
}
