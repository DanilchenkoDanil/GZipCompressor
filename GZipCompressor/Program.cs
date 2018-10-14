using GZipCompressor.Logic.Models;
using GZipCompressor.Utils;
using System;


namespace GZipCompressor
{
    class Program
    {
        static void Main(string[] args) {
            try {
                ProgramOptions.Check(args);
                ProcessManager processManager = new ProcessManager(ProgramOptions.InputFilePath, ProgramOptions.OutputFilePath, ProgramOptions.ProcessMode);
                processManager.Process();
            } catch (ArgumentException argEx) {
                Console.WriteLine(argEx.ToString());
                Console.WriteLine(ProgramOptions.GetHelpInfo());
            } catch (Exception ex) {
                Console.WriteLine("Something goes wrong - {0}", ex.ToString());
                Environment.Exit(1);
            }
            Environment.Exit(0);
        }
    }
}
