using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.Compilers
{
    public class CompilerFail
    {
        public CompilerFail(string fileName, int line, int column, string errorNumber, string errorText, bool isWarning)
        {
            this.FileName = fileName;
            this.Line = line;
            this.Column = column;
            this.ErrorNumber = errorNumber;
            this.ErrorText = errorText;
            this.IsWarning = isWarning;
        }

        public int Line { get; }
        public int Column { get; }
        public string ErrorNumber { get; }
        public string ErrorText { get; }
        public bool IsWarning { get; }
        public string FileName { get; }
    }
}
