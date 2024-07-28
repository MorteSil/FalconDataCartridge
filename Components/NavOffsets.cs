using FalconDataCartridge.Enums;
using System.Text;


namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the NavOffsets Section of an INI File
    /// </summary>
    public class NavOffset : INIComponent, IEquatable<NavOffset>
    {
        #region Properties
        /// <summary>
        /// VIP Profile
        /// </summary>
        public OffsetProfile VIP { get => vip; set => vip = value; }        
        /// <summary>
        /// VRP Profile
        /// </summary>
        public OffsetProfile VRP {  get => vrp; set => vrp = value; }        
        /// <summary>
        /// VIP Pull-Up Profile
        /// </summary>
        public OffsetProfile VIPPUP {  get => vippup; set => vippup = value; }        
        /// <summary>
        /// VRP Pull-UP Profile
        /// </summary>
        public OffsetProfile VRPPUP { get => vrppup; set => vrppup = value; }        
        /// <summary>
        /// VIP OA1 Profile
        /// </summary>
        public OffsetProfile VIPOA1 { get => vipoa1; set => vipoa1 = value; }       
        /// <summary>
        /// VIP OA2 Profile
        /// </summary>
        public OffsetProfile VIPOA2 { get => vipoa2; set => vipoa2 = value; }       
        /// <summary>
        /// VRP OA1 Profile
        /// </summary>
        public OffsetProfile VRPOA1 { get => vrpoa1; set => vrpoa1 = value; }       
        /// <summary>
        /// VRP OA2 Profile
        /// </summary>
        public OffsetProfile VRPOA2 { get => vrpoa2; set => vrpoa2 = value; }       
        #endregion Properties

        #region Fields
        private NavOffsetMode mode = NavOffsetMode.NONE;
        private OffsetProfile vip = new();
        private OffsetProfile vrp = new();
        private OffsetProfile vippup = new();
        private OffsetProfile vrppup = new();
        private OffsetProfile vipoa1 = new();
        private OffsetProfile vipoa2 = new();
        private OffsetProfile vrpoa1 = new();
        private OffsetProfile vrpoa2 = new();

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
                    if (s == null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer

                    if (s.StartsWith("MODESEL"))
                        mode = (NavOffsetMode)Enum.GetNames(typeof(NavOffsetMode)).ToList().IndexOf(s.Substring(s.IndexOf("=") + 1));
                    else
                    {
                        string[] delims = ["=", "-", ","];
                        string[] dataLine = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        _ = int.TryParse(dataLine[1], out int stpt);
                        _ = double.TryParse(dataLine[3], out double rng);
                        _ = int.TryParse(dataLine[4], out int elev);
                        _ = double.TryParse(dataLine[2], out double brg);

                        switch (dataLine[0])
                        {
                            case "VIP":
                                vip = new OffsetProfile(stpt, brg, rng, elev);
                                break;
                            case "VIPPUP":
                                vippup = new OffsetProfile(stpt, brg, rng, elev);
                                break;
                            case "VRP":
                                vrp = new OffsetProfile(stpt, brg, rng, elev);
                                break;
                            case "VRPPUP":
                                vrppup = new OffsetProfile(stpt, brg, rng, elev);
                                break;
                            case "OA1":
                                if (VIPOA1.SteerpointID == 0)
                                    vipoa1 = new OffsetProfile(stpt, brg, rng, elev);
                                else
                                    vrpoa1 = new OffsetProfile(stpt, brg, rng, elev);
                                break;
                            case "OA2":
                                if (VIPOA2.SteerpointID == 0)
                                    vipoa2 = new OffsetProfile(stpt, brg, rng, elev);
                                else
                                    vrpoa2 = new OffsetProfile(stpt, brg, rng, elev);
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
            sb.AppendLine("Modesel=" + mode.ToString().ToLowerInvariant());
            sb.AppendLine("VIP=" + vip.SteerpointID + "," + vip.Write());
            sb.AppendLine("VIPPUP=" + vippup.SteerpointID + "," + vippup.Write());
            sb.AppendLine("VRP=" + vrp.SteerpointID + "," + vrp.Write());
            sb.AppendLine("VRPPUP=" + vrppup.SteerpointID + "," + vrppup.Write());
            if (VIPOA1.SteerpointID != 0)
                sb.AppendLine("OA1-" + vipoa1.SteerpointID + "=" + VIPOA1.Write());
            if (VIPOA2.SteerpointID != 0)
                sb.AppendLine("OA2-" + vipoa2.SteerpointID + "=" + VIPOA2.Write());
            if (VRPOA1.SteerpointID != 0)
                sb.AppendLine("OA1-" + vrpoa1.SteerpointID + "=" + VRPOA1.Write());
            if (VRPOA2.SteerpointID != 0)
                sb.AppendLine("OA2-" + vrpoa2.SteerpointID + "=" + VRPOA2.Write());

            return sb.ToString();
        }

        #endregion Helper Methods

        #region Funcitonal Methods
        public override string ToString()
        {

            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            sb.AppendLine("******************* Nav Offset Configuration *******************");
            sb.AppendLine("     Selected Mode: " + mode);
            sb.AppendLine("     VIP: " + vip.ToString());
            sb.AppendLine("     VIPPUP: " + vippup.ToString());
            sb.AppendLine("     VRP: " + vrp.ToString());
            sb.AppendLine("     VRPPUP: " + vrppup.ToString());
            sb.AppendLine("     VIP OA1: ");
            sb.Append(VIPOA1.ToString());
            sb.AppendLine("     VIP OA2: ");
            sb.Append(VIPOA2.ToString());
            sb.AppendLine("     VRP OA1: ");
            sb.Append(VRPOA1.ToString());
            sb.AppendLine("     VRP OA2: ");
            sb.Append(VRPOA1.ToString());
            return sb.ToString();

        }

        #region Equality Functions
        public bool Equals(NavOffset? other)
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

            if (other is not NavOffset comparator)
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
                hash = hash * 5483 + VIP.GetHashCode();
                hash = hash * 5483 + VRP.GetHashCode();
                hash = hash * 5483 + VIPPUP.GetHashCode();
                hash = hash * 5483 + VRPPUP.GetHashCode();
                hash = hash * 5483 + VIPOA1.GetHashCode();
                hash = hash * 5483 + VIPOA2.GetHashCode();
                hash = hash * 5483 + VRPOA1.GetHashCode();
                hash = hash * 5483 + VRPOA2.GetHashCode();


                return hash;
            }
        }
        public static bool operator ==(NavOffset comparator1, NavOffset comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(NavOffset comparator1, NavOffset comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constuctors
        /// <summary>
        /// Initializes a default instance of the <see cref="NavOffset"/> object
        /// </summary>
        public NavOffset()
        {
            SectionFlag = "[NAV OFFSETS]";

        }
        /// <summary>
        /// Initializes a new <see cref="NavOffset"/> object with the values in <paramref name="offset"/>
        /// </summary>
        /// <param name="offset">The <see cref="NavOffset"/></param> object with the values to copy
        public NavOffset(NavOffset offset)
        {
            SectionFlag = "[NAV OFFSETS]";
            vip = new(offset.vip);
            vrp = new(offset.vrp);
            vippup = new(offset.vippup);
            vrppup = new(offset.vrppup);
            vipoa1 = new(offset.vipoa1);
            vipoa2 = new(offset.vipoa2);
            vrpoa1 = new(offset.vrpoa1);
            vrpoa2 = new(offset.vrpoa2);
            mode = offset.mode;
        }
        /// <summary>
        /// Initializes an instance of the <see cref="NavOffset"/> object with the supplied <see cref="OffsetProfile"/> objects
        /// </summary>
        /// <param name="vip">VIP Profile</param>
        /// <param name="vrp">VRP Profile</param>
        /// <param name="vippup">VIP Pullup Profile</param>
        /// <param name="vrppup">VRP Pullup Profile</param>
        /// <param name="vipoa1">VIP OA1 Profile</param>
        /// <param name="vipoa2">VIP OA2 Profile</param>
        /// <param name="vrpoa1">VRP OA1 Profile</param>
        /// <param name="vrpoa2">VRP OA2 Profile</param>
        public NavOffset(OffsetProfile vip, OffsetProfile vrp, OffsetProfile vippup, OffsetProfile vrppup, OffsetProfile vipoa1, OffsetProfile vipoa2, OffsetProfile vrpoa1, OffsetProfile vrpoa2, NavOffsetMode mode)
        {
            SectionFlag = "[NAV OFFSETS]";
            this.vip = new(vip);
            this.vrp = new(vrp);
            this.vippup = new(vippup);
            this.vrppup = new(vrppup);
            this.vipoa1 = new(vipoa1);
            this.vipoa2 = new(vipoa2);
            this.vrpoa1 = new(vrpoa1);
            this.vrpoa2 = new(vrpoa2);
            this.mode = mode;
        }
        /// <summary>
        /// Initializes a new <see cref="NavOffset"/> object with the data in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see langword=""/>string</param> with Initialization data for this <see cref="NavOffset"/> object
        public NavOffset(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

        #region Sub Classes
        /// <summary>
        /// Represents a single Offset Profile in a DTC .ini File
        /// </summary>
        public class OffsetProfile
        {
            #region Properties
            /// <summary>
            /// <para>Steerpoint ID</para>
            /// <para>Valid Range: 0-100</para>
            /// </summary>
            public int SteerpointID { get => steerPointID; set => steerPointID = Math.Min(99, value); }            
            /// <summary>
            /// <para>Bearing Setting</para>
            /// <para>Valid Range: 0-360</para>
            /// </summary>
            public double Bearing
            {
                get => bearing; 
                set
                {
                    double val = value % 360;
                    if (val < 0)
                        val = 360 - val;
                    bearing = Math.Round(val, 1);
                }
            }
            /// <summary>
            /// Range in Feet of Offset
            /// </summary>
            public double Range {  get => range; set => range = value; }           
            /// <summary>
            /// Elevation in Feet of Offset
            /// </summary>
            public int Elevation { get => elevation; set => elevation = value; }          

            #endregion Properties

            #region Fields
            private int steerPointID = 0;
            private double bearing = 0;
            private double range = 0;
            private int elevation = 0;
            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                return bearing.ToString("F1") + "," + (uint)range + "," + elevation;
            }
            /// <summary>
            /// <para>Formats the data contained within this Component object into Readable Text.</para>
            /// <para>Readable Text does not always match the underlying file format and should not be used to save text based files such as .ini, .lst, or .txtpb files.</para>
            /// <para>Instead, use Write() to format all text or binary data for writing to a file.</para>
            /// </summary>
            /// <returns>A formatted <see cref="string"/> with Human Readable Text.</returns>
            public override string ToString()
            {
                StringBuilder sb = new ();
                sb.AppendLine("        Steerpoint ID; " + SteerpointID);
                sb.AppendLine("        Bearing: " + Bearing);
                sb.AppendLine("        Range: " + Range);
                sb.AppendLine("        Elevation: " + Elevation);

                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(OffsetProfile? other)
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

                if (other is not OffsetProfile comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + steerPointID.GetHashCode();
                    hash = hash * 5483 + bearing.GetHashCode();
                    hash = hash * 5483 + range.GetHashCode();
                    hash = hash * 5483 + elevation.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(OffsetProfile comparator1, OffsetProfile comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(OffsetProfile comparator1, OffsetProfile comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="OffsetProfile"/> object
            /// </summary>
            public OffsetProfile() { }
            /// <summary>
            /// Initializes a <see cref="OffsetProfile"/> object with the values in <paramref name="profile"/>
            /// </summary>
            /// <param name="profile">The <see cref="OffsetProfile"/> object with the values to copy</param>
            public OffsetProfile(OffsetProfile profile)
            {
                steerPointID = profile.SteerpointID;
                bearing = profile.Bearing;
                range = profile.Range;
                elevation = profile.Elevation;
            }
            /// <summary>
            /// Initializes an instance of the <see cref="OffsetProfile"/> object with the supplied values
            /// </summary>
            /// <param name="steerpoint">Steerpoint ID assocaited with this <see cref="OffsetProfile"/></param>
            /// <param name="bearing">Bearing To/From the Steerpoint being referenced</param>
            /// <param name="range">Range To/From the Steerpoint being referenced</param>
            /// <param name="elevation">Elevation of the Offset Point</param>
            public OffsetProfile(int steerpoint, double bearing, double range, int elevation)
            {
                steerPointID = steerpoint;
                this.bearing = bearing;
                this.range = range;
                this.elevation = elevation;
            }
            #endregion Constructors

        }
        #endregion Sub Classes
    }
}
