using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SparkPoint_Server.Constants;
using SparkPoint_Server.Enums;
using SparkPoint_Server.Models;

namespace SparkPoint_Server.Utils
{

    public static class ChargingStationUtils
    {
        public static StationValidationResult ValidateCreateModel(StationCreateModel model)
        {
            if (model == null)
                return StationValidationResult.Failed(StationValidationError.None, ChargingStationConstants.StationDataRequired);

            var errors = new List<StationValidationError>();

            // Validate Type first: if provided but invalid, return that error immediately
            if (!string.IsNullOrEmpty(model.Type) && !IsValidStationType(model.Type))
                return StationValidationResult.Failed(StationValidationError.InvalidType);

            // Validate Station Name
            if (string.IsNullOrEmpty(model.StationName))
                errors.Add(StationValidationError.StationNameRequired);
            else if (model.StationName.Length > ChargingStationConstants.MaxStationNameLength)
                errors.Add(StationValidationError.StationNameTooLong);

            // Validate Type (required)
            if (string.IsNullOrEmpty(model.Type))
                errors.Add(StationValidationError.TypeRequired);

            // Validate Address
            if (string.IsNullOrEmpty(model.Address))
                errors.Add(StationValidationError.AddressRequired);
            else if (model.Address.Length > ChargingStationConstants.MaxAddressLength)
                errors.Add(StationValidationError.AddressTooLong);

            // Validate City
            if (string.IsNullOrEmpty(model.City))
                errors.Add(StationValidationError.CityRequired);
            else if (model.City.Length > ChargingStationConstants.MaxCityLength)
                errors.Add(StationValidationError.CityTooLong);

            // Validate State/Province
            if (string.IsNullOrEmpty(model.StateProvince))
                errors.Add(StationValidationError.StateProvinceRequired);
            else if (model.StateProvince.Length > ChargingStationConstants.MaxStateProvinceLength)
                errors.Add(StationValidationError.StateProvinceTooLong);

            // Validate Total Slots
            if (model.TotalSlots <= 0)
                errors.Add(StationValidationError.TotalSlotsMustBePositive);
            else if (model.TotalSlots > ChargingStationConstants.MaxTotalSlots)
                errors.Add(StationValidationError.TotalSlotsExceedsMaximum);

            // Validate Contact Phone
            if (string.IsNullOrEmpty(model.ContactPhone))
                errors.Add(StationValidationError.ContactPhoneRequired);
            else if (!IsValidPhone(model.ContactPhone))
                errors.Add(StationValidationError.ContactPhoneInvalid);

            // Validate Contact Email
            if (string.IsNullOrEmpty(model.ContactEmail))
                errors.Add(StationValidationError.ContactEmailRequired);
            else if (!IsValidEmail(model.ContactEmail))
                errors.Add(StationValidationError.ContactEmailInvalid);

            // Validate Latitude
            if (model.Latitude < ChargingStationConstants.MinLatitude || model.Latitude > ChargingStationConstants.MaxLatitude)
                errors.Add(StationValidationError.LatitudeInvalidRange);

            // Validate Longitude
            if (model.Longitude < ChargingStationConstants.MinLongitude || model.Longitude > ChargingStationConstants.MaxLongitude)
                errors.Add(StationValidationError.LongitudeInvalidRange);

            return errors.Any() ? StationValidationResult.Failed(errors) : StationValidationResult.Success();
        }

