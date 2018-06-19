using System.Threading.Tasks;
using Octokit;

namespace GitHubExtension.Clients
{
    public class UserClient
    {
        private readonly IUsersClient _userClient;

        /// <summary>
        /// Returns a Octokit.User for the current authenticated user.
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetCurrentAsync()
        {
            return await _userClient.Current();
        }
    }
}
