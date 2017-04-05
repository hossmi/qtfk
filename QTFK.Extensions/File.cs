using QTFK.Extensions.Collections.Filters;
using System;
using System.Collections.Generic;
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
    }
}
