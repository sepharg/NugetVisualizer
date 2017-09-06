namespace NugetVisualizer.Core.Domain
{
    public interface IProjectIdentifier
    {
        string Name { get; }

        string Path { get; }
    }

    public class ProjectIdentifier : IProjectIdentifier
    {
        public ProjectIdentifier(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; }

        public string Path { get; }
    }
}