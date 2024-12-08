using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using mcms.Models;
using Newtonsoft.Json.Linq;

namespace mcms.ApiServices
{
    public class GoogleMapsApi : IGoogleMapsApi
    {

        static string _googleMapsKey;

        private const string ApiBaseAddress = "https://maps.googleapis.com/maps/";
        private HttpClient CreateClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseAddress)
            };
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
        public static void Initialize(string googleMapsKey)
        {
            _googleMapsKey = googleMapsKey;
        }

        public async Task<GooglePlace> GetPlaceDetails(string placeId)
        {
            GooglePlace result = null;
            using (var httpClient = CreateClient())
            {
                var response = await httpClient.GetAsync($"api/place/details/json?placeid={Uri.EscapeUriString(placeId)}&key={_googleMapsKey}").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(json) && json != "ERROR")
                    {
                        result = new GooglePlace(JObject.Parse(json));
                    }
                }
            }

            return result;
        }
    }
}
