﻿using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface ILocationSpec
    {
        Task<List<LocationSpec>> GetLocationSpecAsync();
        Task<LocationSpec> GetLocationSpec(int id);
        Task AddLocationSpec(LocationSpec d);
        Task DeleteLocationSpec(LocationSpec d);
        Task UpdateLocationSpec(LocationSpec d);
        Task ResetLocationSpec();
        Task<List<LocationSpec>> GetLocationSpecByWO(int id);
        Task<int?> CountLocationSpec(int id);
    }
}
