using GZipCompressor.Utils.Extensions;
using System;
using System.IO;
using System.Linq;

namespace GZipCompressor.Utils
{
    public static class ParameterChecker
    {
        public static string InputFilePath { get; private set; }
        public static string OutputFilePath { get; private set; }
        public static ProcessMode ProcessMode { get; private set; }

        public static void Check(string[] args) {
            if (args.Any(x => x.IsNullOrWhitespace()) || args.Length != 3 || args == null) {
                throw new ArgumentException("Wrong input parameters");
            }

            try {
                checkMode(args[0]);
            }catch(Exception ex) {
                throw new ArgumentException("Parameter 'compress/decompress is wrong.'", ex);
            }

            // Check each parameter

            if (!File.Exists(args[0])) {
                throw new ArgumentException(string.Format(@"Parameter {0} is not valid", args[1]), new FileNotFoundException("File hasnt been found.",args[1]));
            }

            InputFilePath = args[0];

            string outputFileDirectory = Path.GetDirectoryName(args[2]);
            string outputFileFileName = Path.GetFileName(args[2]);
            try {
                if (!Directory.Exists(outputFileDirectory) && outputFileFileName.IsNullOrWhitespace()) {
                    throw new Exception("Wrong output parameter.");
                }
            } catch(Exception ex) {
                throw new ArgumentException(string.Format(@"Parameter {0} is not valid", args[2]), ex);
            }

            OutputFilePath = args[1];
        }

        public static string GetHelpInfo() {
            return "Please, follow next rule: 'compress/decompress' 'inputFilePath' 'outputFilePath'";
        }

        private static void checkMode(string modeSource) {
            if (modeSource.Equals("compress", StringComparison.OrdinalIgnoreCase)) {
                ProcessMode = ProcessMode.Compress;
                return;
            }
            if (modeSource.Equals("decompress", StringComparison.OrdinalIgnoreCase)) {
                ProcessMode = ProcessMode.Decompress;
                return;
            }

            throw new Exception("Wrong parameter 'ProcessMode'");
        }
    }
}
