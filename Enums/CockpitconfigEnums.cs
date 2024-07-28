using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalconDataCartridge.Enums
{
    /// <summary>
    /// Available Settings for the HUD Scales Configuration
    /// </summary>
    public enum HUDScales
    {
        OFF,
        VAH,
        VAHVV
    }

    /// <summary>
    /// Available Settings for the HUD FPM Configuration
    /// </summary>
    public enum HUDFPM
    {
        OFF,
        FPM,
        ATT
    }

    /// <summary>
    /// Available Settings for the HUD Velocity Type Configuration
    /// </summary>
    public enum HUDVelocityType
    {
        GND,
        CAS,
        TAS
    }

    /// <summary>
    /// Available Settings for the HUD DED Configuration
    /// </summary>
    public enum HUDDED
    {
        OFF,
        PFL,
        DED
    }

    /// <summary>
    /// Available Settings for the HUD Altitude Configuration
    /// </summary>
    public enum HUDAltitude
    {
        Radar,
        Baro,
        Auto
    }

    /// <summary>
    /// Master Arm Switch Positions
    /// </summary>
    public enum MasterArmSetting
    {
        OFF,
        SIM,
        ARM
    }

    /// <summary>
    /// Available Radar Altimeter Settings
    /// </summary>
    public enum RALTSetting
    {
        OFF,
        STBY,
        ON
    }

    /// <summary>
    /// Available DED Brightness Settings
    /// </summary>
    public enum DEDBrightness
    {
        OFF,
        DIM,
        BRT
    }

    /// <summary>
    /// Selectable Views in the 3D World
    /// </summary>
    public enum OTWViewSetting
    {
        HUD = 1,
        Pit2D,
        Pit3D,
        Padlock,
        HUDOnly,
        Target,
        Incoming,
        Friendly,
        Chase,
        Orbit

    }
}
