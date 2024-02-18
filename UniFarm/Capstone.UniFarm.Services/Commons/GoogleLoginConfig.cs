using System.Text.Json;

namespace Capstone.UniFarm.Services.Commons
{
    public class GoogleLoginConfig
    {
        private readonly HttpClient _httpClient;

        public GoogleLoginConfig(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GoogleUserInfo> VerifyGoogleAccessTokenAsync(string googleAccessToken)
        {
            // Make a request to Google API to verify the access token
            var response = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={googleAccessToken}");

            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                // Deserialize the response content to extract user information
                var googleUserInfo = await JsonSerializer.DeserializeAsync<GoogleUserInfo>(contentStream);
                return googleUserInfo;
            }

            return null; // Return null if verification fails
        }

        public class GoogleUserInfo
        {
            public string Email { get; set; }
            // Add other properties as needed
        }
    }
}
