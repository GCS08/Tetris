using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class RandomUsername : RandomUsernameModel
    {
        // Static method to get a random username
        public override async Task<string> GetAsync()
        {
            using (client)
            {
                try
                {
                    // Fix: Use RandomUsernameModel.apiUrl if apiUrl is static, otherwise make it static
                    response = await client.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode)
                        return Strings.FailedRandomApiUN; // fallback
                    
                    json = await response.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(json);
                    return doc.RootElement
                              .GetProperty(TechnicalConsts.ResultsJson)[0]
                              .GetProperty(TechnicalConsts.LoginJson)
                              .GetProperty(TechnicalConsts.UsernameJson)
                              .GetString()!;
                }
                catch
                {
                    return Strings.FailedRandomApiUN; // fallback on exception
                }
            }
        }
    }
}
