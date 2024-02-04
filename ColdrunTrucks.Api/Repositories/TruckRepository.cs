using ColdrunTrucks.Api.Controllers;
using ColdrunTrucks.Api.Models;
using ColdrunTrucks.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ColdrunTrucks.Api.Services
{
    public interface ITruckRepository
    {
        Task<IEnumerable<Truck>> GetTrucksAsync(string? searchTerm, string? sortBy);

        Task AddAsync(Truck truck);

        Task UpdateAsync(Truck truck);

        Task UpdateStatusAsync(string truckCode, TruckStatus status);

        Task SetNextStatusAsync(string truckCode);

        Task DeleteTruckAsync(string truckCode);
    }

    public class TruckRepository : ITruckRepository
    {
        private readonly TrucksDbContext _dbContext;
        private readonly ILogger<TruckRepository> _logger;

        public TruckRepository(TrucksDbContext dbContext, ILogger<TruckRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Truck>> GetTrucksAsync(string? searchTerm, string? sortBy)
        {
            IQueryable<Truck> trucks;
            if (!string.IsNullOrEmpty(searchTerm))
                trucks = _dbContext.Trucks.Where(t => t.Code.Contains(searchTerm)
                                                      || t.Name.Contains(searchTerm)
                                                      || t.Description.Contains(searchTerm));
            else
                trucks = _dbContext.Trucks;

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.Contains("code"))
                    trucks = trucks.OrderBy(p => p.Code);
                if (sortBy.Contains("name"))
                    trucks = trucks.OrderBy(p => p.Name);
                if (sortBy.Contains("description"))
                    trucks = trucks.OrderBy(p => p.Description);
                if (sortBy.Contains("status"))
                    trucks = trucks.OrderBy(p => p.Status);
            }

            return trucks;
        }

        public async Task AddAsync(Truck truck)
        {
            await _dbContext.Trucks.AddAsync(truck);
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(Truck truck)
        {
            var entity = _dbContext.Trucks.Where(x => x.Code == truck.Code).Single();
            entity.Name = truck.Name;
            entity.Description = truck.Description;
            entity.SetTruckStatus(truck.Status);
            _dbContext.SaveChanges();
        }

        public async Task UpdateStatusAsync(string truckCode, TruckStatus status)
        {
            var entity = _dbContext.Trucks.Where(x => x.Code == truckCode).Single();
            entity.SetTruckStatus(status);
            _dbContext.SaveChanges();
        }

        public async Task SetNextStatusAsync(string truckCode)
        {
            var entity = _dbContext.Trucks.Where(x => x.Code == truckCode).Single();
            entity.SetNextStatus();
            _dbContext.SaveChanges();
        }

        public async Task DeleteTruckAsync(string truckCode)
        {

            await _dbContext.Trucks.Where(x => x.Code == truckCode).ExecuteDeleteAsync();
            _dbContext.SaveChanges();
        }
    }
}
