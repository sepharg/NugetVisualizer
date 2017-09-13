namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Domain;

    public class ProjectRepository
    {
        private readonly IConfigurationHelper _configurationHelper;

        public ProjectRepository(IConfigurationHelper configurationHelper)
        {
            _configurationHelper = configurationHelper;
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
            using (var db = new NugetVisualizerContext(_configurationHelper))
            {
                return db.Projects.Include(x => x.ProjectPackages).ThenInclude(y => y.Package).ToList();
            }

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
            using (var db = new NugetVisualizerContext(_configurationHelper))
            {
                var existingProject = db.Projects.SingleOrDefault(x => x.Name == project.Name);
                if (existingProject == null)
                {
                    foreach (var packageId in packageIds)
                    {
                        project.ProjectPackages.Add(new ProjectPackage() { ProjectName = project.Name, PackageId = packageId });
                    }
                    db.Projects.Add(project);
                    db.SaveChanges();
                }
            }
        }

        public void SaveProjects(List<Project> projects)
        {
            using (var db = new NugetVisualizerContext(_configurationHelper))
            {
                db.Projects.AddRange(projects);
                db.SaveChanges();
            }

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
    }
}
