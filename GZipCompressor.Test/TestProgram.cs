using System.IO;

namespace GZipCompressorTest
{
    class TestProgram
    {
        static string compressMode = "compress";
        static string decompressMode = "decompress";
        static string sourceFilePath = "source.txt";
        static string compressedFilePath = "compressed.txt";
        static string decompressedFilePath = "decompressed.txt";

        static void Main(string[] args) {
            string processMode = compressMode;
            string inputFilePath = sourceFilePath;
            string outputFilePath = compressedFilePath;

            //createSmallFile(inputFilePath);
            createLargeFile(inputFilePath);
            args = new string[3] {
               processMode ,inputFilePath, outputFilePath};
            GZipCompressor.Program.Main(args);
        }

        private static string letters = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghjklmnopqrstuvwxyz0123456789";
        private static void createSmallFile(string inputFilePath) {
            using (var fileStream = new FileStream(inputFilePath, FileMode.OpenOrCreate)) {
                var lettersBytes = System.Text.Encoding.ASCII.GetBytes(letters);
                while(fileStream.Length <= GZipCompressor.Utils.ProgramOptions.MinFileSizeForSingleThreadPlan)
                    fileStream.Write(lettersBytes, 0, letters.Length);
            }
        }

        private static void createLargeFile(string inputFilePath) {
            using (var fileStream = new FileStream(inputFilePath, FileMode.OpenOrCreate)) {
                var lettersBytes = System.Text.Encoding.ASCII.GetBytes(letters);
                while (fileStream.Length <= GZipCompressor.Utils.ProgramOptions.MinFileSizeForSingleThreadPlan * 2)
                    fileStream.Write(lettersBytes, 0, letters.Length);
            }
        }
    }
}
