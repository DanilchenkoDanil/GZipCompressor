using System;
using System.IO;

namespace GZipCompressor.Utils
{
    public class FileManager
    {
        public static FileStream OpenToRead(string path) {
            try {
                return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            } catch (FileNotFoundException ex) {
                Console.WriteLine(@"Error in '{0}' " + ex.Message, path);
                throw new GZipException();
            } catch (ArgumentException argEx) {
                Console.WriteLine(@"Error in '{0}' " + argEx.Message, path);
                throw new GZipException();
            }
        }

        public static FileStream OpenToWrite(string path) {
            try {
                return File.Open(path, FileMode.Open, FileAccess.Write);
            } catch (FileNotFoundException fileNotFoundEx) {
                Console.WriteLine(@"Error in '{0}' " + fileNotFoundEx.Message, path);
                throw new GZipException();
            } catch (ArgumentException argEx) {
                Console.WriteLine(@"Error in '{0}' " + argEx.Message, path);
                throw new GZipException();
            }
        }

        public static bool IsExist(string path) {
            try {
                return File.Exists(path);
            } catch (FileNotFoundException fileNotFoundEx) {
                Console.WriteLine(@"Error in '{0}' " + fileNotFoundEx.Message, path);
                throw new GZipException();
            } catch (ArgumentException argEx) {
                Console.WriteLine(@"Error in '{0}' " + argEx.Message, path);
                throw new GZipException();
            }
        }

        public static void Delete(string path) {
            try {
                File.Delete(path);
            } catch (FileNotFoundException fileNotFoundEx) {
                Console.WriteLine(@"Error in '{0}' " + fileNotFoundEx.Message, path);
                throw new GZipException();
            } catch (ArgumentException argEx) {
                Console.WriteLine(@"Error in '{0}' " + argEx.Message, path);
                throw new GZipException();
            }
        }

        public static void Create(string path) {
            try {
                File.Create(path).Close();
            } catch (FileNotFoundException fileNotFoundEx) {
                Console.WriteLine(@"Error in '{0}' " + fileNotFoundEx.Message, path);
                throw new GZipException();
            } catch (ArgumentException argEx) {
                Console.WriteLine(@"Error in '{0}' " + argEx.Message, path);
                throw new GZipException();
            }
        }

        public static long GetFileSize(string path) {
            long length = 0;
            try {
                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read)) {
                    length = fileStream.Length;
                }
                return length;
            } catch (FileNotFoundException fileNotFoundEx) {
                Console.WriteLine(@"Error in '{0}' " + fileNotFoundEx.Message, path);
                throw new GZipException();
            } catch (ArgumentException argEx) {
                Console.WriteLine(@"Error in '{0}' " + argEx.Message, path);
                throw new GZipException();
            }
        }
    }
}
