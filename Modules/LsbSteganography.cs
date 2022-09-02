using System;
using System.IO;

namespace QuartzCryptoTools.Modules
{
    internal class LsbSteganography
    {
        private static String FileLocation = null;
        private static void Overview()
        {
            Program.PrintHead();
            Console.WriteLine("                LSB Steganography");
            if (FileLocation == null)
            {
                Console.WriteLine("================================================");
                Console.WriteLine("Please type location of .jpg/.png/.bmp image");
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
