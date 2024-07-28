using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;
using Utilities.Attributes;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the Radio Configuration Settings of an INI File
    /// </summary>
    public class Radio : INIComponent, IEquatable<Radio>
    {
        #region Properties
        /// <summary>
        /// List of UHF Presets.
        /// </summary>
        public Collection<RadioPreset> UHF { get => uhf; set => uhf = value; } // Note: Mutable List
        /// <summary>
        /// List of VHF Presets.
        /// </summary>
        public Collection<RadioPreset> VHF {  get => vhf; set => vhf = value; } // Note: Mutable List
        /// <summary>
        /// List of ILS Presets.
        /// </summary>
        public Collection<RadioPreset> ILS {  get => ils; set => ils = value; } // Note: Mutable List

        #endregion Properties // Checked

        #region Fields
        private Collection<RadioPreset> uhf = [];
        private Collection<RadioPreset> vhf = [];
        private Collection<RadioPreset> ils = [];

        #endregion Fields

        #region Helper Methods

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [Radio] flag                   
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [Radio] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();

                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    string[] delims = ["=", "_"];
                    string[] dataLine = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                    int id;
                    if (dataLine.Length == 3)
                    {
                        _ = double.TryParse(dataLine[2], out double val);
                        _ = int.TryParse(dataLine[1], out id);

                        switch (dataLine[0].ToUpper())
                        {
                            case "UHF":
                                uhf[id - 1].Frequency = val / 1000;
                                break;
                            case "VHF":
                                vhf[id - 1].Frequency = val / 1000;
                                break;
                            case "ILS":
                                // Non-Standard to address Inconsistent BMS Behavior (Stored in Hecto Hz vs Kilo Hz)
                                ils[id - 1].Frequency = val / 100;
                                break;
                        }
                    }
                    else if (dataLine.Length == 4)
                    {
                        _ = int.TryParse(dataLine[2], out id);

                        switch (dataLine[0])
                        {
                            case "UHF":
                                uhf[id - 1].comment = dataLine[3];
                                break;
                            case "VHF":
                                vhf[id - 1].comment = dataLine[3];
                                break;
                            case "ILS":
                                ils[id - 1].comment = dataLine[3];
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Utilities.Logging.ErrorLog.CreateLogFile(ex, "This error occurred while attempting to load " + SectionFlag + " from the following string:\n" + data);
                if (ex is IOException)
                    return false;
                throw;
            }
            return true;
        }

        internal override string Write()
        {
            if (!_IncludeInOutput)
                return "";
            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            foreach (RadioPreset p in UHF)
                sb.Append(p.Write(false, false));
            foreach (RadioPreset p in UHF)
                sb.Append(p.Write(true, false));
            foreach (RadioPreset p in VHF)
                sb.Append(p.Write(false, false));
            foreach (RadioPreset p in VHF)
                sb.Append(p.Write(true, false));
            foreach (RadioPreset p in ILS)
            {
                sb.Append(p.Write(false, true));
                sb.Append(p.Write(true, true));
            }

            return sb.ToString();
        }
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {

            StringBuilder sb = new();
            sb.AppendLine("********************* Radio Configuration **********************");
            sb.AppendLine("     UHF: ");
            foreach (RadioPreset preset in uhf)
                sb.Append(preset.ToString());
            sb.AppendLine("     VHF: ");
            foreach (RadioPreset preset in vhf)
                sb.Append(preset.ToString());
            sb.AppendLine("     ILS: ");
            foreach (RadioPreset preset in ils)
                sb.Append(preset.ToString());


            return sb.ToString();
        }

        

        #region Equality Functions
        public bool Equals(Radio? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;

            return other.ToString() == ToString() & other.Write() == Write();
        }
        public override bool Equals(object? other)
        {
            if (other == null)
                return false;

            if (other is not Radio comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + SectionFlag.GetHashCode();
                if (uhf is not null)
                    if (uhf.Count != 0)
                        foreach (RadioPreset p in uhf)
                            hash ^= p.GetHashCode();
                if (vhf is not null)
                    if (vhf.Count != 0)
                        foreach (RadioPreset p in vhf)
                            hash ^= p.GetHashCode();
                if (ils is not null)
                    if (ils.Count != 0)
                        foreach (RadioPreset p in ils)
                            hash ^= p.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(Radio comparator1, Radio comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(Radio comparator1, Radio comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="Radio"/> object with a default <see cref="RadioPreset"/> list
        /// </summary>
        public Radio()
        {
            SectionFlag = "[Radio]";

            // UHF Defaults
            uhf.Add(new RadioPreset(1, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(2, 225000, "DEP Ground", RadioType.UHF));
            uhf.Add(new RadioPreset(3, 225000, "DEP Tower", RadioType.UHF));
            uhf.Add(new RadioPreset(4, 225000, "DEP Approach", RadioType.UHF));
            uhf.Add(new RadioPreset(5, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(6, 225000, "Tactical", RadioType.UHF));
            uhf.Add(new RadioPreset(7, 225000, "ARR Approach", RadioType.UHF));
            uhf.Add(new RadioPreset(8, 225000, "ARR Tower", RadioType.UHF));
            uhf.Add(new RadioPreset(9, 225000, "ARR Ground", RadioType.UHF));
            uhf.Add(new RadioPreset(10, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(11, 278200, "Advisory", RadioType.UHF));
            uhf.Add(new RadioPreset(12, 225000, "Intra Flight 1", RadioType.UHF));
            uhf.Add(new RadioPreset(13, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(14, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(15, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(16, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(17, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(18, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(19, 225000, "(open)", RadioType.UHF));
            uhf.Add(new RadioPreset(20, 225000, "(open)", RadioType.UHF));

            // VHF Defaults
            vhf.Add(new RadioPreset(1, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(2, 120000, "DEP ATIS", RadioType.VHF));
            vhf.Add(new RadioPreset(3, 120000, "DEP Tower", RadioType.VHF));
            vhf.Add(new RadioPreset(4, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(5, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(6, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(7, 120000, "ARR ATIS", RadioType.VHF));
            vhf.Add(new RadioPreset(8, 120000, "ARR Tower", RadioType.VHF));
            vhf.Add(new RadioPreset(9, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(10, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(11, 120000, "ALT Tower", RadioType.VHF));
            vhf.Add(new RadioPreset(12, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(13, 119500, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(14, 120000, "UNICOM", RadioType.VHF));
            vhf.Add(new RadioPreset(15, 120000, "Flight 1", RadioType.VHF));
            vhf.Add(new RadioPreset(16, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(17, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(18, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(19, 120000, "(open)", RadioType.VHF));
            vhf.Add(new RadioPreset(20, 120000, "(open)", RadioType.VHF));

            // ILS
            ils.Add(new RadioPreset(1, 108000, "ILS_1", RadioType.ILS));
            ils.Add(new RadioPreset(2, 108000, "ILS_2", RadioType.ILS));
            ils.Add(new RadioPreset(3, 108000, "ILS_3", RadioType.ILS));
            ils.Add(new RadioPreset(4, 108000, "ILS_4", RadioType.ILS));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Radio"/> object with the values in <paramref name="radio"/>
        /// </summary>
        /// <param name="radio">The <see cref="Radio"/></param> object with the values to copy
        public Radio(Radio radio)
        {
            SectionFlag = "[Radio]";
            foreach (RadioPreset p in radio.uhf)
                uhf.Add(new RadioPreset(p));
            foreach (RadioPreset p in radio.vhf)
                vhf.Add(new RadioPreset(p));
            foreach (RadioPreset p in radio.ils)
                ils.Add(new RadioPreset(p));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Radio"/> object with the supplied values
        /// </summary>
        /// <param name="uhf">Collection of Radio Presets for the UHF Radio</param>
        /// <param name="vhf">Collection of Radio Presets for the VHF Radio</param>
        /// <param name="ils">Collection of Radio Presets for the ILS Radio</param>
        public Radio(ICollection<RadioPreset> uhf, ICollection<RadioPreset> vhf, ICollection<RadioPreset> ils)
        {
            SectionFlag = "[Radio]";
            foreach (RadioPreset p in uhf)
                this.uhf.Add(new RadioPreset(p));
            foreach (RadioPreset p in vhf)
                this.vhf.Add(new RadioPreset(p));
            foreach (RadioPreset p in ils)
                this.ils.Add(new RadioPreset(p));
        }
        /// <summary>
        /// Initializes a new <see cref="Radio"/> object with the data contained in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data for this object</param>
        public Radio(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors

        #region Sub Classes
        /// <summary>
        /// Represents a single Radio Preset within a DTC .ini File
        /// </summary>
        public class RadioPreset
        {
            #region Properties
            /// <summary>
            /// Identifies which Preset Channel this <see cref="RadioPreset"/> represents
            /// </summary>
            public uint PresetID { get => id; }            
            /// <summary>
            /// <para>The Frequency in MHz to use for this Preset.</para>
            /// <para>Valid Ranges: 
            /// <list type="bullet"> 
            /// <item>UHF: 225.000-399.975 MHz in increments of 0.025</item>
            /// <item>VHF (FM): 30.000-61.000 MHz in increments of 0.025</item>
            /// <item>VHF (AM): 118.000-144.000 MHz in increments of 0.025</item>
            /// <item>ILS (ILS): 108.000-111.975 MHz in increments of 0.025</item>
            /// <item>ILS (NAV): 108.000-111.975 MHz in increments of 0.025</item>
            /// </list></para>
            /// <para>NOTE: UHF, VHF, and ILS are the only types of Radio Presets that can be selected. 
            /// However, the VHF band limitations for AM and FM are implemented. Frequencies entered between
            /// 61.001-107.999 will default to 61.000.
            /// <para>Frequencies that do not adhere to 25 kHz spacing will be rounded down to the next lowest channel.</para>
            /// </para>
            /// </summary>
            public double Frequency
            {
                get => frequency; 
                set
                {
                    if (value == 0)
                    {
                        frequency = 0;
                        return;
                    }

                    double val = Math.Round(value * 1000, 3);
                    // Must be increments of 25KHz
                    while (val % 25 != 0) val--;
                    val /= 1000;

                    switch (radioType)
                    {
                        case RadioType.UHF:
                            val = Math.Max(225.000, Math.Min(399.975, val));
                            break;
                        case RadioType.VHF:
                            if (val < 118.000)
                                val = Math.Max(30.000, Math.Min(61.000, val));      // Valid Range is 30-88, BMS only implements 30-61
                            else val = Math.Max(118.000, Math.Min(144.000, val));
                            break;
                        case RadioType.ILS:
                            val = Math.Max(108.000, Math.Min(117.975, val));
                            break;

                    }
                    frequency = val;
                }
            }
            /// <summary>
            /// Frequency Preset Description
            /// </summary>
            public string Comment {  get => comment; set => comment = value; }           
            /// <summary>
            /// <para>The Type of Radio this Preset belongs to</para>
            /// <para>0: UHF, 1: VHF, 2: ILS</para>
            /// </summary>
            public RadioType RadioType {  get => radioType; set => radioType = value; }
           

            #endregion Properties // Checked

            #region Fields
            internal double frequency = 0;
            internal string comment = "";
            internal RadioType radioType = RadioType.UHF;
            internal uint id = 0;
            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write(bool commentOnly, bool isILS)
            {
                if (commentOnly)
                {
                    return radioType.GetStringValue() + "_COMMENT_" + PresetID + "=" + Comment + Environment.NewLine;
                }
                else
                    return radioType.GetStringValue() + "_" + PresetID + "=" + (int)(Frequency * (isILS ? 100 : 1000)) + Environment.NewLine;
            }
            /// <summary>
            /// <para>Formats the data contained within this Component object into Readable Text.</para>
            /// <para>Readable Text does not always match the underlying file format and should not be used to save text based files such as .ini, .lst, or .txtpb files.</para>
            /// <para>Instead, use Write() to format all text or binary data for writing to a file.</para>
            /// </summary>
            /// <returns>A formatted <see cref="string"/> with Human Readable Text.</returns>
            public override string ToString()
            {

                StringBuilder sb = new();
                sb.AppendLine("        Preset ID: " + PresetID);
                sb.AppendLine("           Frequency: " + Frequency);
                sb.AppendLine("           Comment: " + Comment);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(RadioPreset? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                if (other.GetType() != GetType()) return false;

                return other.ToString() == ToString();
            }
            public override bool Equals(object? other)
            {
                if (other == null)
                    return false;

                if (other is not RadioPreset comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + PresetID.GetHashCode();
                    hash = hash * 5483 + Frequency.GetHashCode();
                    hash = hash * 5483 + RadioType.GetStringValue().GetHashCode();
                    hash = hash * 5483 + comment.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(RadioPreset comparator1, RadioPreset comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(RadioPreset comparator1, RadioPreset comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="RadioPreset"/> object
            /// </summary>
            public RadioPreset() { }
            /// <summary>
            /// Initializes a new instance of the <see cref="RadioPreset"/> object with the values in <paramref name="preset"/>
            /// </summary>
            /// <param name="preset">The <see cref="RadioPreset"/></param> object with the values to copy
            public RadioPreset(RadioPreset preset)
            {
                frequency = preset.frequency;
                comment = preset.comment;
                radioType = preset.radioType;
                id = preset.id;
            }
            /// <summary>
            /// Initializes and instance of the <see cref="RadioPreset"/> object with the supplied values
            /// </summary>
            /// <param name="presetID">The Channel of this Preset Configuration</param>
            /// <param name="frequency">The Frequency for this Preset in Mhz</param>
            /// <param name="comment">The Comment displayed in the DTC View for this Preset</param>
            /// <param name="radioType">The Type of Radio this Preset is used for</param>
            public RadioPreset(uint presetID, double frequency, string comment, string radioType)
            {
                id = presetID;
                this.frequency = frequency;
                this.comment = comment;
                foreach (RadioType r in Enum.GetValues(RadioType.GetType()))
                    if (r.GetStringValue() == radioType) { this.radioType = r; }
            }
            /// <summary>
            /// Initializes and instance of the <see cref="RadioPreset"/> object with the supplied values
            /// </summary>
            /// <param name="presetID">The Channel of this Preset Configuration</param>
            /// <param name="frequency">The Frequency for this Preset in Mhz</param>
            /// <param name="comment">The Comment displayed in the DTC View for this Preset</param>
            /// <param name="radioType">The Type of Radio this Preset is used for</param>
            public RadioPreset(uint presetID, double frequency, string comment, RadioType radioType)
            {
                id = presetID;
                this.frequency = frequency;
                this.comment = comment;
                this.radioType = radioType;
            }

            #endregion Constructors

        }

        #endregion Sub Class
    }
}
