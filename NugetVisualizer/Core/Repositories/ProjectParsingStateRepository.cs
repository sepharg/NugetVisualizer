namespace NugetVisualizer.Core.Repositories
{
    using System.IO;

    public class ProjectParsingStateRepository : IProjectParsingState
    {
        private readonly string _projectParsingFileFullPath;
        public static string ProjectParsingStateFilename => "projectParsingState.txt";

        public ProjectParsingStateRepository()
        {
            _projectParsingFileFullPath = Path.Combine(Path.GetFullPath(Directory.GetCurrentDirectory()), ProjectParsingStateFilename);
        }

        public void SaveLatestParsedProject(string projectName)
        {
            File.WriteAllText(_projectParsingFileFullPath, projectName);
        }

        public string GetLatestParsedProject()
        {
            if (File.Exists(_projectParsingFileFullPath))
            {
                return File.ReadAllText(_projectParsingFileFullPath);
            }
            return null;
        }
    }
}
