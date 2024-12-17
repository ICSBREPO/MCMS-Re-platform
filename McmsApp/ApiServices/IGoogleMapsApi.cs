using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using McmsApp.Models;

namespace McmsApp.ApiServices
{
    public interface IGoogleMapsApi
    {
        Task<GooglePlace> GetPlaceDetails(string placeId);
    }
}
