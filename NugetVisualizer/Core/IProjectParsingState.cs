namespace NugetVisualizer.Core
{
    public interface IProjectParsingState
    {
        void SaveLatestParsedProject(string projectName);

        string GetLatestParsedProject();
    }
}