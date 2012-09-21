using System;
using System.Collections.Generic;
using ContinuousSourceControl.BL;
using ContinuousSourceControl.DataAccess.RavenDB;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;
using ContinuousSourceControl.Model.Domain.Changes;

namespace ContinuousSourceControl.ConsoleHost
{
    class Program
    {
        private static Application _application;

        // Expect arguments as -path=<> -project=<>
        static void Main(string[] args)
        {
            string path = null;
            string projectName = null;

            foreach (string s in args)
            {
                if (s.StartsWith("-path"))
                {
                    path = s.Split('=')[1];
                    Console.WriteLine("Using path: " + path);
                }

                if (s.StartsWith("-project"))
                {
                    projectName = s.Split('=')[1];
                    Console.WriteLine("Using project: " + projectName);
                }
            }

            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(projectName))
            {
                Console.WriteLine("Need both -path=<> and -project=<>");
            }
            else
            {
                RunApplication(projectName, path);
            }

            Console.WriteLine("Press enter to exit, or l to list files.");

            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
                Console.WriteLine();

                switch(key.Key)
                {
                    case ConsoleKey.L:
                        Console.WriteLine("-----------------------------------------------------------");
                        ListFiles(projectName);
                        Console.WriteLine("===========================================================");
                        Console.WriteLine();
                        break;
                }
            } while (key.Key != ConsoleKey.Enter);
        }

        private static void ListFiles(string projectName)
        {
            IList<ProjectFile> projectFiles = _application.GetFiles(projectName);

            foreach (var projectFile in projectFiles)
            {
                IList<FileContent> fileContents = _application.GetFileContents(projectFile);

                Console.WriteLine("File: " + projectFile.FilePath);
                Console.WriteLine("Changes: " + fileContents.Count);

                foreach (var fileContent in fileContents)
                {
                    Console.WriteLine("Version: " + fileContent.Version + ", Change: " + fileContent.ChangeType);
                }
            }
            
        }

        private static void RunApplication(string projectName, string path)
        {
            IRepository repository = new MemoryRepository();
            _application = new Application(repository, new WatcherFactory());
            Project project = _application.CreateProject(projectName, path);
            Console.WriteLine("Created project: " + project.Id);

            Console.WriteLine("Watching project folder...");
            _application.Start(project.Name);
        }
    }
}
