using QTFK.Extensions.Collections.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.FileSystem
{
    public delegate void FileOperationAttempDelegate(Exception exception, int attemptIteration, int totalAttempts);

    public static class FileExtension
    {
        private const int DEFAULT_ATTEMPTS = 3;

        /// <summary>
        /// Tries to move the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try moving the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool tryMoveTo(this FileInfo file, string destFileName, FileOperationAttempDelegate onError = null)
        {
            return prv_tryMoveTo(file, destFileName, DEFAULT_ATTEMPTS, onError);
        }

        /// <summary>
        /// Tries to move the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try moving the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool tryMoveTo(this FileInfo file, string destFileName, int attempts, FileOperationAttempDelegate onError = null)
        {
            return prv_tryMoveTo(file, destFileName, attempts, onError);
        }

        /// <summary>
        /// Tries to move the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try moving the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool tryCopyTo(this FileInfo file, string destFileName, bool overwrite, FileOperationAttempDelegate onError = null)
        {
            return prv_tryCopyTo(file, destFileName, overwrite, DEFAULT_ATTEMPTS, onError);
        }

        /// <summary>
        /// Tries to copy the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try coping the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool tryCopyTo(this FileInfo file, string destFileName, bool overwrite, int attempts, FileOperationAttempDelegate onError = null)
        {
            return prv_tryCopyTo(file, destFileName, overwrite, attempts, onError);
        }

        public static string getNameWithoutExtension(this FileInfo info)
        {
            string ext, name;

            ext = info.Extension.StartsWith(".")
                ? info.Extension.Substring(1)
                : info.Extension
                ;

            name = prv_removeExtension(info.Name, ext);

            return name;
        }

        public static string getNameWithoutExtension(this string filename, string extension)
        {
            return prv_removeExtension(filename, extension);
        }

        public static IEnumerable<string> readBlocks(this FileInfo file, string delimiter)
        {
            return prv_readBlocks(file.FullName, delimiter);
        }

        public static IEnumerable<string> readBlocks(string fileName, string delimiter)
        {
            return prv_readBlocks(fileName, delimiter);
        }

        private static IEnumerable<string> prv_readBlocks(string fileName, string delimiter)
        {
            delimiter = delimiter.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(delimiter))
            {
                yield return System.IO.File.ReadAllText(fileName);
                yield break;
            }

            var sb = new StringBuilder();
            foreach (string line in System.IO.File
                .ReadLines(fileName, Encoding.UTF8)
                )
            {
                if (line.Trim().ToLower() == delimiter)
                {
                    yield return sb.ToString();
                    sb.Clear();
                }
                else
                {
                    sb.AppendLine(line);
                }
            }
        }

        private static bool prv_tryMoveTo(FileInfo file, string destFileName, int attempts, FileOperationAttempDelegate onError = null)
        {
            return prv_tryFileOperation(() => file.MoveTo(destFileName), attempts, onError);
        }

        private static bool prv_tryCopyTo(FileInfo file, string destFileName, bool overwrite, int attempts, FileOperationAttempDelegate onError = null)
        {
            return prv_tryFileOperation(() => file.CopyTo(destFileName, overwrite), attempts, onError);
        }

        private static bool prv_tryFileOperation(Action fileOperationFunction, int attempts, FileOperationAttempDelegate onError = null)
        {
            for (int i = 1; i <= attempts; ++i)
            {
                try
                {
                    fileOperationFunction();
                    return true;
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex, i, attempts);
                }
            }
            return false;
        }

        private static string prv_removeExtension(string filename, string extension)
        {
            string resultFileName;

            resultFileName = !string.IsNullOrEmpty(filename) 
                && !string.IsNullOrEmpty(extension) 
                && filename.EndsWith(extension)
                ? filename.Substring(0, filename.Length - (extension.Length + 1))
                : filename
                ;

            return resultFileName;
        }

    }
}
