using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using SparkPoint_Server.Models;
using SparkPoint_Server.Constants;
using SparkPoint_Server.Enums;

namespace SparkPoint_Server.Helpers
{
    public static class ChargingStationFilterHelper
    {
        public static FilterDefinition<ChargingStation> BuildStationFilter(StationFilterModel filter)
        {
            var filterBuilder = Builders<ChargingStation>.Filter.Empty;

            if (filter == null)
                return filterBuilder;

            if (filter.IsActive.HasValue)
            {
                var activeFilter = Builders<ChargingStation>.Filter.Eq(s => s.IsActive, filter.IsActive.Value);
                filterBuilder = Builders<ChargingStation>.Filter.And(filterBuilder, activeFilter);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchFilter = BuildSearchFilter(filter.SearchTerm);
                filterBuilder = Builders<ChargingStation>.Filter.And(filterBuilder, searchFilter);
            }

            return filterBuilder;
        }
        private static FilterDefinition<ChargingStation> BuildSearchFilter(string searchTerm)
        {
            // Search in station name
            var stationNameFilter = Builders<ChargingStation>.Filter.Regex(
                s => s.StationName, 
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")
            );

            // Search in type
            var typeFilter = Builders<ChargingStation>.Filter.Regex(
                s => s.Type, 
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")
            );

            // Search in address
            var addressFilter = Builders<ChargingStation>.Filter.Regex(
                s => s.Address, 
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")
            );

            // Search in city
            var cityFilter = Builders<ChargingStation>.Filter.Regex(
                s => s.City, 
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")
            );

            // Search in state/province
            var stateProvinceFilter = Builders<ChargingStation>.Filter.Regex(
                s => s.StateProvince, 
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")
            );

            // Search in combined location field for backward compatibility
            var locationFilter = Builders<ChargingStation>.Filter.Regex(
                s => s.Location, 
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")
            );

            return Builders<ChargingStation>.Filter.Or(
                stationNameFilter, 
                typeFilter, 
                addressFilter, 
                cityFilter, 
                stateProvinceFilter, 
                locationFilter
            );
        }

        public static SortDefinition<ChargingStation> BuildStationSort(StationSortField sortField, SortOrder sortOrder)
        {
            var sortBuilder = Builders<ChargingStation>.Sort;

            SortDefinition<ChargingStation> sortDefinition;

            switch (sortField)
            {
                case StationSortField.Location:
                    sortDefinition = sortOrder == SortOrder.Ascending 
                        ? sortBuilder.Ascending(s => s.Location)
                        : sortBuilder.Descending(s => s.Location);
                    break;
                case StationSortField.Type:
                    sortDefinition = sortOrder == SortOrder.Ascending
                        ? sortBuilder.Ascending(s => s.Type)
                        : sortBuilder.Descending(s => s.Type);
                    break;
                case StationSortField.TotalSlots:
                    sortDefinition = sortOrder == SortOrder.Ascending
                        ? sortBuilder.Ascending(s => s.TotalSlots)
                        : sortBuilder.Descending(s => s.TotalSlots);
                    break;
                case StationSortField.AvailableSlots:
                    sortDefinition = sortOrder == SortOrder.Ascending
                        ? sortBuilder.Ascending(s => s.AvailableSlots)
                        : sortBuilder.Descending(s => s.AvailableSlots);
                    break;
                case StationSortField.IsActive:
                    sortDefinition = sortOrder == SortOrder.Ascending
                        ? sortBuilder.Ascending(s => s.IsActive)
                        : sortBuilder.Descending(s => s.IsActive);
                    break;
                case StationSortField.CreatedAt:
                    sortDefinition = sortOrder == SortOrder.Ascending
                        ? sortBuilder.Ascending(s => s.CreatedAt)
                        : sortBuilder.Descending(s => s.CreatedAt);
                    break;
                case StationSortField.UpdatedAt:
                    sortDefinition = sortOrder == SortOrder.Ascending
                        ? sortBuilder.Ascending(s => s.UpdatedAt)
                        : sortBuilder.Descending(s => s.UpdatedAt);
                    break;
                default:
                    sortDefinition = sortBuilder.Descending(s => s.CreatedAt);
                    break;
            }

            return sortDefinition;
        }

