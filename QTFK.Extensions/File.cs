using QTFK.Extensions.Collections.Filters;
using QTFK.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK
{
    public static class File
    {
        public static IEnumerable<string> ReadBlocks(string fileName, string delimiter)
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

        public static bool TryMoveTo(string fileName, string destFileName, Action<Exception, int, int> onError = null)
        {
            return new FileInfo(fileName).TryMoveTo(destFileName, onError);
        }

        public static bool TryMoveTo(this string fileName, string destFileName, int attempts, Action<Exception, int, int> onError = null)
        {
            return new FileInfo(fileName).TryMoveTo(destFileName, attempts, onError);
        }

        public static bool TryCopyTo(this string fileName, string destFileName, bool overwrite, Action<Exception, int, int> onError = null)
        {
            return new FileInfo(fileName).TryCopyTo(destFileName, overwrite, onError);
        }

        public static bool TryCopyTo(this string fileName, string destFileName, bool overwrite, int attempts, Action<Exception, int, int> onError = null)
        {
            return new FileInfo(fileName).TryCopyTo(destFileName, overwrite, attempts, onError);
        }
    }
}
