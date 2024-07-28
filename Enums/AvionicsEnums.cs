using Utilities.Attributes;

namespace FalconDataCartridge.Enums
{
    // MFD Settings

    /// <summary>
    /// Master Modes used in configuring the MFDs
    /// </summary>
    /// 
    public enum MFDMasterMode
    {
        [StringValue("A-A")]
        AA,
        [StringValue("A-G")]
        AG,
        [StringValue("NAV")]
        NAV,
        [StringValue("MSL OVRD")]
        MSL,
        [StringValue("DGFT OVRD")]
        DGFT,
        [StringValue("S-J")]
        SJ
    }

    /// <summary>
    /// Displays available within the DTC
    /// </summary>
    /// 
    public enum MFDDisplay
    {
        [StringValue("Display 0")]
        Display0,
        [StringValue("Display 1")]
        Display1,
        [StringValue("Display 2")]
        Display2,
        [StringValue("Display 3")]
        Display3
    }

    /// <summary>
    /// Button configuration options in the MFD DTC Section
    /// </summary>
    public enum MFDDisplayConfig
    {
        [StringValue("Left OSB")]
        Left,
        [StringValue("Center OSB")]
        Center,
        [StringValue("Right OSB")]
        Right,
        [StringValue("Default Selected View")]
        Selected
    }