        public static FilterDefinition<Booking> BuildActiveBookingsFilter(string stationId)
        {
            return Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.StationId, stationId),
                Builders<Booking>.Filter.Not(
                    Builders<Booking>.Filter.In(b => b.Status, new[] { "Cancelled", "Completed" })
                )
            );
        }

        public static FilterDefinition<User> BuildStationUsersFilter(string stationId)
        {
            return Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.ChargingStationId, stationId),
                Builders<User>.Filter.Eq(u => u.RoleId, ApplicationConstants.StationUserRoleId)
            );
        }
    }

    public static class ChargingStationUpdateHelper
    {
        public static UpdateDefinition<ChargingStation> BuildStationUpdate(StationUpdateModel model, ChargingStation currentStation)
        {
            var updateBuilder = Builders<ChargingStation>.Update.Set(s => s.UpdatedAt, System.DateTime.UtcNow);

            // Update Station Name
            if (!string.IsNullOrEmpty(model.StationName))
            {
                var sanitizedStationName = Utils.ChargingStationUtils.SanitizeStationName(model.StationName);
                updateBuilder = updateBuilder.Set(s => s.StationName, sanitizedStationName);
            }

            // Update Type
            if (!string.IsNullOrEmpty(model.Type))
            {
                var sanitizedType = Utils.ChargingStationUtils.SanitizeType(model.Type);
                updateBuilder = updateBuilder.Set(s => s.Type, sanitizedType);
            }

            // Update Address
            if (!string.IsNullOrEmpty(model.Address))
            {
                var sanitizedAddress = Utils.ChargingStationUtils.SanitizeAddress(model.Address);
                updateBuilder = updateBuilder.Set(s => s.Address, sanitizedAddress);
            }

            // Update City
            if (!string.IsNullOrEmpty(model.City))
            {
                var sanitizedCity = Utils.ChargingStationUtils.SanitizeCity(model.City);
                updateBuilder = updateBuilder.Set(s => s.City, sanitizedCity);
            }

            // Update State/Province
            if (!string.IsNullOrEmpty(model.StateProvince))
            {
                var sanitizedStateProvince = Utils.ChargingStationUtils.SanitizeStateProvince(model.StateProvince);
                updateBuilder = updateBuilder.Set(s => s.StateProvince, sanitizedStateProvince);
            }

            // Update Contact Phone
            if (!string.IsNullOrEmpty(model.ContactPhone))
            {
                var sanitizedPhone = Utils.ChargingStationUtils.SanitizeContactPhone(model.ContactPhone);
                updateBuilder = updateBuilder.Set(s => s.ContactPhone, sanitizedPhone);
            }

            // Update Contact Email
            if (!string.IsNullOrEmpty(model.ContactEmail))
            {
                var sanitizedEmail = Utils.ChargingStationUtils.SanitizeContactEmail(model.ContactEmail);
                updateBuilder = updateBuilder.Set(s => s.ContactEmail, sanitizedEmail);
            }

            // Update Latitude
            if (model.Latitude.HasValue)
            {
                updateBuilder = updateBuilder.Set(s => s.Latitude, model.Latitude.Value);
            }

            // Update Longitude
            if (model.Longitude.HasValue)
            {
                updateBuilder = updateBuilder.Set(s => s.Longitude, model.Longitude.Value);
            }

            // Update Total Slots and recalculate available slots
            if (model.TotalSlots.HasValue && model.TotalSlots.Value > 0)
            {
                var newAvailable = Utils.ChargingStationUtils.CalculateNewAvailableSlots(
                    currentStation.AvailableSlots,
                    currentStation.TotalSlots,
                    model.TotalSlots.Value
                );

                updateBuilder = updateBuilder.Set(s => s.TotalSlots, model.TotalSlots.Value);
                updateBuilder = updateBuilder.Set(s => s.AvailableSlots, newAvailable);
            }

            // Update combined location field for backward compatibility if address components changed
            if (!string.IsNullOrEmpty(model.Address) || !string.IsNullOrEmpty(model.City) || !string.IsNullOrEmpty(model.StateProvince))
            {
                var address = !string.IsNullOrEmpty(model.Address) ? Utils.ChargingStationUtils.SanitizeAddress(model.Address) : currentStation.Address;
                var city = !string.IsNullOrEmpty(model.City) ? Utils.ChargingStationUtils.SanitizeCity(model.City) : currentStation.City;
                var stateProvince = !string.IsNullOrEmpty(model.StateProvince) ? Utils.ChargingStationUtils.SanitizeStateProvince(model.StateProvince) : currentStation.StateProvince;
                
                var combinedLocation = $"{address}, {city}, {stateProvince}";
                updateBuilder = updateBuilder.Set(s => s.Location, combinedLocation);
            }

            return updateBuilder;
        }

        public static UpdateDefinition<ChargingStation> BuildActivationUpdate()
        {
            return Builders<ChargingStation>.Update
                .Set(s => s.IsActive, true)
                .Set(s => s.UpdatedAt, System.DateTime.UtcNow);
        }

        public static UpdateDefinition<ChargingStation> BuildDeactivationUpdate()
        {
            return Builders<ChargingStation>.Update
                .Set(s => s.IsActive, false)
                .Set(s => s.UpdatedAt, System.DateTime.UtcNow);
        }
    }
}