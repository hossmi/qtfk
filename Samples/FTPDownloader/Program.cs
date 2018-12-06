using QTFK.Extensions;
using QTFK.Services.ConsoleArgsServices;
using System;

namespace FTPDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var info = ConsoleArgsService.createDefault()
                .setDescription("Downloads remotes files from an FTP or SFTP.")
                .setCaseSensitive(false)
                .Parse(args, b => new
                {
                    Host = b.getRequired("host","Remote ip or domain."),
                    Port = b.setRequired<int>("port", "Server listening port."),
                    User = b.getOptional("user", "User name for authentication required access.", "anonymous"),
                    Pass = b.getOptional("pass", "The password for user name.", "anonymous"),
                    SourceFile = b.getRequired(1,"source_file", "remote full file path (ex: /dir1/dir2/someFile.txt)"),
                    TargetFolder = b.getOptional(2, "target_folder", @"Folder on to download file. (ex: C:\dir3\dir4\)", Environment.CurrentDirectory),
                    Retries = b.setOptional("retries", "Number of times to retry connection and file download", 3),
                    Overwrite = b.getFlag("overwrite", "Replaces file in target folder if already exists."),
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
