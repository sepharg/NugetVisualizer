namespace NugetVisualizer.Core
{
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Dto;

    public interface IProcessor
    {
        /// <summary>
        /// Processes all nuget packages for the given path and sub paths. All data processed is appended to the existing snapshot passed in.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="filters"></param>
        /// <param name="snapshotVersion">The snapshot ID to appened the results to.</param>
        /// <returns></returns>
        Task<ProjectParsingResult> Process(string rootPath, string[] filters, int snapshotVersion);

        /// <summary>
        /// Processes all nuget packages for the given path and sub paths. Creates a new snapshot to store the data under the given name.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="filters"></param>
        /// <param name="snapshotName"></param>
        /// <returns></returns>
        Task<ProjectParsingResult> Process(string rootPath, string[] filters, string snapshotName);
    }
}