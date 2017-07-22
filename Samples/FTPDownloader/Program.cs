using QTFK.Extensions;
using QTFK.Extensions.Collections.Casting;
using QTFK.Services;
using QTFK.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var info = new DefaultConsoleArgsFactory()
                .Build()
                .SetDescription("Downloads remotes files from an FTP or SFTP.")
                .SetCaseSensitive(false)
                .Parse(args, b => new
                {
                    Host = b.Required("host","Remote ip or domain."),
                    Port = b.Required<int>("port", "Server listening port."),
                    User = b.Optional("user", "User name for authentication required access.", "anonymous"),
                    Pass = b.Optional("pass", "The password for user name.", "anonymous"),
                    SourceFile = b.Required(1,"source_file", "remote full file path (ex: /dir1/dir2/someFile.txt)"),
                    TargetFolder = b.Optional(2, "target_folder", @"Folder on to download file. (ex: C:\dir3\dir4\)", Environment.CurrentDirectory),
                    Retries = b.Optional("retries", "Number of times to retry connection and file download", 3),
                    Overwrite = b.Flag("overwrite", "Replaces file in target folder if already exists."),
                });

            //at this time we have all needed info for our ftp app. 
            Console.WriteLine($@"
Connecting to {info.Host}:{info.Port}...
Connection OK!

Downloading {info.SourceFile} (try 1 of {info.Retries})...
3%...
5%...
8%...
13%...
21%...
34%...
55%...
89%...
100% done!

Disconnected!
");
        }
    }
}
