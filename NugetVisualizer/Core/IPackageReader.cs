namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.PackageParser;

    public interface IPackageReader
    {
        /// <summary>
        /// The get packages contents async.
        /// </summary>
        /// <param name="projectIdentifier">
        /// The project identifier.
        /// </param>
        /// <returns> 
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="CannotGetPackagesContentsException">Thrown when the packages contents cannot be returned</exception>
        Task<List<IPackageContainer>> GetPackagesContentsAsync(IProjectIdentifier projectIdentifier);
    }
}