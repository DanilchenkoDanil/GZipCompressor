using GZipCompressor.Logic.Models;
using GZipCompressor.Utils;
using System;

namespace GZipCompressor
{
    class Program
    {
        static void Main(string[] args) {
            try {
                CompressorManager cm = new CompressorManager(args[0], args[1]);
                //cm.Compress();
                cm.Decompress();
            } catch(GZipException ex) {
                Console.WriteLine("Press SPACE to exit.");
                var exitKey = new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
                while (exitKey != Console.ReadKey(true)) { }
                Environment.Exit(1);
            }
        }
    }
}
