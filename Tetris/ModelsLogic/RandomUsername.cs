using System.Text.Json;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Provides functionality to fetch a random username from an external API.
    /// </summary>
    public class RandomUsername : RandomUsernameModel
    {
        #region Public Methods

        /// <summary>
        /// Asynchronously retrieves a random username from the API.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{string}"/> containing the username retrieved from the API.
        /// Returns a fallback string <see cref="Strings.FailedRandomApiUN"/> if the request fails,
        /// the response is invalid, or an exception occurs.
        /// </returns>
        /// <remarks>
        /// - Uses <see cref="HttpClient"/> (assumed to be <c>client</c> in the base class) to perform the request.
        /// - Parses the JSON response to extract the username.
        /// - Handles network errors, non-success HTTP codes, and parsing exceptions gracefully.
        /// </remarks>
        public override async Task<string> GetAsync()
        {
            using (client)
            {
                try
                {
                    // Attempt to get the response from the API
                    response = await client.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode)
                        return Strings.FailedRandomApiUN; // fallback if HTTP request failed

                    json = await response.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(json);

                    // Navigate the JSON to extract the username
                    return doc.RootElement
                              .GetProperty(TechnicalConsts.ResultsJson)[0]
                              .GetProperty(TechnicalConsts.LoginJson)
                              .GetProperty(TechnicalConsts.UsernameJson)
                              .GetString() ?? Strings.FailedRandomApiUN; // fallback if null
                }
                catch
                {
                    return Strings.FailedRandomApiUN; // fallback on exception
                }
            }
        }

        #endregion
    }
}