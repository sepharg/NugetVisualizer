using System;

namespace ConsoleVisualizer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Autofac;

    using Boostrapper;

    using ConsoleTables;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.FileSystem;
    using NugetVisualizer.Core.Github;
    using NugetVisualizer.Core.Repositories;

    class Program
    {
        static void Main(string[] args)
        {
            var container = AutofacContainerFactory.GetBuilder().Build();
            //var projects2 = new GithubProjectParser().ParseProjects(projectIdentifiers).ToList();
            Console.WriteLine("1.- Folder Search");
            Console.WriteLine("2.- Github Search");
            Console.WriteLine("3.- Read Saved Projects");
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
                        
                        var repoReader = container.Resolve<FileSystemRepositoryReader>();

                        var projects = container.Resolve<FileSystemProjectParser>().ParseProjects(repoReader.GetProjects(rootPath, filters.Split(' '))).ToList();
                        foreach (var project in projects)
                        {
                            Console.WriteLine($"{project.Name} has {FormatPackages(project.ProjectPackages.Select(x => x.Package).ToList())}");
                        }
                        container.Resolve<ProjectRepository>().SaveProjects(projects);
                        break;
                    }
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    {
                        break;
                    }
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    {
                        var projects = container.Resolve<ProjectRepository>().LoadProjects();
                        
                        foreach (var package in projects.SelectMany(x => x.ProjectPackages))
                        {
                            var table = new ConsoleTable(projects.SelectMany(x => x.ProjectPackages.Select(y => y.Package).Where(z => z.Name == package.Package.Name).Select(y => y.Version)).ToArray());
                            foreach (var project in projects)
                            {
                                table.AddRow(project.Name);
                            }
                            
                        }

                        
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