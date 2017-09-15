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
        private static IEnumerable<Package> allPackages;

        private static IEnumerable<Project> projectsThatContainPackage;

        static void Main(string[] args)
        {
            var container = AutofacContainerFactory.GetBuilder().Build();

            Console.WriteLine("1.- Folder Parsing");
            Console.WriteLine("2.- Github Parsing");
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
                            Console.WriteLine($"{project.Name} parsed");
                        }
                        break;
                    }
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    {
                        Console.WriteLine("Please enter a space separated list of filters: ");
                        var filters = Console.ReadLine();

                        var repoReader = container.Resolve<GithubRepositoryReader>();

                        var projects = container.Resolve<GithubProjectParser>().ParseProjects(repoReader.GetProjects("photobox", filters.Split(' '))).ToList();
                        foreach (var project in projects)
                        {
                            Console.WriteLine($"{project.Name} parsed");
                        }
                        break;
                    }
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    {
                        var projects = container.Resolve<ProjectRepository>().LoadProjects();
                        var allPackages = container.Resolve<PackageRepository>().LoadPackages();

                        var distinctPackageNames = allPackages.GroupBy(x => x.Name).Select(x => x.First().Name);
                        foreach (var packageName in distinctPackageNames)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(packageName);
                            Console.ResetColor();
                            var allVersionsForPackage = allPackages.GroupBy(x => x.Name).Single(x => x.Key.Equals(packageName)).OrderBy(x => x.Version).Select(x => x.Version).ToList();
                            var header = new List<string>();
                            header.Add("Package");
                            header.AddRange(allVersionsForPackage);
                            var table = new ConsoleTable(header.ToArray());
                            projectsThatContainPackage = projects.Where(p => p.ProjectPackages.Any(pp => pp.Package.Name.Equals(packageName)));
                            foreach (var project in projectsThatContainPackage)
                            {
                                table.AddRow(GetProjectRow(project, packageName, allVersionsForPackage));
                            }
                            table.Write();
                            Console.WriteLine();
                        }
                        break;
                    }
                default: throw new InvalidOperationException();
            }

            Console.WriteLine();
            Console.Write("press any key to exit...");
            Console.ReadKey();
        }

        private static string[] GetProjectRow(Project project, string packageName, List<string> packageVersionsList)
        {
            var rowContents = new string [packageVersionsList.Count + 1];
            rowContents[0] = project.Name;
            int i = 1;
            foreach (var packageVersion in packageVersionsList)
            {
                if (project.ProjectPackages.Where(x => x.Package.Name == packageName)
                                           .Any(pp => pp.Package.Version.Equals(packageVersion)))
                {
                    rowContents[i] = "X";
                }
                else
                {
                    rowContents[i] = string.Empty;
                }
                i++;
            }
            return rowContents;
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