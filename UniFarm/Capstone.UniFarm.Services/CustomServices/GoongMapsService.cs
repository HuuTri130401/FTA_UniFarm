using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class GoongMapsService : IGoongMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _goongApiKey;
        private readonly ILogger<GoongMapsService> _logger;

        public GoongMapsService(HttpClient httpClient, IConfiguration configuration, ILogger<GoongMapsService> logger)
        {
            _httpClient = httpClient;
            _goongApiKey = configuration["GoongMaps:ApiKey"];
            _logger = logger;
        }

        public async Task<string> GetAddressFromLatLong(double latitude, double longitude)
        {
            try
            {
                var url = $"https://rsapi.goong.io/Geocode?latlng={latitude},{longitude}&api_key={_goongApiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAddressFromLatLong Service!");
                throw;
            }
        }

        public async Task<GoongMapResponse> GetDistanceAndDuration(string origins, string destinations)
        {
            try
            {
                var goongMap = new GoongMapResponse();
                var url = $"https://rsapi.goong.io/DistanceMatrix?origins={origins}&destinations={destinations}&vehicle=car&api_key={_goongApiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using (var document = JsonDocument.Parse(json))
                {
                    var rows = document.RootElement.GetProperty("rows").EnumerateArray().FirstOrDefault();

                    var elements = rows.GetProperty("elements").EnumerateArray().FirstOrDefault();

                    var distanceText = elements.GetProperty("distance").GetProperty("text").GetString();
                    var durationText = elements.GetProperty("duration").GetProperty("text").GetString();

                    goongMap.DurationText = durationText;
                    goongMap.DistanceText = distanceText;
                }
                return goongMap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetDistanceAndDuration Service!");
                throw;
            }
        }
    }
}
