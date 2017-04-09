using QTFK.Extensions.Collections.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.FileSystem
{
    public static class FileExtension
    {
        /// <summary>
        /// Tries to move the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try moving the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool TryMoveTo(this FileInfo file, string destFileName, Action<Exception, int, int> onError = null)
        {
            return TryMoveTo(file, destFileName, 3, onError);
        }

        /// <summary>
        /// Tries to move the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try moving the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool TryMoveTo(this FileInfo file, string destFileName, int attempts, Action<Exception, int, int> onError = null)
        {
            for (int i = 1; i <= attempts; ++i)
            {
                try
                {
                    file.MoveTo(destFileName);
                    return true;
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex, i, attempts);
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to move the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try moving the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool TryCopyTo(this FileInfo file, string destFileName, bool overwrite, Action<Exception, int, int> onError = null)
        {
            return TryCopyTo(file, destFileName, overwrite, 3, onError);
        }

        /// <summary>
        /// Tries to copy the file 'attempts' times
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="destFileName">Target path and file name</param>
        /// <param name="attempts">Numbers of times to try coping the file.</param>
        /// <param name="onError">Action to call on each attempt, passing the throwed exception and the attempt number.</param>
        /// <returns>Return true or false indicating if the file has been moved.</returns>
        public static bool TryCopyTo(this FileInfo file, string destFileName, bool overwrite, int attempts, Action<Exception, int, int> onError = null)
        {
            for (int i = 1; i <= attempts; ++i)
            {
                try
                {
                    file.CopyTo(destFileName, overwrite);
                    return true;
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex, i, attempts);
                }
            }
            return false;
        }

        public static string OnlyName(this FileInfo info)
        {
            string ext = info.Extension.StartsWith(".") ? info.Extension.Substring(1) : info.Extension;
            return info.Name.RemoveExtension(ext);
        }

        public static string RemoveExtension(this string filename, string extension)
        {
            if (!string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(extension) && filename.EndsWith(extension))
                return filename.Substring(0, filename.Length - (extension.Length + 1));
            return filename;
        }

    }
}
