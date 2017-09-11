namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using LiteDB;

    using NugetVisualizer.Core.Domain;

    public class ProjectRepository
    {
        public void DeleteProjects()
        {
            using (var db = new LiteDatabase(@"nugetVisualizer.db"))
            {
                db.DropCollection("project");
            }
        }

        public List<Project> LoadProjects()
        {
            using (var db = new LiteDatabase(@"nugetVisualizer.db"))
            {
                var projectCollection = db.GetCollection<Project>();
                var enumerable = projectCollection.Find(x => x.Name.Contains("Project 0"));
                return projectCollection.FindAll().ToList();
            }

            /*using (var fileStream = new FileStream(@"projects.csv", FileMode.Open))
            using (var streamReader = new StreamReader(fileStream))
            using (var csvReader = new CsvReader(streamReader))
            {
                var projects = csvReader.GetRecord<List<Project>>();
                return projects;
            }*/
            return null;
        }

        public void SaveProjects(List<Project> projects)
        {

            using (var db = new LiteDatabase(@"nugetVisualizer.db"))
            {
                var projectCollection = db.GetCollection<Project>();
                projectCollection.InsertBulk(projects);
            }

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
    }
}
