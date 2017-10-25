using System;

namespace ConsoleVisualizer
{
    using System.Collections.Generic;
    using System.Linq;

    using Autofac;

    using Boostrapper;

    using ConsoleTables;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    class Program
    {
        private static IEnumerable<Package> allPackages;

        private static IEnumerable<Project> projectsThatContainPackage;

        static void Main(string[] args)
        {
            var container = AutofacContainerFactory.GetBuilder().Build();

            container.Resolve<NugetVisualizerContext>().Database.EnsureCreated();

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
                        var snapshots = container.Resolve<ISnapshotRepository>().GetAll();
                        string snapshotName = string.Empty;
                        int snapshotVersion = default(int);
                        ReadSnapshotName(snapshots, ref snapshotName, ref snapshotVersion);
                        
                        Console.WriteLine("Please enter the root path where the projects are located: ");
                        var rootPath = Console.ReadLine();
                        Console.WriteLine("Please enter a space separated list of filters: ");
                        var filters = Console.ReadLine();

                        var processor = container.Resolve<IProcessor>(new TypedParameter(typeof(ProjectParserType), ProjectParserType.FileSystem));

                        if (!string.IsNullOrEmpty(snapshotName))
                        {
                            DoProcess(processor, rootPath, filters, snapshotName);
                        }
                        else
                        {
                            DoProcess(processor, rootPath, filters, snapshotVersion);
                        }
                        
                        break;
                    }
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    {
                        var snapshots = container.Resolve<ISnapshotRepository>().GetAll();
                        string snapshotName = string.Empty;
                        int snapshotVersion = default(int);
                        ReadSnapshotName(snapshots, ref snapshotName, ref snapshotVersion);

                        Console.WriteLine("Please enter the name of your github organization: ");
                        var rootPath = Console.ReadLine();
                        Console.WriteLine("Please enter a space separated list of filters: ");
                        var filters = Console.ReadLine();

                        var processor = container.Resolve<IProcessor>(new TypedParameter(typeof(ProjectParserType), ProjectParserType.Github));

                        if (!string.IsNullOrEmpty(snapshotName))
                        {
                            DoProcess(processor, rootPath, filters, snapshotName);
                        }
                        else
                        {
                            DoProcess(processor, rootPath, filters, snapshotVersion);
                        }
                        break;
                    }
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    {
                        var projects = container.Resolve<IProjectRepository>().LoadProjects();
                        var allPackages = container.Resolve<IPackageRepository>().GetPackages();

                        var distinctPackageNames = allPackages.GroupBy(x => x.Name).Select(x => x.First().Name);
                        foreach (var packageName in distinctPackageNames)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(packageName);
                            Console.Out.Flush();
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

        private static void ReadSnapshotName(List<Snapshot> snapshots, ref string snapshotName, ref int snapshotVersion)
        {
            if (snapshots.Count == 0)
            {
                Console.WriteLine("Please enter the name for the snapshot to save the data under: ");
                snapshotName = Console.ReadLine();
            }
            else
            {
                Console.WriteLine(
                    "If you wish to append data to an existing snapshot, please type in the version number from one of the following. Otherwise, type in a new snapshot name:");
                foreach (var snapshot in snapshots)
                {
                    Console.WriteLine($"[{snapshot.Version}] - {snapshot.Name}");
                }
                var input = Console.ReadLine();
                if (!int.TryParse(input, out snapshotVersion))
                {
                    snapshotName = input;
                }
            }
        }

        private static void DoProcess(IProcessor processor, string rootPath, string filters, string newSnapshotName)
        {
            var projectParsingResult = processor.Process(rootPath, filters.Split(' '), newSnapshotName).GetAwaiter().GetResult();
            OutputProcessResults(projectParsingResult);
        }

        private static void DoProcess(IProcessor processor, string rootPath, string filters, int versionNumber)
        {
            var projectParsingResult = processor.Process(rootPath, filters.Split(' '), versionNumber).GetAwaiter().GetResult();
            OutputProcessResults(projectParsingResult);
        }

        private static void OutputProcessResults(ProjectParsingResult projectParsingResult)
        {
            var projects = projectParsingResult.ParsedProjects.ToList();

            foreach (var project in projects)
            {
                Console.WriteLine($"{project.Name} parsed");
            }
            if (!projectParsingResult.AllExistingProjectsParsed)
            {
                Console.WriteLine("Not all projects could be parsed, please rerun to continue after the last successful parsed project");
            }
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