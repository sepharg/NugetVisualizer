# NugetVisualizer
Tool intended to help visualize all of the nuget packages and their corresponding versions for a set of given repositories

### Process

Initially, the data must be harvested. This can be done via the file system or by using the github API.
A console app is provided for testing purposes (and can be used as code examples on how to use)
Once the data is harvested, it can be visualized
All data is stored in a Sqlite database

### Configuration

A configuration.development.json file must be created to run the tests and the application. Use configuration.json as a template.
The following variables are supported:

 - GithubOrganization : TBD
 - GithubToken: OAuth token needed to connect to Github's API. For more information, read more about [personal access tokens](https://github.com/blog/1509-personal-api-tokens)
 - Dbpath: This is the path where your Sqlite database will be created and the data stored

### Initial setup

Run the following command in a Windows prompt under Core\

    dotnet ef database update

This will create the database under the path defined by the *Dbpath* variable
