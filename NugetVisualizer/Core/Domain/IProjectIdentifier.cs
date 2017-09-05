namespace NugetVisualizer.Core.Domain
{
    public interface IProjectIdentifier
    {
        string Name { get; }

        string Path { get; }
    }
}