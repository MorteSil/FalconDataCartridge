using Utilities.Attributes;

namespace FalconDataCartridge.Enums
{
    /// <summary>
    /// List of Radio Types in the DTC
    /// </summary>
    public enum RadioType
    {
        [StringValue("UHF")]
        UHF,
        [StringValue("VHF")]
        VHF,
        [StringValue("ILS")]
        ILS
    }

    /// <summary>
    /// Available TACAN Bands
    /// </summary>
    public enum TACANBand
    {
        [StringValue("X")]
        X,
        [StringValue("Y")]
        Y
    }

    /// <summary>
    /// Available TACAN Modes
    /// </summary>
    public enum TACANMode
    {
        AA,
        AG
    }
}
