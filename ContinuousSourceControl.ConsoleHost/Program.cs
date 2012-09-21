using System;
using ContinuousSourceControl.BL;
using ContinuousSourceControl.DataAccess.RavenDB;
using ContinuousSourceControl.DataAccess.RavenDB.Interfaces;
using ContinuousSourceControl.Model.Domain;

namespace ContinuousSourceControl.ConsoleHost
{
    class Program
    {
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

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static void RunApplication(string projectName, string path)
        {
            IRepository repository = new MemoryRepository();
            var application = new Application(repository, new WatcherFactory());
            Project project = application.CreateProject(projectName, path);
            Console.WriteLine("Created project: " + project.Id);

            Console.WriteLine("Watching project folder...");
            application.Start(project.Name);
        }
    }
}
