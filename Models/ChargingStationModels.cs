using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SparkPoint_Server.Constants;
using SparkPoint_Server.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SparkPoint_Server.Models
{
    public class ChargingStation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("stationName")]
        public string StationName { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("stateProvince")]
        public string StateProvince { get; set; }

        [BsonElement("totalSlots")]
        public int TotalSlots { get; set; }

        [BsonElement("availableSlots")]
        public int AvailableSlots { get; set; }

        [BsonElement("contactPhone")]
        public string ContactPhone { get; set; }

        [BsonElement("contactEmail")]
        public string ContactEmail { get; set; }

        [BsonElement("latitude")]
        public decimal Latitude { get; set; }

        [BsonElement("longitude")]
        public decimal Longitude { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Optional: Combined location field for backward compatibility
        [BsonElement("location")]
        public string Location { get; set; }
    }


    public class StationCreateModel
    {
        [Required(ErrorMessage = "Station name is required")]
        [StringLength(100, ErrorMessage = "Station name cannot exceed 100 characters")]
        public string StationName { get; set; }

        [Required(ErrorMessage = "Station type is required")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "State/Province is required")]
        [StringLength(100, ErrorMessage = "State/Province cannot exceed 100 characters")]
        public string StateProvince { get; set; }

        [Required(ErrorMessage = "Total slots is required")]
        [Range(1, 100, ErrorMessage = "Total slots must be between 1 and 100")]
        public int TotalSlots { get; set; }

        [Required(ErrorMessage = "Contact phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }
    }

    public class StationUpdateModel
    {
        [StringLength(100, ErrorMessage = "Station name cannot exceed 100 characters")]
        public string StationName { get; set; }

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [StringLength(100, ErrorMessage = "State/Province cannot exceed 100 characters")]
        public string StateProvince { get; set; }

        [Range(1, 100, ErrorMessage = "Total slots must be between 1 and 100")]
        public int? TotalSlots { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string ContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string ContactEmail { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }
    }

    public class StationFilterModel
    {
        public bool? IsActive { get; set; }

        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string SearchTerm { get; set; }
    }

    public class StationQueryModel : StationFilterModel
    {
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "Descending";
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class StationOperationResult
    {
        public StationOperationStatus Status { get; private set; }
        public bool IsSuccess => Status == StationOperationStatus.Success;
        public string Message { get; private set; }
        public object Data { get; private set; }

        private StationOperationResult() { }

        public static StationOperationResult Success(string message, object data = null)
        {
            return new StationOperationResult
            {
                Status = StationOperationStatus.Success,
                Message = message,
                Data = data
            };
        }

        public static StationOperationResult Failed(StationOperationStatus status, string customMessage = null)
        {
            var message = customMessage ?? GetDefaultErrorMessage(status);
            return new StationOperationResult
            {
                Status = status,
                Message = message
            };
        }

        private static string GetDefaultErrorMessage(StationOperationStatus status)
        {
            switch (status)
            {
                case StationOperationStatus.StationNotFound:
                    return ChargingStationConstants.StationNotFound;
                case StationOperationStatus.AlreadyInState:
                    return "Station is already in the requested state";
                case StationOperationStatus.HasActiveBookings:
                    return ChargingStationConstants.CannotDeactivateWithActiveBookings;
                case StationOperationStatus.ValidationFailed:
                    return "Validation failed";
                default:
                    return "Operation failed";
            }
        }
    }

    public class StationQueryResult
    {
        public bool IsSuccess { get; private set; }
        public List<ChargingStation> Stations { get; private set; }
        public string ErrorMessage { get; private set; }

        private StationQueryResult() { }

        public static StationQueryResult Success(List<ChargingStation> stations)
        {
            return new StationQueryResult
            {
                IsSuccess = true,
                Stations = stations
            };
        }

        public static StationQueryResult Failed(string errorMessage)
        {
            return new StationQueryResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    public class StationRetrievalResult
    {
        public bool IsSuccess { get; private set; }
        public ChargingStation Station { get; private set; }
        public List<object> StationUsers { get; private set; }
        public string ErrorMessage { get; private set; }

        private StationRetrievalResult() { }

        public static StationRetrievalResult Success(ChargingStation station, List<object> stationUsers = null)
        {
            return new StationRetrievalResult
            {
                IsSuccess = true,
                Station = station,
                StationUsers = stationUsers
            };
        }

        public static StationRetrievalResult Failed(string errorMessage)
        {
            return new StationRetrievalResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    public class StationValidationResult
    {
        public bool IsValid { get; private set; }
        public List<StationValidationError> Errors { get; private set; }
        public string ErrorMessage { get; private set; }

        private StationValidationResult()
        {
            Errors = new List<StationValidationError>();
        }

        public static StationValidationResult Success()
        {
            return new StationValidationResult
            {
                IsValid = true
            };
        }

        public static StationValidationResult Failed(StationValidationError error, string message = null)
        {
            return new StationValidationResult
            {
                IsValid = false,
                Errors = new List<StationValidationError> { error },
                ErrorMessage = message ?? GetDefaultErrorMessage(error)
            };
        }

        public static StationValidationResult Failed(List<StationValidationError> errors, string message = null)
        {
            return new StationValidationResult
            {
                IsValid = false,
                Errors = errors,
                ErrorMessage = message ?? (errors != null && errors.Count == 1 ? GetDefaultErrorMessage(errors[0]) : "Multiple validation errors occurred")
            };
        }

        private static string GetDefaultErrorMessage(StationValidationError error)
        {
            switch (error)
            {
                case StationValidationError.LocationRequired:
                    return ChargingStationConstants.LocationRequired;
                case StationValidationError.LocationTooLong:
                    return $"Location cannot exceed {ChargingStationConstants.MaxLocationLength} characters";
                case StationValidationError.TypeRequired:
                    return ChargingStationConstants.TypeRequired;
                case StationValidationError.InvalidType:
                    return "Invalid station type";
                case StationValidationError.TotalSlotsMustBePositive:
                    return ChargingStationConstants.TotalSlotsMustBePositive;
                case StationValidationError.TotalSlotsExceedsMaximum:
                    return $"Total slots cannot exceed {ChargingStationConstants.MaxTotalSlots}";
                case StationValidationError.StationNameRequired:
                    return ChargingStationConstants.StationNameRequired;
                case StationValidationError.StationNameTooLong:
                    return $"Station name cannot exceed {ChargingStationConstants.MaxStationNameLength} characters";
                case StationValidationError.AddressRequired:
                    return ChargingStationConstants.AddressRequired;
                case StationValidationError.AddressTooLong:
                    return $"Address cannot exceed {ChargingStationConstants.MaxAddressLength} characters";
                case StationValidationError.CityRequired:
                    return ChargingStationConstants.CityRequired;
                case StationValidationError.CityTooLong:
                    return $"City cannot exceed {ChargingStationConstants.MaxCityLength} characters";
                case StationValidationError.StateProvinceRequired:
                    return ChargingStationConstants.StateProvinceRequired;
                case StationValidationError.StateProvinceTooLong:
                    return $"State/Province cannot exceed {ChargingStationConstants.MaxStateProvinceLength} characters";
                case StationValidationError.ContactPhoneRequired:
                    return ChargingStationConstants.ContactPhoneRequired;
                case StationValidationError.ContactPhoneInvalid:
                    return "Invalid phone number format";
                case StationValidationError.ContactEmailRequired:
                    return ChargingStationConstants.ContactEmailRequired;
                case StationValidationError.ContactEmailInvalid:
                    return "Invalid email format";
                case StationValidationError.LatitudeRequired:
                    return ChargingStationConstants.LatitudeRequired;
                case StationValidationError.LatitudeInvalidRange:
                    return $"Latitude must be between {ChargingStationConstants.MinLatitude} and {ChargingStationConstants.MaxLatitude}";
                case StationValidationError.LongitudeRequired:
                    return ChargingStationConstants.LongitudeRequired;
                case StationValidationError.LongitudeInvalidRange:
                    return $"Longitude must be between {ChargingStationConstants.MinLongitude} and {ChargingStationConstants.MaxLongitude}";
                default:
                    return "Validation error";
            }
        }
    }
}