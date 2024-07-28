using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the MFD Configuration Section  of an INI File
    /// </summary>
    public class MFD : INIComponent, IEquatable<MFD>
    {
        #region Properties        
        /// <summary>
        /// List of the displays configurable with the DTC
        /// </summary>
        public Collection<MFDDisplay> Displays { get => displays; set => displays = value; } // Note: Mutable List
        #endregion Properties // Checked

        #region Fields
        private Collection<MFDDisplay> displays = [];
        private int _MFDID {  get => displays is null || displays.Count == 0 ? 0 : displays.Count; }        

        #endregion Fields

        #region Helper Methods
        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [MFD] flag                   
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the Section tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();

                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    string[] delims = ["-", "="];
                    string[] PGM = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    int id = int.Parse(PGM[0].Replace("DISPLAY", ""));
                    int mode = int.Parse(PGM[1]);
                    if (PGM.Length == 4)
                    {
                        _ = int.TryParse(PGM[3], out int val);

                        switch (PGM[2], mode)
                        {
                            case ("0", 0):
                                displays[id].AA.DisplayButtonLeft = val;
                                break;
                            case ("1", 0):
                                displays[id].AA.DisplayButtonCenter = val;
                                break;
                            case ("2", 0):
                                displays[id].AA.DisplayButtonRight = val;
                                break;
                            case ("CSEL", 0):
                                displays[id].AA.SelectedDisplay = val;
                                break;
                            case ("0", 1):
                                displays[id].AG.DisplayButtonLeft = val;
                                break;
                            case ("1", 1):
                                displays[id].AG.DisplayButtonCenter = val;
                                break;
                            case ("2", 1):
                                displays[id].AG.DisplayButtonRight = val;
                                break;
                            case ("CSEL", 1):
                                displays[id].AG.SelectedDisplay = val;
                                break;
                            case ("0", 2):
                                displays[id].NAV.DisplayButtonLeft = val;
                                break;
                            case ("1", 2):
                                displays[id].NAV.DisplayButtonCenter = val;
                                break;
                            case ("2", 2):
                                displays[id].NAV.DisplayButtonRight = val;
                                break;
                            case ("CSEL", 2):
                                displays[id].NAV.SelectedDisplay = val;
                                break;
                            case ("0", 3):
                                displays[id].MSL.DisplayButtonLeft = val;
                                break;
                            case ("1", 3):
                                displays[id].MSL.DisplayButtonCenter = val;
                                break;
                            case ("2", 3):
                                displays[id].MSL.DisplayButtonRight = val;
                                break;
                            case ("CSEL", 3):
                                displays[id].MSL.SelectedDisplay = val;
                                break;
                            case ("0", 4):
                                displays[id].DGFT.DisplayButtonLeft = val;
                                break;
                            case ("1", 4):
                                displays[id].DGFT.DisplayButtonCenter = val;
                                break;
                            case ("2", 4):
                                displays[id].DGFT.DisplayButtonRight = val;
                                break;
                            case ("CSEL", 4):
                                displays[id].DGFT.SelectedDisplay = val;
                                break;
                            case ("0", 5):
                                displays[id].SJ.DisplayButtonLeft = val;
                                break;
                            case ("1", 5):
                                displays[id].SJ.DisplayButtonCenter = val;
                                break;
                            case ("2", 5):
                                displays[id].SJ.DisplayButtonRight = val;
                                break;
                            case ("CSEL", 5):
                                displays[id].SJ.SelectedDisplay = val;
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
            foreach (MFDDisplay d in Displays)
                _ = sb.Append(d.SJ.Write());

            foreach (MFDDisplay d in Displays)
            {
                _ = sb.Append(d.AA.Write());
                _ = sb.Append(d.AG.Write());
                _ = sb.Append(d.NAV.Write());
                _ = sb.Append(d.MSL.Write());
                _ = sb.Append(d.DGFT.Write());
            }

            return sb.ToString();
        }       

        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("********************** MFD Configuration ***********************");
            foreach (MFDDisplay d in Displays)
                sb.Append(d.ToString());

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(MFD? other)
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

            if (other is not MFD comparator)
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
                if (displays is not null)
                    if (displays.Count > 0)
                        foreach (MFDDisplay d in displays)
                            hash ^= d.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(MFD comparator1, MFD comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(MFD comparator1, MFD comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="MFD"/> object with default values
        /// </summary>
        public MFD()
        {
            SectionFlag = "[MFD]";
            for (int i = 0; i < 4; i++)
                displays.Add(new MFDDisplay(i));
        }
        /// <summary>
        /// Initializes a new <see cref="MFD"/> object with the values in <paramref name="mfd"/>
        /// </summary>
        /// <param name="mfd"><see cref="MFD"/> object with the values to copy from</param>
        public MFD(MFD mfd)
        {
            SectionFlag = "[MFD]";
            foreach (MFDDisplay d in mfd.displays)
                displays.Add(new(d));
        }
        /// <summary>
        /// Initializes a new <see cref="MFD"/> object with the supplied values
        /// </summary>
        /// <param name="displays">A collection <see cref="MFDDisplay"/> objects to initialize this <see cref="MFD"/> object</param>
        public MFD(ICollection<MFDDisplay> displays)
        {
            SectionFlag = "[MFD]";
            foreach (MFDDisplay d in displays)
                this.displays.Add(new MFDDisplay(d));
        }
        /// <summary>
        /// Initializes a new <see cref="MFD"/> object with the data contained in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"></param>
        public MFD(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
            
        }
        #endregion Constructors

        #region Sub-Classes
        /// <summary>
        /// Represents the configuration for a single MFD
        /// </summary>
        public class MFDDisplay
        {
            #region Properties
            /// <summary>
            /// MFD Display Configuration for the A-A Mastermode
            /// </summary>
            public MFDConfig AA
            { get; set; }
            /// <summary>
            /// MFD Display Configuration for the A-G Mastermode
            /// </summary>
            public MFDConfig AG
            { get; set; }
            /// <summary>
            /// MFD Display Configuration for the NAV Mastermode
            /// </summary>
            public MFDConfig NAV
            { get; set; }
            /// <summary>
            /// MFD Display Configuration for the MSL Mastermode
            /// </summary>
            public MFDConfig MSL
            { get; set; }
            /// <summary>
            /// MFD Display Configuration for the DGFT Mastermode
            /// </summary>
            public MFDConfig DGFT
            { get; set; }
            /// <summary>
            /// MFD Display Configuration for the Stores Jettison Mastermode
            /// </summary>
            public MFDConfig SJ
            { get; set; }
            /// <summary>
            /// Inidicates which MFD this configuration applies to
            /// </summary>
            public int MFDID
            {
                get => _MFDID; 
                set
                {
                    _MFDID = value;
                    AA.MFDID = AG.MFDID = NAV.MFDID = MSL.MFDID = DGFT.MFDID = SJ.MFDID = value;
                }
            }

            #endregion Properties // Checked

            #region Fields
            internal int _MFDID = 0;
            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                StringBuilder sb = new();
                sb.AppendLine("MFD ID: " + MFDID);
                sb.Append(AA.Write());
                sb.Append(AG.Write());
                sb.Append(NAV.Write());
                sb.Append(MSL.Write());
                sb.Append(DGFT.Write());
                sb.Append(SJ.Write());

                return sb.ToString();
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
                sb.AppendLine("     MFD " + MFDID + " Configuration:");
                sb.AppendLine("        Air-to-Air Configuration: ");
                sb.Append(AA.ToString());
                sb.AppendLine("        Air-to-Ground Configuration: ");
                sb.Append(AG.ToString());
                sb.AppendLine("        Navigation Configuration: ");
                sb.Append(NAV.ToString());
                sb.AppendLine("        MRM Override Configuration: ");
                sb.Append(MSL.ToString());
                sb.AppendLine("        Dogfight Configuration: ");
                sb.Append(DGFT.ToString());
                sb.AppendLine("        Stores Jettison Configuration: ");
                sb.Append(SJ.ToString());

                return sb.ToString();

            }

            #region Equality Functions
            public bool Equals(MFDDisplay? other)
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

                if (other is not MFDDisplay comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(MFDID, AA, AG, NAV, MSL, DGFT, SJ);
            }
            public static bool operator ==(MFDDisplay comparator1, MFDDisplay comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(MFDDisplay comparator1, MFDDisplay comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a new <see cref="MFDDisplay"/> object for the MFD identified in <paramref name="mfdid"/>
            /// </summary>
            /// <param name="mfdid">Identifies which MFD this <see cref="MFDDisplay"/> represents</param>
            public MFDDisplay(int mfdid = 0)
            {
                _MFDID = mfdid;
                AA = new MFDConfig(MFDMasterMode.AA, mfdid);
                AG = new MFDConfig(MFDMasterMode.AG, mfdid);
                NAV = new MFDConfig(MFDMasterMode.NAV, mfdid);
                MSL = new MFDConfig(MFDMasterMode.MSL, mfdid);
                DGFT = new MFDConfig(MFDMasterMode.DGFT, mfdid);
                SJ = new MFDConfig(MFDMasterMode.SJ, mfdid);

            }
            /// <summary>
            /// Initializes a new <see cref="MFDDisplay"/> object with the values in <paramref name="mfd"/>
            /// </summary>
            /// <param name="mfd">The <see cref="MFDDisplay"/> object with the values to copy</param>
            public MFDDisplay(MFDDisplay mfd)
            {
                AA = new(mfd.AA);
                AG = new(mfd.AG);
                NAV = new(mfd.NAV);
                MSL = new(mfd.MSL);
                DGFT = new(mfd.DGFT);
                SJ = new(mfd.SJ);
                _MFDID = mfd._MFDID;
            }
            /// <summary>
            /// Initializes an instance of the <see cref="MFDDisplay"/> object with the supplied values
            /// </summary>
            /// <param name="mfdid">MFD Identifier</param>
            /// <param name="configs">List of <see cref="MFDConfig"/> objects to use for this display</param>
            public MFDDisplay(int mfdid, Collection<MFDConfig> configs) : this(mfdid)
            {
                foreach (MFDConfig config in configs)
                {
                    if (config.MFDID == MFDID)
                    {
                        switch (config.Mode)
                        {
                            case MFDMasterMode.AA:
                                AA = new MFDConfig(config);
                                break;
                            case MFDMasterMode.AG:
                                AG = new MFDConfig(config);
                                break;
                            case MFDMasterMode.NAV:
                                NAV = new MFDConfig(config);
                                break;
                            case MFDMasterMode.MSL:
                                MSL = new MFDConfig(config);
                                break;
                            case MFDMasterMode.DGFT:
                                DGFT = new MFDConfig(config);
                                break;
                            case MFDMasterMode.SJ:
                                SJ = new MFDConfig(config);
                                break;
                        }
                    }
                }
            }
            #endregion Constructors

        }

        /// <summary>
        /// Represents the Configuration of a Single MFD object in the DTC .ini File
        /// </summary>
        public class MFDConfig
        {
            #region Properties
            /// <summary>
            /// MFD Left Option
            /// </summary>
            public int DisplayButtonLeft { get; set; } = 0;
            /// <summary>
            /// MFD Center Option
            /// </summary>
            public int DisplayButtonCenter { get; set; } = 0;
            /// <summary>
            /// MFD Right Option
            /// </summary>
            public int DisplayButtonRight { get; set; } = 0;
            /// <summary>
            /// Selected Display
            /// </summary>
            public int SelectedDisplay { get; set; } = 0;
            /// <summary>
            /// <para>Identifies the MODE this display represents.</para>
            /// </summary>
            public MFDMasterMode Mode { get; set; } = MFDMasterMode.AA;
            /// <summary>
            /// MFD this Configuration Belongs to
            /// </summary>
            public int MFDID { get; set; } = 0;
            

            #endregion Properties

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                StringBuilder sb = new();
                sb.AppendLine("Display" + MFDID + "-" + (int)Mode + "-" + "0=" + DisplayButtonLeft);
                sb.AppendLine("Display" + MFDID + "-" + (int)Mode + "-" + "1=" + DisplayButtonCenter);
                sb.AppendLine("Display" + MFDID + "-" + (int)Mode + "-" + "2=" + DisplayButtonRight);
                sb.AppendLine("Display" + MFDID + "-" + (int)Mode + "-" + "csel=" + SelectedDisplay);
                return sb.ToString();
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
                sb.AppendLine("           Left OSB: " + DisplayButtonLeft);
                sb.AppendLine("           Center OSB: " + DisplayButtonCenter);
                sb.AppendLine("           Right OSB: " + DisplayButtonRight);
                sb.AppendLine("           Selected Display: " + SelectedDisplay);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(MFDConfig? other)
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

                if (other is not MFDConfig comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(MFDID, Mode, DisplayButtonLeft, DisplayButtonCenter, DisplayButtonRight, SelectedDisplay);
            }
            public static bool operator ==(MFDConfig comparator1, MFDConfig comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(MFDConfig comparator1, MFDConfig comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="MFDConfig"/> object
            /// </summary>
            public MFDConfig()
            {

            }
            /// <summary>
            /// Initializes an instance of the <see cref="MFDConfig"/> object with the values in <paramref name="config"/>
            /// </summary>
            /// <param name="config">The <see cref="MFDConfig"/> object with the values to copy</param>
            public MFDConfig(MFDConfig config)
            {
                Mode = config.Mode;
                MFDID = config.MFDID;
                DisplayButtonLeft = config.DisplayButtonLeft;
                DisplayButtonCenter = config.DisplayButtonCenter;
                DisplayButtonRight = config.DisplayButtonRight;
                SelectedDisplay = config.SelectedDisplay;
            }
            /// <summary>
            /// Initializes an instance of the <see cref="MFDConfig"/> object with the supplied values
            /// </summary>
            /// <param name="leftOSB">MFD Page assigned to the Bottom Left MFD Button</param>
            /// <param name="centerOSB">MFD Page assigned to the Bottom Center MFD Button</param>
            /// <param name="rightOSB">MFD Page assigned to the Bottom Right MFD Button</param>
            /// <param name="selectedView">The MFD Page selected by default when selecting this Master Mode</param>
            /// <param name="mode">The Master Mode associated with this configuration</param>
            /// <param name="mfdid">The ID of the MFD this Configuration belongs to</param>
            public MFDConfig(int leftOSB, int centerOSB, int rightOSB, int selectedView, MFDMasterMode mode, int mfdid)
            {
                DisplayButtonLeft = leftOSB; 
                DisplayButtonCenter = centerOSB; 
                DisplayButtonRight = rightOSB; 
                SelectedDisplay = selectedView; 
                Mode = mode; 
                MFDID = mfdid;
            }
            /// <summary>
            /// Initializes a generic <see cref="MFDConfig"/> object
            /// </summary>
            /// <param name="mode">Master Mode of this config</param>
            /// <param name="mfdid">ID of the MFD this config is for</param>
            public MFDConfig(MFDMasterMode mode, int mfdid)
            {
                Mode = mode; MFDID = mfdid;
            }

            #endregion Constructors

        }

        #endregion Sub-Classes	
    }
}
