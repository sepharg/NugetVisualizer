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

            var directories = new List<IProjectIdentifier>();
            foreach (var projectDirectory in projectDirectories)
            {
                directories.Add(new ProjectIdentifier(Path.GetFileName(projectDirectory), projectDirectory));
            }
            return directories;
        }

        public Task<List<IProjectIdentifier>> GetProjectsAsync(string rootPath, string[] filters)
        {
            return Task.FromResult(GetProjects(rootPath, filters));
        }
    }
}
