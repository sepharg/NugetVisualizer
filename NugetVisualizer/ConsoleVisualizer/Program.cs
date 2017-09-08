using System;

namespace ConsoleVisualizer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.FileSystem;
    using NugetVisualizer.Core.Github;

    class Program
    {
        static void Main(string[] args)
        {
            //var projects2 = new GithubProjectParser().ParseProjects(projectIdentifiers).ToList();
            Console.WriteLine("1.- Folder Search");
            Console.WriteLine("2.- Github Search");
            var option = Console.ReadKey();
            Console.WriteLine();
            switch (option.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    {
                        Console.WriteLine("Please enter the root path where the projects are located: ");
                        var rootPath = Console.ReadLine();
                        Console.WriteLine("Please enter a space separated list of filters: ");
                        var filters = Console.ReadLine();

                        var repoReader = new FileSystemRepositoryReader();

                        var projects = new FileSystemProjectParser().ParseProjects(repoReader.GetProjects(rootPath, filters.Split(' '))).ToList();
                        foreach (var project in projects)
                        {
                            Console.WriteLine($"{project.Name} has {FormatPackages(project.Packages)}");
                        }
                        break;
                    }
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    {
                        break;
                    }
                default: throw new InvalidOperationException();
            }

            Console.WriteLine();
            Console.Write("press any key to exit...");
            Console.ReadKey();
        }

        private static string FormatPackages(List<Package> projectPackages)
        {
            string res = string.Empty;
            foreach (var projectPackage in projectPackages)
            {
                res += projectPackage.Name + " v" + projectPackage.Version + " - ";
            }
            return res;
        }
    }
}