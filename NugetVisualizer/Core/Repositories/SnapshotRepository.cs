namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NugetVisualizer.Core.Domain;

    public class SnapshotRepository : ISnapshotRepository, IDisposable
    {
        private readonly INugetVisualizerContext _dbContext;

        public SnapshotRepository(INugetVisualizerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Snapshot> GetAll()
        {
            return _dbContext.Snapshots.ToList();
        }

        public void Add(Snapshot snapshot)
        {
            _dbContext.Snapshots.Add(snapshot);
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
