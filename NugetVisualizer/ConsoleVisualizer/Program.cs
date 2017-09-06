using System;

namespace ConsoleVisualizer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the root path where the projects are located: ");
            var rootPath = Console.ReadLine();
            var projectDirectories = Directory.GetDirectories(rootPath);

            var projectIdentifiers = new List<IProjectIdentifier>();
            foreach (var projectDirectory in projectDirectories)
            {
                projectIdentifiers.Add(new ProjectIdentifier(Path.GetFileName(projectDirectory), projectDirectory));
            }

            var projects = new FileSystemProjectParser().ParseProjects(projectIdentifiers).ToList();
        }
    }
}