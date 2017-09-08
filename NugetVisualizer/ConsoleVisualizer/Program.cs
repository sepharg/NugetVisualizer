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
            Console.WriteLine("Please enter the root path where the projects are located: ");
            var rootPath = Console.ReadLine();

            var repoReader = new FileSystemRepositoryReader();

            var projects = new FileSystemProjectParser().ParseProjects(repoReader.GetProjects(rootPath, new [] {"moonpig"})).ToList();
        }
    }
}