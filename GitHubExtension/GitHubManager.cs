using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        ///  Access GitHub's Repositories API.
        /// </summary>
        /// <param name="owner">The owner of the repository.</param>
        /// <param name="name">The name of the repository.</param>
        /// <returns></returns>
        public Clients.RepositoriesClient Repository(string owner, string name)
        {
            return new Clients.RepositoriesClient(_githubClient, owner, name);
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
        ///  Returns a user for the current authenticated user.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<User> GetCurrentUserAsync(string token)
        {
            return await _githubClient.User.Current();
        }
    }
}
