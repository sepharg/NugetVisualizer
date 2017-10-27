namespace WebVisualizer.Services
{
    using System.Collections.Generic;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    public class SnapshotService
    {
        private readonly ISnapshotRepository _snapshotRepository;

        public SnapshotService(ISnapshotRepository snapshotRepository)
        {
            _snapshotRepository = snapshotRepository;
        }

        public List<Snapshot> GetSnapshots()
        {
            return _snapshotRepository.GetAll();
        }
    }
}
