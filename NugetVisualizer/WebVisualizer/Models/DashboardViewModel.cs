namespace WebVisualizer.Models
{
    using System.Linq;

    public class MostUsedPackagesViewModel
    {
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

    public class DashboardViewModel
    {
        public MostUsedPackagesViewModel MostUsedPackagesViewModel { get; set; }
    }
}
