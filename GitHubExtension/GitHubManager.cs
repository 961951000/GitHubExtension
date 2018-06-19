using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Octokit;

namespace GitHubExtension
{
    public class GitHubManager
    {
        public readonly IGitHubClient _githubClient;

        public GitHubManager(string token)
        {
            _githubClient = new GitHubClient(new ProductHeaderValue(Assembly.GetEntryAssembly().GetName().Name))
            {
                Credentials = new Credentials(token)
            };
        }

        /// <summary>
        /// Access GitHub's Repositories API.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="name">The name of the repository.</param>
        /// <remarks>
        /// https://developer.github.com/v3/repos/
        /// </remarks>
        /// <returns></returns>
        public Clients.RepositoriesClient Repository(string owner, string name)
        {
            return new Clients.RepositoriesClient(_githubClient, owner, name);
        }

        /// <summary>
        /// Access GitHub's Users API.
        /// </summary>
        /// <remarks>
        /// https://developer.github.com/v3/users/
        /// </remarks>
        /// <returns></returns>
        public Clients.UserClient User()
        {
            return new Clients.UserClient();
        }

        /// <summary>
        /// Access GitHub's Apps API.
        /// </summary>
        /// <remarks>
        /// https://developer.github.com/v3/apps/
        /// </remarks>
        /// <returns></returns>
        public Clients.GitHubAppsClient GitHubAppsClient()
        {
            return new Clients.GitHubAppsClient();
        }

        /// <summary>
        /// Gets user collection by token collection.
        /// </summary>
        /// <param name="tokens">The token.</param>
        /// <returns></returns>
        public async Task<Dictionary<string, User>> GetUserListAsync(IEnumerable<string> tokens)
        {
            var dict = new Dictionary<string, User>();

            foreach (var token in tokens)
            {
                dict.Add(token, await GetUserAsync(token));
            }

            return dict;
        }

        /// <summary>
        /// Gets a user by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Represents a user on GitHub.</returns>
        public async Task<User> GetUserAsync(string token)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue(Assembly.GetEntryAssembly().GetName().Name))
            {
                Credentials = new Credentials(token)
            };

            return await githubClient.User.Current();
        }
    }
}
