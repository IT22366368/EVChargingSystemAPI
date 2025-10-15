namespace SparkPoint_Server.Constants
{
    public static class ChargingStationConstants
    {
        // Validation Messages
        public const string StationDataRequired = "Station data is required.";
        public const string LocationRequired = "Location is required.";
        public const string TypeRequired = "Type is required.";
        public const string TotalSlotsMustBePositive = "Total slots must be greater than 0.";
        public const string UpdateDataRequired = "Update data is required.";
        public const string StationNotFound = "Charging station not found.";
        public const string StationAlreadyActive = "Charging station is already active.";
        public const string StationAlreadyDeactivated = "Charging station is already deactivated.";
        public const string CannotDeactivateWithActiveBookings = "Cannot deactivate station. There are active bookings for this station.";
        
        // New validation messages for frontend properties
        public const string StationNameRequired = "Station name is required.";
        public const string AddressRequired = "Address is required.";
        public const string CityRequired = "City is required.";
        public const string StateProvinceRequired = "State/Province is required.";
        public const string ContactPhoneRequired = "Contact phone is required.";
        public const string ContactEmailRequired = "Contact email is required.";
        public const string LatitudeRequired = "Latitude is required.";
        public const string LongitudeRequired = "Longitude is required.";
        
        // Success Messages
        public const string StationCreatedSuccessfully = "Charging station created successfully.";
        public const string StationUpdatedSuccessfully = "Charging station updated successfully.";
        public const string StationActivatedSuccessfully = "Charging station activated successfully.";
        public const string StationDeactivatedSuccessfully = "Charging station deactivated successfully.";
        
        // Database Collection Names
        public const string ChargingStationsCollection = "ChargingStations";
        public const string BookingsCollection = "Bookings";
        
        // Validation Constants
        public const int MinTotalSlots = 1;
        public const int MaxTotalSlots = 100;
        public const int MaxLocationLength = 200;
        public const int MaxTypeLength = 50;
        public const int MaxStationNameLength = 100;
        public const int MaxAddressLength = 200;
        public const int MaxCityLength = 100;
        public const int MaxStateProvinceLength = 100;
        public const decimal MinLatitude = -90;
        public const decimal MaxLatitude = 90;
        public const decimal MinLongitude = -180;
        public const decimal MaxLongitude = 180;
        
        // Station Types - Both AC and DC charging supported
        public static readonly string[] ValidStationTypes = { "AC", "DC" };
        
        // Station Search Constants
        public const string LocationField = "location";
        public const string TypeField = "type";

        // Default Values
        public const bool DefaultIsActiveStatus = true;
        
    }
}