    /// <summary>
    /// MFD Color Settings
    /// </summary>
    public enum MFDColorSetting : int
    {
        [StringValue("Default")]
        MFD_DEFAULT,
        [StringValue("SOI Box")]
        MFD_SOI_BOX,
        [StringValue("Aircraft Reference")]
        MFD_AIRCRAFT_REF,
        [StringValue("Bullseye")]
        MFD_BULLSEYE,
        [StringValue("Bullseye Data")]
        MFD_BULLSEYE_DATA,
        [StringValue("Steerpoint Cursor Data")]
        MFD_STEERPOINT_CURSOR_DATA,
        [StringValue("Reference Symbol")]
        MFD_REFERENCE_SYMBOL,
        [StringValue("Not SOI")]
        MFD_NOT_SOI,
        [StringValue("Target Closure Rate")]
        MFM_TGT_CLOSURE_RATE,
        [StringValue("Antenna Elevation Scale")]
        MFD_ANTENNA_AZEL_SCALE,
        [StringValue("Antenna Elevation")]
        MFD_ANTENNA_AZEL,
        [StringValue("FCR Re-Aqcuire Indicator")]
        MFD_FCR_REAQ_IND,
        [StringValue("FCR Range Ticks")]
        MFD_FCR_RANGE_TICKS,
        [StringValue("Cursor")]
        MFD_CURSOR,
        [StringValue("Cursor Scan Limit Negative")]
        MFD_CURSOR_SCAN_LIMIT_NEGATIVE,
        [StringValue("Min/Max Altitude")]
        MFD_MINMAX_ALT,
        [StringValue("Azimuth Scan Limit")]
        MFD_FCR_AZIMUTH_SCAN_LIM,
        [StringValue("Attack Steering Cue")]
        MFD_ATTACK_STEERING_CUE,
        [StringValue("Lines")]
        MFD_LINES,
        [StringValue("Custom Lines")]
        MFD_CUSTOM_LINES,
        [StringValue("Current Steerpoint")]
        MFD_CUR_STPT,
        [StringValue("Dynamic Launch Zone")]
        MFD_DLZ,
        [StringValue("Steer Error Cue")]
        MFD_STEER_ERROR_CUE,
        [StringValue("Unknown Indicator")]
        MFD_UNKNOWN,
        [StringValue("Exapand Box")]
        MFD_EXPAND_BOX,
        [StringValue("FCR Bug")]
        MFD_FCR_BUG,
        [StringValue("FCR Bugged Flashing Tail")]
        MFD_FCR_BUGGED_FLASH_TAIL,
        [StringValue("FCR Bugged Tail")]
        MFD_FCR_BUGGED_TAIL,
        [StringValue("FCR Bugged")]
        MFD_FCR_BUGGED,
        [StringValue("Kill 'X'")]
        MFD_KILL_X,
        [StringValue("FCR Unknown Track Flasshing")]
        MFD_FCR_UNK_TRACK_FLASH,
        [StringValue("FCR Unknown Track")]
        MFD_FCR_UNK_TRACK,
        [StringValue("Data Link Team 1-4")]
        MFD_DL_TEAM14,
        [StringValue("Data Link Team 5-8")]
        MFD_DL_TEAM58,
        [StringValue("LSDL Line")]
        MFD_LSDL_LINE,
        [StringValue("Airspeed Box")]
        MFD_AIRSPEED_BOX,
        [StringValue("Airspeed Heading Box")]
        MFD_AIRSPEED_HDG_BOX,
        [StringValue("Radar Altitude Box")]
        MFD_RADAR_ALT_BOX,
        [StringValue("Ownship")]
        MFD_OWNSHIP,
        [StringValue("Routes")]
        MFD_ROUTES,
        [StringValue("Data Link")]
        MFD_DATALINK,
        [StringValue("Mark Point")]
        MFD_MARKPOINT,
        [StringValue("Sweep")]
        MFD_SWEEP,
        [StringValue("DLP Scap")]
        MFD_DLP_SCAP,
        [StringValue("DLP Missile")]
        MFD_DLP_MISSILE,
        [StringValue("DLP Azimuth Line")]
        MFD_DLP_AZ_LINE,
        [StringValue("Pre-Plan In Range")]
        MFD_PREPLAN_INRANGE,
        [StringValue("Pre-Plan")]
        MFD_PREPLAN,
        [StringValue("Harpoon Path")]
        MFD_HARPOON_PATH,
        [StringValue("Harpoon Test")]
        MFD_HARPOON_TEST,
        [StringValue("HARM ALIC Box")]
        MFD_HARM_ALIC_BOX,
        [StringValue("HARM ALIC Box Range Lines")]
        MFD_HARM_ALIC_BOX_RANGE_LINES,
        [StringValue("HARM DTSB Box")]
        MFD_HARM_DTSB_BOX,
        [StringValue("HARM DTSB Symbol")]
        MFD_HARM_DTSB_SYMBOL,
        [StringValue("HARM HAD Symbol")]
        MFD_HARM_HAD_CURSOR,
        [StringValue("HARM HAD WEZ")]
        MFD_HARM_HAD_WEZ,
        [StringValue("HARM HAD Routes")]
        MFD_HARM_HAD_ROUTES,
        [StringValue("HARM HAD Lock")]
        MFD_HARM_HAD_LOCK,
        [StringValue("HARM HAD Emitter Behind 9 O'Clock")]
        MFD_HARM_HAD_EMITTER_BEHIND9,
        [StringValue("HARD HAD Emitter Behind 9 O'Clock Transmitting")]
        MFD_HARD_HAD_EMITTER_BEHIND9_TR,
        [StringValue("HARM HAD Emitter")]
        MFD_HARM_HAD_EMITTER,
        [StringValue("HARM HAD Emitter Launch")]
        MFD_HARM_HAD_EMITTER_LAUNCH,
        [StringValue("HARM HAD Emitter Track")]
        MFD_HARM_HAD_EMITTER_TRACK,
        [StringValue("HARM HAD Emitter Tadiate")]
        MFD_HARM_HAD_EMITTER_RADIATE,
        [StringValue("HARM HAD Emitter")]
        MFD_HARM_HAS_EMITTER,
        [StringValue("HARM Handoff Emitter")]
        MFD_HARM_HANDOFF_EMITTER,
        [StringValue("HARM Handoff")]
        MFD_HARM_HANDOFF,
        [StringValue("Pullup Cross")]
        MFD_PULLUP_CROSS,
        [StringValue("Check Altitude")]
        MFD_CHECKATTITUDE,
        [StringValue("Check Altitude Test")]
        MFD_CHECKATTITUDE_TEXT,
        [StringValue("TFR Limits")]
        MFD_TFRLIMITS,
        [StringValue("TFR Limits Text")]
        MFD_TFRLIMITS_TEXT,
        [StringValue("Sensors Video")]
        MFD_SENSORS_VIDEO,
        [StringValue("Cusom Line 1")]
        MFD_CUSTOM_LINE1,
        [StringValue("Custom Line 2")]
        MFD_CUSTOM_LINE2,
        [StringValue("Custom Line 3")]
        MFD_CUSTOM_LINE3,
        [StringValue("Custom Line 4")]
        MFD_CUSTOM_LINE4,
        [StringValue("Routes Steerpoint")]
        MFD_ROUTES_STPT
    }

