namespace NugetVisualizer.Core.Github
{
    using Octokit;

    public interface IGithubClientFactory
    {
        IGitHubClient GetClient(string organization, ICredentialStore credentials);
    }

    public class GithubClientFactory : IGithubClientFactory
    {
        public IGitHubClient GetClient(string organization, ICredentialStore credentials)
        {
            return new GitHubClient(new ProductHeaderValue(organization), credentials);
        }
    }
}
