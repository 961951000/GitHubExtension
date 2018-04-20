using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace GitHubExtension.Clients
{
    /// <summary>
    /// Access GitHub's Repositories API.
    /// </summary>
    public class RepositoriesClient
    {
        private readonly IGitHubClient _githubClient;

        private readonly string _owner;

        private readonly string _name;

        public RepositoriesClient(IGitHubClient githubClient, string owner, string name)
        {
            this._githubClient = githubClient;
            this._owner = owner;
            this._name = name;
        }

        #region branch

        /// <summary>
        /// Gets a branch request by SHA.
        /// </summary>
        /// <param name="sha">The sha value of the reference.</param>
        /// <returns></returns>
        public async Task<Branch> GetBranchAsync(string sha)
        {
            var pullRequests = await GetAllOpenPullRequestsAsync();
            var requiredPullRequest = pullRequests.FirstOrDefault(x => x.Head?.Sha == sha);

            return await _githubClient.Repository.Branch.Get(_owner, _name, requiredPullRequest.Base.Ref);
        }

        /// <summary>
        ///  Gets all the branches for the specified repository.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Branch>> GetAllBranchAsync()
        {
            return await _githubClient.Repository.Branch.GetAll(_owner, _name);
        }

        /// <summary>
        /// Create a branch.
        /// </summary>
        /// <param name="branch">The name of the branch.</param>
        /// <param name="baseRef">The baseRef value to set this reference branch to.</param>
        /// <returns></returns>
        public async Task<Reference> CreateBranchAsync(string branch, string baseRef)
        {
            var baseBranch = await _githubClient.Repository.Branch.Get(_owner, _name, baseRef);

            return await _githubClient.Git.Reference.Create(_owner, _name, new NewReference($"refs/heads/{branch}", baseBranch.Commit.Sha));
        }

        /// <summary>
        /// Deletes a branch.
        /// </summary>
        /// <param name="branch">The name of the branch.</param>
        /// <returns></returns>
        public async Task DeleteBranchAsync(string branch)
        {
            await _githubClient.Git.Reference.Delete(_owner, _name, $"heads/{branch}");
        }

        #endregion

        #region pull request

        /// <summary>
        /// Get all open pull requests for the repository.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PullRequest>> GetAllOpenPullRequestsAsync()
        {
            return await _githubClient.PullRequest.GetAllForRepository(_owner, _name);
        }

        /// <summary>
        /// Get a pull request by number.
        /// </summary>
        /// <returns></returns>
        public async Task<PullRequest> GetPullRequestAsync(int number)
        {
            return await _githubClient.PullRequest.Get(_owner, _name, number);
        }

        /// <summary>
        /// Create a pull request.
        /// </summary>
        /// <param name="title">The title of the pull request.</param>
        /// <param name="branch">The name of the branch.</param>
        /// <param name="baseRef">The baseRef value to set this reference branch to.</param>
        /// <returns></returns>
        public async Task<PullRequest> CreatePullRequestAsync(string title, string branch, string baseRef)
        {
            return await _githubClient.PullRequest.Create(_owner, _name, new NewPullRequest(title, branch, baseRef));
        }

        /// <summary>
        /// Create a pull request.
        /// </summary>
        /// <param name="title">The title of the pull request.</param>
        /// <param name="branch">The name of the branch.</param>
        /// <param name="body">The body of the pull request.</param>
        /// <param name="baseRef">The baseRef value to set this reference branch to.</param>
        /// <returns></returns>
        public async Task<PullRequest> CreatePullRequestAsync(string title, string body, string branch, string baseRef)
        {
            return await _githubClient.PullRequest.Create(_owner, _name, new NewPullRequest(title, branch, baseRef)
            {
                Body = body
            });
        }

        /// <summary>
        /// Open a pull request.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <returns></returns>
        public async Task OpenPullRequestAsync(int number)
        {
            await _githubClient.PullRequest.Update(_owner, _name, number, new PullRequestUpdate { State = ItemState.Open });
        }

        /// <summary>
        /// Close a pull request.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <returns></returns>
        public async Task ClosePullRequestAsync(int number)
        {
            await _githubClient.PullRequest.Update(_owner, _name, number, new PullRequestUpdate { State = ItemState.Closed });
        }

        /// <summary>
        /// Update a title of the pull request.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <param name="title">Title of the pull request.</param>
        /// <returns></returns>
        public async Task<PullRequest> UpdatePullRequestTitleAsync(int number, string title)
        {
            return await _githubClient.PullRequest.Update(_owner, _name, number, new PullRequestUpdate { Title = title });
        }

        /// <summary>
        /// Merge a pull request.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <param name="title">The title for the automatic commit message.</param>
        /// <param name="message">The message that will be used for the merge commit.</param>
        /// <returns></returns>
        public async Task Merge(int number, string title, string message)
        {
            await _githubClient.PullRequest.Merge(_owner, _name, number, new MergePullRequest
            {
                CommitTitle = title,
                CommitMessage = message
            });
        }

        /// <summary>
        /// Get the pull request merge status.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <returns></returns>
        public async Task<bool> Merged(int number)
        {
           return await _githubClient.PullRequest.Merged(_owner, _name, number);
        }

        #endregion

        #region pull request file

        /// <summary>
        /// Gets files by number.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <returns></returns>
        public async Task<IEnumerable<PullRequestFile>> GetFilesAsync(int number)
        {
            return await _githubClient.PullRequest.Files(_owner, _name, number);
        }

        /// <summary>
        /// Gets a single Blob by SHA.
        /// </summary>
        /// <param name="sha">The sha value of the blob.</param>
        /// <returns></returns>
        public async Task<string> GetFileContent(string sha)
        {
            var blob = await _githubClient.Git.Blob.Get(_owner, _name, sha);
            var fileData = Convert.FromBase64String(blob.Content);

            return Encoding.UTF8.GetString(fileData);
        }

        /// <summary>
        /// Creates a commit that creates a new file in a repository.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="message">The message.</param>
        /// <param name="content">The content.</param>
        /// <param name="branch">The branch the request is for.</param>
        /// <returns></returns>
        public async Task<RepositoryContentChangeSet> CreateFileAsync(string path, string message, string content, string branch)
        {
            return await _githubClient.Repository.Content.CreateFile(_owner, _name, path, new CreateFileRequest(message, content, branch));
        }

        /// <summary>
        /// Creates a commit that deletes a file in a repository.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="message">The message.</param>
        /// <param name="branch">The branch the request is for.</param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string path, string message, string branch)
        {
            var existingFile = await _githubClient.Repository.Content.GetAllContentsByRef(_owner, _name, path, branch);

            await _githubClient.Repository.Content.DeleteFile(_owner, _name, path, new DeleteFileRequest(message, existingFile.First().Sha, branch));
        }

        /// <summary>
        /// Creates a commit that updates the contents of a file in a repository.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="message">The message.</param>
        /// <param name="content">The content.</param>
        /// <param name="branch">The branch the request is for.</param>
        /// <returns></returns>
        public async Task<RepositoryContentChangeSet> UpdateFileAsync(string path, string message, string content, string branch)
        {
            var existingFile = await _githubClient.Repository.Content.GetAllContentsByRef(_owner, _name, path, branch);

            return await _githubClient.Repository.Content.UpdateFile(_owner, _name, path, new UpdateFileRequest(message, content, existingFile.First().Sha, branch));
        }

        #endregion

        #region label

        /// <summary>
        /// Gets labels by number.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Label>> GetLabelsAsync(int number)
        {
            return await _githubClient.Issue.Labels.GetAllForIssue(_owner, _name, number);
        }

        /// <summary>
        /// Creates a label.
        /// </summary>
        /// <param name="name">The name of the label.</param>
        /// <param name="color">The color of the label.</param>
        /// <returns></returns>
        public async Task<Label> CreateLabelAsync(string name, string color)
        {
            return await _githubClient.Issue.Labels.Create(_owner, _name, new NewLabel(name, color));
        }

        /// <summary>
        /// Deletes a label.
        /// </summary>
        /// <param name="name">The name of the label.</param>
        /// <returns></returns>
        public async Task DeleteLabelAsync(string name)
        {
            await _githubClient.Issue.Labels.Delete(_owner, _name, name);
        }

        #endregion

        #region comment

        /// <summary>
        /// Gets comments by number.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <returns></returns>
        public async Task<IEnumerable<IssueComment>> GetCommentsAsync(int number)
        {
            return await _githubClient.Issue.Comment.GetAllForIssue(_owner, _name, number);
        }

        /// <summary>
        /// Creates a new comment for a pull request.
        /// </summary>
        /// <param name="number">The number of the pull request.</param>
        /// <param name="comment">The comment to add to the issue.</param>
        /// <returns></returns>
        public async Task<IssueComment> CreateCommentAsync(int number, string comment)
        {
            return await _githubClient.Issue.Comment.Create(_owner, _name, number, comment);
        }

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="id">The comment id.</param>
        /// <returns></returns>
        public async Task DeleteCommentAsync(int id)
        {
            await _githubClient.Issue.Comment.Delete(_owner, _name, id);
        }

        #endregion
    }
}
