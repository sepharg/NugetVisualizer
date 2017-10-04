namespace WebVisualizer.Models
{
    using System.Linq;

    public abstract class UsedPackagesViewModel
    {
        public abstract string CanvasId { get; }

        public abstract string ChartType { get; }

        public abstract string ChartTitle { get; }

        public int MaxToRetrieve { get; set; }

        private string _packageList;

        private string _usagesValues;

        public void SetPackageList(string[] packageNames)
        {
            _packageList = string.Join(',', packageNames.Select(name => $"\"{name}\""));
        }

        public void SetUsagesValues(int[] packageUsages)
        {
            _usagesValues = string.Join(',', packageUsages.Select(usage => usage.ToString()));
        }

        public string PackageList => _packageList;

        public string UsagesValues => _usagesValues;
    }

    public class MostUsedPackagesViewModel : UsedPackagesViewModel
    {
        public override string CanvasId => "mostUsedPackagesCanvas";

        public override string ChartType => "bar";

        public override string ChartTitle => "Most used packages";
    }

    public class LeastUsedPackagesViewModel : UsedPackagesViewModel
    {
        public override string CanvasId => "leastUsedPackagesCanvas";

        public override string ChartType => "pie";

        public override string ChartTitle => "Least used packages";
    }
}