    /// <summary>
    /// MFD Color Options
    /// </summary>
    public enum MFDColorOption
    {
        [StringValue("Default")]
        DEFAULT = -1,
        [StringValue("Green")]
        MFD_GREEN = 1,
        [StringValue("White")]
        MFD_WHITE,
        [StringValue("Red")]
        MFD_RED,
        [StringValue("Yellow")]
        MFD_YELLOW,
        [StringValue("Cyan")]
        MFD_CYAN,
        [StringValue("Magenta")]
        MFD_MAGENTA,
        [StringValue("Blue")]
        MFD_BLUE,
        [StringValue("Grey")]
        MFD_GREY,
        [StringValue("Bright Green")]
        MFD_BRIGHT_GREEN,
        [StringValue("Light Gray")]
        MFD_WHITY_GRAY,
        [StringValue("Dark Gray")]
        MFD_DARK_GRAY,
        [StringValue("Black")]
        MFD_BLACK,
        [StringValue("Dark Green")]
        MFD_DARK_GREEN

    }



    // FCR Settings

    /// <summary>
    /// Submodes for the Air-to-Ground Bombing Mode
    /// </summary>
    public enum FCR_Submode
    {
        [StringValue("STRF")]
        STRF,
        [StringValue("EEGS")]
        EEGS,
        [StringValue("SNAP")]
        SNAP,
        [StringValue("LCOS")]
        LCOS,
        [StringValue("SSLC")]
        SSLC,
        [StringValue("SRM")]
        SRM,
        [StringValue("MRM")]
        MRM,
        [StringValue("CCIP")]
        CCIP,
        [StringValue("CCRP")]
        CCRP,
        [StringValue("LADD")]
        LADD,
        [StringValue("DTOS")]
        DTOS,
        [StringValue("EO-VIS")]
        EOVIS,
        [StringValue("EO-PRE")]
        EOPRE,
        [StringValue("EO-BORE")]
        EOBORE,
        [StringValue("IAM-VIS")]
        IAMVIS,
        [StringValue("IAM-PRE")]
        IAMPRE
    }

    /// <summary>
    /// Selectable modes for the AIM 9 Seeker Mode
    /// </summary>
    public enum AIM9_SearchMode
    {
        [StringValue("Spot")]
        Spot,
        [StringValue("Scan")]
        Scan
    }

    /// <summary>
    /// AIM9 Seeker Threshold Detection Mode
    /// </summary>
    public enum AIM9_ThresholdMode
    {
        BP,
        TD
    }

    /// <summary>
    /// Default Size setting for the Aim-120 Targeting Computer
    /// </summary>
    public enum AIM120_TargetSize
    {
        Unknown,
        Small,
        Medium,
        Large
    }

    /// <summary>
    /// Fuzing Modes for Bombs
    /// </summary>
    public enum FuzeMode
    {
        NOSE,
        TAIL,
        NSTL
    }

    /// <summary>
    /// Selectable Modes for the HARM Configuration
    /// </summary>
    public enum HARMMode
    {
        HAS,
        POS
    }

    /// <summary>
    /// Selectable Sub-Modes for the HARM Configuration
    /// </summary>
    public enum HARMSubMode
    {
        PN,
        EOM,
        RUK
    }

    /// <summary>
    /// Nav Offset Mode
    /// </summary>
    public enum NavOffsetMode
    {
        [StringValue("NONE")]
        NONE = -1,
        [StringValue("VRP")]
        VRP,
        [StringValue("VIP")]
        VIP
    }


    // IFF Settings

    /// <summary>
    /// Values that can be set for the Auto Change Condition for the IFF in the DTC
    /// </summary>
    public enum IFFAutoChange
    {
        MAN,
        TIM,
        POS,
        PT
    }
}
