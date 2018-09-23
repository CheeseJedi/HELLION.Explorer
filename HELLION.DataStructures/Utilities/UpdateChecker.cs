using System;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures.Utilities
{
    /// <summary>
    /// A class to handle GitHub lookups in order to notify the user of newer version.
    /// </summary>
    /// <remarks>
    /// This will likely be fleshed out a little further in future.
    /// </remarks>
    public class UpdateChecker
    {
        /// <summary>
        /// Default constructor - requires GitHub Username and Repo name.
        /// </summary>
        /// <param name="gitHubUserName">GitHub User name</param>
        /// <param name="repositoryName">GitHub Repository name</param>
        public UpdateChecker (string gitHubUserName, string repositoryName)
        {
            // Only proceed if both the username and repository name have non-empty strings.
            if (gitHubUserName != String.Empty && repositoryName != String.Empty)
            {
                GitHubUserName = gitHubUserName;
                RepositoryName = repositoryName;
            }
            else
            {
                throw new InvalidOperationException("One or more zero-length string(s) representing GitHub Username/Repo passed to constructor.");
            }
        }

        /// <summary>
        /// The GitHub user name of which the repository will be located.
        /// </summary>
        /// <remarks>
        /// Set when using the constructor.
        /// </remarks>
        public string GitHubUserName { get; private set; } = string.Empty;

        /// <summary>
        /// The GitHub repository name of which the releases will be queried.
        /// </summary>
        /// <remarks>
        /// Set when using the constructor.
        /// </remarks>
        public string RepositoryName { get; private set; } = string.Empty;

        /// <summary>
        /// Checks current build number against the latest release on GitHub repository and 
        /// returns a MessageBox with the current and latest release version numbers.
        /// </summary>
        public void CheckForUpdates(bool notifyIfCurrent = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Environment.NewLine);

            sb.Append("Currently running version:");
            sb.Append(Environment.NewLine);
            sb.Append("v" + Application.ProductVersion);
            sb.Append(Environment.NewLine);

            sb.Append(Environment.NewLine);
            sb.Append("Latest GitHub release version:");
            sb.Append(Environment.NewLine);
            //foreach (var release in AllReleases)
            //{
            //    sb.Append(release["tag_name"] + Environment.NewLine);
            //    break;
            //}

            sb.Append(FindLatestRelease(includePreReleaseVersions: false) + Environment.NewLine);
            
            MessageBox.Show(sb.ToString(), "Version update check", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Public read-only property to access the latestRelease field and trigger the generation
        /// of the data if it doesn't already exist.
        /// </summary>
        public string LatestRelease
        {
            get
            {
                if (latestRelease == String.Empty)
                {
                    // Generate the latest release info
                    latestRelease = FindLatestRelease();
                }
                return latestRelease;
            }
        }

        /// <summary>
        /// Private field to cache the latest release version to prevent repeated web requests
        /// per session.
        /// </summary>
        private string latestRelease = string.Empty;

        /// <summary>
        /// Private field for the AllReleases Property
        /// </summary>
        private JArray _allReleases = null;

        /// <summary>
        /// The private property for the array of all releases for the repo as specified when 
        /// the object was created using the constructor.
        /// </summary>
        private JArray AllReleases
        {
            get
            {
                if (_allReleases == null)
                {
                    _allReleases = FindAllGitHubReleases();
                }
                return _allReleases;
            }
            set
            {
                if (value != null)
                {
                    _allReleases = value;
                }
            }
        }

        /// <summary>
        /// Finds the latest release from a specified GitHub repo.
        /// </summary>
        /// <param name="includePreReleaseVersions">Whether to include PreRelease versions in the check.</param>
        /// <returns></returns>
        private string FindLatestRelease(bool includePreReleaseVersions = false)
        {
            IOrderedEnumerable<JToken> orderedReleases;

            if (includePreReleaseVersions)
            {
                orderedReleases = from s in AllReleases
                                  where (bool)s["prerelease"] == false
                                  orderby (string)s["published_at"] descending
                                  select s;
            }
            else
            {
                orderedReleases = from s in AllReleases
                                  orderby (string)s["published_at"] descending
                                  select s;
            }

            string potentialLatestRelease = string.Empty;
            if (orderedReleases.Count() > 0)
            {
                // Grab the first item, as they're already sorted by reverse date order.
                potentialLatestRelease = (string)orderedReleases.First()["tag_name"];

                //foreach (var item in orderedReleases)
                //{
                //    potentialLatestRelease = (string)item["tag_name"];
                //    break;
                //}
            }
            return potentialLatestRelease;
        }

        /// <summary>
        /// Populates the  JArray of all releases in a particular GitHub repository.
        /// </summary>
        /// <returns>A JArray of all releases in the given repository.</returns>
        private JArray FindAllGitHubReleases()
        {
            // Set the URL for the request
            // http request GET /repos/owner/repo/releases

            string sURL = "https://api.github.com/repos/" + GitHubUserName + "/" + RepositoryName + "/releases";

            JArray jData = null;

            try
            {
                // Create a new WebClient object to handle the HTTP request.
                using (WebClient webClient = new WebClient())
                {
                    // Add a user-agent header.
                    webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

                    // Define a stream object and assign the webClient's OpenRead method.
                    using (Stream stm = webClient.OpenRead(sURL))
                    {
                        // Create a StreamReader from the stream.
                        using (StreamReader sr = new StreamReader(stm))
                        {
                            // Process the stream with the JSON Text Reader in to a JArray.
                            using (JsonTextReader jtr = new JsonTextReader(sr))
                            {
                                jData = (JArray)JToken.ReadFrom(jtr);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Some error handling to be implemented here
                MessageBox.Show("Exception caught during StreamReader or JsonTextReader while processing FindAllGitHubReleases" + Environment.NewLine + e.ToString());
            }
            // return the JArray
            return jData;
        }
    }

    /*
    /// <summary>
    /// Defines a class to hold custom event info
    /// </summary>    
    public class HEJsonBaseFileEventArgs : EventArgs
    {
        public HEJsonBaseFileEventArgs(string s)
        {
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
    */

}
