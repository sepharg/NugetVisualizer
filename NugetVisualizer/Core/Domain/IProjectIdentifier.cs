namespace NugetVisualizer.Core.Domain
{
    public interface IProjectIdentifier
    {
        string SolutionName { get; }

        string Path { get; }

        string RepositoryName { get; }
    }

    public class ProjectIdentifier : IProjectIdentifier
    {
        public ProjectIdentifier(string solutionName, string repositoryName, string path)
        {
            SolutionName = solutionName;
            RepositoryName = repositoryName;
            Path = path;
        }

        public string SolutionName { get; }

        public string Path { get; }

        public string RepositoryName { get; }
    }
}