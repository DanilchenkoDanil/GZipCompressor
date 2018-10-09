using GZipCompressor.Logic.Models;
using GZipCompressor.Utils;
using System;


namespace GZipCompressor
{
    class Program
    {
        static void Main(string[] args) {
            try {
                ParameterChecker.Check(args);
                ProcessManager processManager = new ProcessManager(ParameterChecker.InputFilePath, ParameterChecker.OutputFilePath, ParameterChecker.ProcessMode);
                processManager.Process();
            } catch (ArgumentException argEx) {
                Console.WriteLine(argEx.ToString());
                Console.WriteLine(ParameterChecker.GetHelpInfo());
            } catch (Exception ex) {
                Console.WriteLine("Something goes wrong - {0}", ex.ToString());
                Environment.Exit(1);
            }
            Environment.Exit(0);
        }
    }
}
