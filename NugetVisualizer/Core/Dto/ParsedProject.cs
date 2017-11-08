namespace NugetVisualizer.Core.Dto
{
    public class ParsedProject
    {
        public ParsedProject(string projectName, string repositoryName, int projectPackageCount)
        {
            ProjectName = projectName;
            RepositoryName = repositoryName;
            ProjectPackageCount = projectPackageCount;
        }

        public string ProjectName { get; }

        public string RepositoryName { get;  }

        public int ProjectPackageCount { get; set; }
    }
}
