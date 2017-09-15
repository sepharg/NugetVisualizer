namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Domain;

    public class ProjectRepository : IDisposable
    {
        private readonly IConfigurationHelper _configurationHelper;

        private readonly NugetVisualizerContext _dbContext;

        public ProjectRepository(IConfigurationHelper configurationHelper, DbContext dbContext)
        {
            _configurationHelper = configurationHelper;
            _dbContext = dbContext as NugetVisualizerContext;
            //CreateDbIfNotExists();
        }

        public void DeleteProjects()
        {
            /*  using (var db = new LiteDatabase(@"nugetVisualizer.db"))
              {
                  db.DropCollection("project");
              }*/
        }

        public List<Project> LoadProjects()
        {
            return _dbContext.Projects.Include(x => x.ProjectPackages).ThenInclude(y => y.Package).ToList();


                /* using (var db = new LiteDatabase(@"nugetVisualizer.db"))
                 {
                     var projectCollection = db.GetCollection<Project>();
                     var enumerable = projectCollection.Find(x => x.Name.Contains("Project 0"));

                     return projectCollection.FindAll().ToList();
                 }*/

                /*using (var fileStream = new FileStream(@"projects.csv", FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                using (var csvReader = new CsvReader(streamReader))
                {
                    var projects = csvReader.GetRecord<List<Project>>();
                    return projects;
                }*/
                return null;
        }

        public void Add(Project project, IEnumerable<int> packageIds)
        {
            var existingProject = _dbContext.Projects.SingleOrDefault(x => x.Name == project.Name);
            if (existingProject == null)
            {
                foreach (var packageId in packageIds)
                {
                    project.ProjectPackages.Add(new ProjectPackage() { ProjectName = project.Name, PackageId = packageId });
                }
                _dbContext.Projects.Add(project);
                _dbContext.SaveChanges();
            }
        }

        public void SaveProjects(List<Project> projects)
        {
            _dbContext.Projects.AddRange(projects);
            _dbContext.SaveChanges();

           /* using (var connection = new SQLiteConnection($"Data Source={_databaseName};Version=3;"))
            {

            }*/


            /*   using (var db = new LiteDatabase(@"nugetVisualizer.db"))
               {
                   var projectCollection = db.GetCollection<Project>();
                   projectCollection.InsertBulk(projects);
               }*/

            /*using (var connection = new SqliteConnection("Filename=" + path))
            {
                connection.Open();

                using (var reader = connection.ExecuteReader("SELECT Name FROM Person;"))
                {
                    while (reader.Read())
                    {

                    }
                }
            }*/


            /*using (var fileStream = new FileStream(@"projects.csv", FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.WriteRecords(projects);
            }*/
        }

    /*private void CreateDbIfNotExists()
        {
            if (!File.Exists(_databaseName))
            {
                SQLiteConnection.CreateFile(_databaseName);
            }
        }*/
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
