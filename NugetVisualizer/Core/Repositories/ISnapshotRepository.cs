namespace NugetVisualizer.Core.Repositories
{
    using System.Collections.Generic;

    using NugetVisualizer.Core.Domain;

    public interface ISnapshotRepository
    {
        List<Snapshot> GetAll();

        void Add(Snapshot snapshot);
    }
}