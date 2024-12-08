using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using mcms.Models;

namespace mcms.ApiServices
{
    public interface IGoogleMapsApi
    {
        Task<GooglePlace> GetPlaceDetails(string placeId);
    }
}
