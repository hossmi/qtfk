using System;

namespace QTFK.Cmd.Tests.Models
{
    public class XCopyArgsTest
    {
        public string Source { get; internal set; }
        public string Target { get; internal set; }
        public bool Recursive { get; internal set; }
        public bool CopyEmptyFolders { get; internal set; }
        public bool VerifyEachFileSize { get; internal set; }
        public DateTime CopyAfter { get; internal set; }
        public bool Overwrite { get; internal set; }
        public int Retries { get; internal set; }
    }
}