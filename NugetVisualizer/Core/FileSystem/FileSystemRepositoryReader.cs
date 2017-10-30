namespace NugetVisualizer.Core.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;

    public class FileSystemRepositoryReader : IRepositoryReader
    {
        public List<IProjectIdentifier> GetProjects(string rootPath, string[] filters)
        {
            var projectDirectories = Directory.GetDirectories(rootPath, $"*{string.Join("*", filters)}*");

            var projects = new List<IProjectIdentifier>();
            foreach (var projectDirectory in projectDirectories)
            {
                var allSolutions = Directory.GetFiles(projectDirectory, "*.sln", SearchOption.AllDirectories);
                foreach (var solution in allSolutions)
                {
                    var fileName = Path.GetFileName(solution);
                    var directoryName = Path.GetDirectoryName(solution);
                    projects.Add(new ProjectIdentifier(fileName.Substring(0, fileName.Length - 4), directoryName));
                }
            }
            return projects;
        }

        public Task<List<IProjectIdentifier>> GetProjectsAsync(string rootPath, string[] filters)
        {
            return Task.FromResult(GetProjects(rootPath, filters));
        }
    }
}
