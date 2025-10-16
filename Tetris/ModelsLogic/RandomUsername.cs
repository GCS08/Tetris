using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tetris.ModelsLogic
{
    public class RandomUsername
    {
        // Static method to get a random username
        public static async Task<string> GetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://randomuser.me/api/?results=1";
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        return "UnknownUser"; // fallback

                    string json = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        return doc.RootElement
                                  .GetProperty("results")[0]
                                  .GetProperty("login")
                                  .GetProperty("username")
                                  .GetString()!;
                    }
                }
                catch
                {
                    return "UnknownUser"; // fallback on exception
                }
            }
        }
    }
}
