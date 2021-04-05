using System;
using System.IO;
using System.Linq;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace rename_image_v2
{
    class Program
    {
        static int Main(string[] args)
        {
            // Create a root command with some options
            var rootCommand = new RootCommand
            {
                new Argument<string>(name:"directory",getDefaultValue:()=>".",description:"A deirectory which contains images to rename."),
                //new Option<string>("--output", getDefaultValue:()=>".",description:"A directory which renamed images will be put in")
            };

            rootCommand.Description = "This program renames image files in a directory based on their creation time";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string>((directory) =>
             {
                 (new App(directory)).Run();
             });

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }

    class App
    {
        string WorkingDirectory;
        string[] ImagePath;
        public App(string dir)
        {
            WorkingDirectory = dir;
            ImagePath = Directory.GetFiles(WorkingDirectory).Where(file =>
            {
                string extension = Path.GetExtension(file);
                return extension == ".jpg" || extension == ".png";
            }).ToArray();
        }

        public void Run()
        {
            foreach (string path in ImagePath)
            {
                for (int i = 0; ; i++)
                {
                    string newPath = GenerateNewName(path, i);
                    if (!File.Exists(newPath))
                    {
                        File.Move(path, newPath);
                        Console.WriteLine($"{Path.GetFileName(path):20} -> {Path.GetFileName(newPath):15}");
                        break;
                    }
                }
            }
        }

        string GenerateNewName(string filepath, int index)
        {
            return Path.GetDirectoryName(filepath) + "\\" + File.GetCreationTime(filepath).ToString("yyyy-MM-dd") + (index == 0 ? "" : $"({index})") + Path.GetExtension(filepath);
        }
    }
}
