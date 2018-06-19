using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace GitHubExtension.Clients
{
    public class GitHubAppsClient
    {
        private readonly IGitHubAppsClient _gitHubAppsClient;

        public GitHubAppsClient(IGitHubAppsClient gitHubAppsClient)
        {
            this._gitHubAppsClient = gitHubAppsClient;
        }

        /// <summary>
        /// Create a time bound access token for a GitHubApp Installation that can be used to access other API endpoints (requires GitHubApp JWT token auth).
        /// </summary>
        /// <param name="installationId">The Id of the GitHub App Installation.</param>
        /// <remarks>
        /// https://developer.github.com/v3/apps/#create-a-new-installation-token https://developer.github.com/apps/building-github-apps/authentication-options-for-github-apps/#authenticating-as-an-installation
        /// https://developer.github.com/v3/apps/available-endpoints/
        /// </remarks>
        /// <returns></returns>
        public async Task<AccessToken> CreateInstallationTokenAsync(long installationId)
        {
            return await _gitHubAppsClient.CreateInstallationToken(installationId);
        }

        /// <summary>
        /// Get a single GitHub App.
        /// </summary>
        /// <param name="slug">The URL-friendly name of your GitHub App. You can find this on the settings page for your GitHub App.</param>
        /// <remarks>
        /// https://developer.github.com/v3/apps/#get-a-single-github-app
        /// </remarks>
        /// <returns></returns>
        public async Task<GitHubApp> GetAsync(string slug)
        {
            return await _gitHubAppsClient.Get(slug);
        }

        /// <summary>
        /// List installations of the authenticated GitHub App (requires GitHubApp JWT token auth).
        /// </summary>
        /// <remarks>
        /// https://developer.github.com/v3/apps/#find-installations
        /// </remarks>
        /// <returns></returns>
        public async Task<IReadOnlyList<Installation>> GetAllInstallationsForCurrentAsync()
        {
            return await _gitHubAppsClient.GetAllInstallationsForCurrent();
        }

        /// <summary>
        /// Returns the GitHub App associated with the authentication credentials used (requires GitHubApp JWT token auth).
        /// </summary>
        /// <remarks>
        /// https://developer.github.com/v3/apps/#get-the-authenticated-github-app
        /// </remarks>
        /// <returns></returns>
        public async Task<GitHubApp> GetCurrentAsync()
        {
            return await _gitHubAppsClient.GetCurrent();
        }

        /// <summary>
        /// Get a single GitHub App Installation (requires GitHubApp JWT token auth).
        /// </summary>
        /// <param name="installationId">The Id of the GitHub App Installation.</param>
        /// <remarks>
        /// https://developer.github.com/v3/apps/#get-a-single-installation
        /// </remarks>
        /// <returns></returns>
        public async Task<Installation> GetInstallationAsync(long installationId)
        {
            return await _gitHubAppsClient.GetInstallation(installationId);
        }
    }
}
