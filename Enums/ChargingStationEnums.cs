namespace SparkPoint_Server.Enums
{
    public enum StationOperationStatus
    {
        Success,
        StationNotFound,
        AlreadyInState,
        HasActiveBookings,
        ValidationFailed,
        Failed
    }

    public enum StationFilterType
    {
        ActiveStatus,
        Location,
        Type,
        SearchTerm
    }

    public enum StationSortField
    {
        Location,
        Type,
        TotalSlots,
        AvailableSlots,
        IsActive,
        CreatedAt,
        UpdatedAt
    }

    public enum StationType
    {
        AC,  // AC charging
        DC   // DC charging
    }

    public enum StationValidationError
    {
        None,
        // Legacy validation errors (kept for backward compatibility)
        LocationRequired,
        LocationTooLong,
        TypeRequired,
        InvalidType,
        TotalSlotsMustBePositive,
        TotalSlotsExceedsMaximum,
        // New validation errors for frontend properties
        StationNameRequired,
        StationNameTooLong,
        AddressRequired,
        AddressTooLong,
        CityRequired,
        CityTooLong,
        StateProvinceRequired,
        StateProvinceTooLong,
        ContactPhoneRequired,
        ContactPhoneInvalid,
        ContactEmailRequired,
        ContactEmailInvalid,
        LatitudeRequired,
        LatitudeInvalidRange,
        LongitudeRequired,
        LongitudeInvalidRange
    }
}