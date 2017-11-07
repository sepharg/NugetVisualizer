# NugetVisualizer
Tool intended to help visualize all of the nuget packages and their corresponding versions for a set of given repositories

[![Build Status](https://travis-ci.org/sepharg/NugetVisualizer.svg?branch=master)](https://travis-ci.org/sepharg/NugetVisualizer)

### Process

Initially, the data must be harvested. This can be done via the file system or by using the github API.
A console app is provided for testing purposes (and can be used as code examples on how to use)
The data can be harvested from the Harvest menu on the web.
Once the data is harvested, it can be visualized
All data is stored in a Sqlite database

The harvesting process looks for .sln files in the hierarchy, and creates a Project for each solution. Then it looks for packages.config files in all children directories and associates those packages with the project, creating new Packages each time a new one (package name + version) is found.
.net core projects are -still- not supported

### Requirements

The solution is built and requires [.net Core 2.0](https://www.microsoft.com/net/download/core) to run

### Configuration

A configuration.json file must be created to run the tests and the application.
The following variables are supported:

 - GithubOrganization : The name of the Github organization to read repositories from
 - GithubToken: OAuth token needed to connect to Github's API. You need to create a token and give it repo access. For more information, read more about [personal access tokens](https://github.com/blog/1509-personal-api-tokens)
 - Dbpath: This is the path where your Sqlite database will be created and the data stored. Do not include this variable in the UnitTests, as the DB is autogenerated.

### Initial setup

##### Database

The database is initially created when the console app or website runs, it is created under the path defined by the *Dbpath* variable

##### Configuration

Create a file named configuration.json in the root of ConsoleVisualizer folder, and also for the WebVisualizer.
This can be used as a template:

    {  "GithubOrganization": "YOUR ORGANIZATION HERE",
       "GithubToken": "YOUR GITHUB TOKEN HERE",
       "Dbpath" : "C:\\FullPathToSqlLiteDatabase\\myDatabase.db" 
    }

##### Running Tests Locally

Just run them normally. At the moment of writing, Resharper's test runner doesn't work on .net core, so use the visual studio test runner.
The Integration Tests that connect to github cannot be run locally, because a token is needed for a dummy organization i've created. Github doesn't let me check in tokens into source control for security reasons, so these tests run only on the travis build.	
	
##### Sample Screenshots

![Create Snapshot](create-snapshot.PNG?raw=true "Create Snapshot")

Snapshot creation (data harvest) page

![Append Snapshot](append-snapshot.PNG?raw=true "Append Snapshot")

Once a Snapshot is created, data can be appended to it (make sure you're not adding the same repositories twice though)

![Dashboard Example](dashboard.PNG?raw=true "Dashboard Example")

Example dashboard with widgets (more to come in future versions)

![Package List With Versions](packageList.PNG?raw=true "Package List With Versions")

Main page where the packages are ordered by distinct versions count and we can see which repository uses which version

##### Useful Tools

https://github.com/sqlitebrowser/sqlitebrowser/
