namespace FalconDataCartridge.Enums
{
    /// <summary>
    /// List of Available Steerpoint Actions
    /// </summary>
    public enum STPTAirAction
    {
        Precision = -1,
        Nav,
        Takeoff, // Altitude needs to be Airbase Elevation
        Push,
        Split,
        Refuel,
        Rearm,
        Pickup,
        Land, // Altitude needs to be Airbase Elevation
        Hold,
        Contact,
        Escort,
        Sweep,
        CAP,
        Intercept,
        GrndAttack,
        SurfaceAttack,
        SearchAndDestroy,
        Strike,
        Bomb,
        SEAD,
        ELINT,
        Recon,
        Rescue,
        ASW,
        Fuel,
        Airdrop,
        Jamming
    }

    /// <summary>
    /// Enum to select type of Altitude Change enroute to next steerpoint
    /// </summary>
    public enum AltitudeChangeType
    {
        Immediate,
        Delayed
    }
}
