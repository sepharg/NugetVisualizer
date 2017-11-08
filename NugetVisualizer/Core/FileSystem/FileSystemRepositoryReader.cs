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
            var projects = new List<IProjectIdentifier>();
            ParseDirectory(rootPath, projects, SearchOption.TopDirectoryOnly);

            var projectDirectories = Directory.GetDirectories(rootPath, $"*{string.Join("*", filters)}*");
            foreach (var projectDirectory in projectDirectories)
            {
                ParseDirectory(projectDirectory, projects, SearchOption.AllDirectories);
            }

            return projects.OrderBy(proj => proj.RepositoryName).ToList();
        }

        public Task<List<IProjectIdentifier>> GetProjectsAsync(string rootPath, string[] filters)
        {
            return Task.FromResult(GetProjects(rootPath, filters));
        }

        private void ParseDirectory(string projectDirectory, List<IProjectIdentifier> projects, SearchOption searchOption)
        {
            var allSolutions = Directory.GetFiles(projectDirectory, "*.sln", searchOption);
            foreach (var solution in allSolutions)
            {
                var fileName = Path.GetFileName(solution);
                var directoryName = Path.GetDirectoryName(solution);
                projects.Add(new ProjectIdentifier(fileName.Substring(0, fileName.Length - 4), directoryName, directoryName));
            }
        }
    }
}