        public static StationValidationResult ValidateUpdateModel(StationUpdateModel model)
        {
            if (model == null)
                return StationValidationResult.Failed(StationValidationError.None, ChargingStationConstants.UpdateDataRequired);

            var errors = new List<StationValidationError>();

            // Validate Type first: if provided but invalid, return that error immediately
            if (!string.IsNullOrEmpty(model.Type) && !IsValidStationType(model.Type))
                return StationValidationResult.Failed(StationValidationError.InvalidType);

            // Validate Station Name (if provided)
            if (!string.IsNullOrEmpty(model.StationName))
            {
                if (model.StationName.Length > ChargingStationConstants.MaxStationNameLength)
                    errors.Add(StationValidationError.StationNameTooLong);
            }

            // Validate Type (if provided)
            // (Already validated above)

            // Validate Address (if provided)
            if (!string.IsNullOrEmpty(model.Address))
            {
                if (model.Address.Length > ChargingStationConstants.MaxAddressLength)
                    errors.Add(StationValidationError.AddressTooLong);
            }

            // Validate City (if provided)
            if (!string.IsNullOrEmpty(model.City))
            {
                if (model.City.Length > ChargingStationConstants.MaxCityLength)
                    errors.Add(StationValidationError.CityTooLong);
            }

            // Validate State/Province (if provided)
            if (!string.IsNullOrEmpty(model.StateProvince))
            {
                if (model.StateProvince.Length > ChargingStationConstants.MaxStateProvinceLength)
                    errors.Add(StationValidationError.StateProvinceTooLong);
            }

            // Validate Total Slots (if provided)
            if (model.TotalSlots.HasValue)
            {
                if (model.TotalSlots.Value <= 0)
                    errors.Add(StationValidationError.TotalSlotsMustBePositive);
                else if (model.TotalSlots.Value > ChargingStationConstants.MaxTotalSlots)
                    errors.Add(StationValidationError.TotalSlotsExceedsMaximum);
            }

            // Validate Contact Phone (if provided)
            if (!string.IsNullOrEmpty(model.ContactPhone))
            {
                if (!IsValidPhone(model.ContactPhone))
                    errors.Add(StationValidationError.ContactPhoneInvalid);
            }

            // Validate Contact Email (if provided)
            if (!string.IsNullOrEmpty(model.ContactEmail))
            {
                if (!IsValidEmail(model.ContactEmail))
                    errors.Add(StationValidationError.ContactEmailInvalid);
            }

            // Validate Latitude (if provided)
            if (model.Latitude.HasValue)
            {
                if (model.Latitude.Value < ChargingStationConstants.MinLatitude || model.Latitude.Value > ChargingStationConstants.MaxLatitude)
                    errors.Add(StationValidationError.LatitudeInvalidRange);
            }

            // Validate Longitude (if provided)
            if (model.Longitude.HasValue)
            {
                if (model.Longitude.Value < ChargingStationConstants.MinLongitude || model.Longitude.Value > ChargingStationConstants.MaxLongitude)
                    errors.Add(StationValidationError.LongitudeInvalidRange);
            }

            return errors.Any() ? StationValidationResult.Failed(errors) : StationValidationResult.Success();
        }

        public static bool IsValidStationType(string type)
        {
            return ChargingStationConstants.ValidStationTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use a regex pattern for basic email validation
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, emailPattern);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Remove all non-digit characters for validation
            var digitsOnly = Regex.Replace(phone, @"\D", "");
            
            // Phone number should have at least 10 digits and at most 15 digits
            return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
        }

        public static int CalculateNewAvailableSlots(int currentAvailable, int currentTotal, int newTotal)
        {
            var usedSlots = currentTotal - currentAvailable;
            var newAvailable = newTotal - Math.Min(usedSlots, newTotal);
            return Math.Max(0, Math.Min(newTotal, newAvailable));
        }

        public static StationType GetStationTypeEnum(string type)
        {
            switch (type?.ToUpper())
            {
                case "AC":
                    return StationType.AC;
                case "DC":
                    return StationType.DC;
                default:
                    return StationType.AC; // Default to AC
            }
        }
        
        public static string GetStationTypeString(StationType type)
        {
            switch (type)
            {
                case StationType.AC:
                    return "AC";
                case StationType.DC:
                    return "DC";
                default:
                    return "AC"; // Default to AC
            }
        }

        // Updated sanitization methods for new properties
        public static string SanitizeStationName(string stationName)
        {
            if (string.IsNullOrWhiteSpace(stationName))
                return null;

            return stationName.Trim();
        }

        public static string SanitizeAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            return address.Trim();
        }

        public static string SanitizeCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return null;

            return city.Trim();
        }

        public static string SanitizeStateProvince(string stateProvince)
        {
            if (string.IsNullOrWhiteSpace(stateProvince))
                return null;

            return stateProvince.Trim();
        }

        public static string SanitizeContactPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            return phone.Trim();
        }

        public static string SanitizeContactEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return email.Trim().ToLower();
        }

        // Legacy methods for backward compatibility
        public static string SanitizeLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return null;

            return location.Trim();
        }

        public static string SanitizeType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            return type.Trim();
        }

        public static object CreateStationUserProfile(User user)
        {
            return new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.RoleId,
                RoleName = AuthUtils.GetRoleName(user.RoleId),
                user.IsActive,
                user.CreatedAt,
                user.UpdatedAt
            };
        }

        public static object CreateDetailedStationResponse(ChargingStation station, List<User> stationUsers)
        {
            var userProfiles = stationUsers?.Select(CreateStationUserProfile).ToList() ?? new List<object>();
            
            return new
            {
                Station = station,
                StationUsers = userProfiles
            };
        }

        public static string[] GetValidStationTypes()
        {
            return ChargingStationConstants.ValidStationTypes;
        }
    }
}