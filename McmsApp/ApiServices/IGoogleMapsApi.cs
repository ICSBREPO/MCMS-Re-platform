using McmsApp.Models;

namespace McmsApp.ApiServices
{
    public interface IGoogleMapsApi
    {
        Task<GooglePlace> GetPlaceDetails(string placeId);
    }
}
