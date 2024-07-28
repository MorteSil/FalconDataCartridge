using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;
using Utilities.GeoLib;


namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the STPT Configuration Settings of an INI File
    /// </summary>
    public class STPT : INIComponent, IEquatable<STPT>
    {
        #region Properties
        /// <summary>
        /// List of Waypoint / <see cref="STPTTarget"/> Objects in the DTC: Entries 0-23, 80-98.
        /// </summary>
        public Collection<STPTTarget> Waypoints { get => waypoints; set =>  waypoints = value; } // Note: Mutable List
        /// <summary>
        /// List of <see cref="STPTPPT"/> Objects in the DTC: 25-39.
        /// </summary>
        public Collection<STPTPPT> PPT { get => ppt; set => ppt = value; } // Note: Mutable List
        /// <summary>
        /// List of <see cref="STPTLine"/> Objects in the DTC: 40-64. TODO:Update entries
        /// </summary>
        public Collection<STPTLine> Lines {  get => lines; set => lines = value; } // Note: Mutable List
        /// <summary>
        /// List of Targets / <see cref="STPTTarget"/> Objects in the DTC: Entries 0-99.
        /// </summary>
        public Collection<STPTWeaponTarget> Targets { get => targets; set => targets = value; } // Note: Mutable List
        #endregion Properties

        #region Fields
        private Collection<STPTTarget> waypoints = [];
        private Collection<STPTPPT> ppt = [];
        private Collection<STPTLine> lines = [];
        private Collection<STPTWeaponTarget> targets = [];

        #endregion Fields

        #region Helper Methods

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [STPT] flag                   
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Clear the Lists
                waypoints.Clear();
                ppt.Clear();
                lines.Clear();
                targets.Clear();

                // Find the section tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();

                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;
                                        
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    string[] delims = ["_", "=", ","];
                    string[] PGM = s.Split(delims, StringSplitOptions.None);
                    int id = int.Parse(PGM[1]);

                    if (PGM.Length == 7)
                    {
                        switch (PGM[0].ToUpper())
                        {
                            case "TARGET":
                                waypoints.Add(new STPTTarget(
                                    _ = int.Parse(PGM[1]),
                                    _ = double.Parse(PGM[2]),
                                    _ = double.Parse(PGM[3]),
                                    _ = double.Parse(PGM[4]),
                                    _ = int.Parse(PGM[5]),
                                    PGM[6].TrimStart()));
                                break;

                            case "WPNTARGET":
                                targets.Add(new STPTWeaponTarget(
                                   _ = int.Parse(PGM[1]),
                                   _ = double.Parse(PGM[2]),
                                   _ = double.Parse(PGM[3]),
                                   _ = double.Parse(PGM[4]),
                                   _ = int.Parse(PGM[5]),
                                   PGM[6].TrimStart()));
                                break;

                            case "PPT":
                                ppt.Add(new STPTPPT(
                                   _ = int.Parse(PGM[1]),
                                   _ = double.Parse(PGM[2]),
                                   _ = double.Parse(PGM[3]),
                                   _ = double.Parse(PGM[4]),
                                   PGM[6].TrimStart()));
                                break;
                        }
                    }
                    else if (PGM.Length == 6)
                    {
                        switch (PGM[0].ToUpper())
                        {
                            case "TARGET":
                                waypoints.Add(new STPTTarget(
                                    _ = int.Parse(PGM[1]),
                                    _ = double.Parse(PGM[2]),
                                    _ = double.Parse(PGM[3]),
                                    _ = double.Parse(PGM[4]),
                                    _ = int.Parse(PGM[5]),
                                    ""));
                                break;

                            case "WPNTARGET":
                                targets.Add(new STPTWeaponTarget(
                                   _ = int.Parse(PGM[1]),
                                   _ = double.Parse(PGM[2]),
                                   _ = double.Parse(PGM[3]),
                                   _ = double.Parse(PGM[4]),
                                   _ = int.Parse(PGM[5]),
                                   ""));
                                break;
                        }
                    }
                    else if (PGM.Length == 5)
                    {
                        switch (PGM[0].ToUpper())
                        {
                            case "LINESTPT":
                                lines.Add(new STPTLine(
                                   _ = int.Parse(PGM[1]),
                                   _ = double.Parse(PGM[2]),
                                   _ = double.Parse(PGM[3])
                                   ));
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
            foreach (STPTTarget t in waypoints) { sb.AppendLine(t.Write()); }
            foreach (STPTPPT t in ppt) { sb.AppendLine(t.Write()); }
            foreach (STPTLine t in lines) { sb.AppendLine(t.Write()); }
            foreach (STPTWeaponTarget t in targets) { sb.AppendLine(t.Write()); }

            return sb.ToString();
        }
        
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {

            StringBuilder sb = new(SectionFlag + Environment.NewLine);

            sb.AppendLine("******************* Steerpoint Configuration *******************");
            foreach (STPTTarget t in waypoints) { sb.Append(t.ToString()); }
            foreach (STPTPPT t in ppt) { sb.Append(t.ToString()); }
            foreach (STPTLine t in lines) { sb.Append(t.ToString()); }
            foreach (STPTWeaponTarget t in targets) { sb.Append(t.ToString()); }

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(STPT? other)
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

            if (other is not STPT comparator)
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
                if (waypoints is not null)
                    foreach (STPTTarget sp in waypoints)
                        hash = hash ^= sp.GetHashCode();
                if (ppt is not null)
                    foreach (STPTPPT spp in ppt)
                        hash ^= spp.GetHashCode();
                if (lines is not null)
                    foreach (STPTLine l in lines)
                        hash ^= l.GetHashCode();
                if (targets is not null)
                    foreach (STPTWeaponTarget weaponTarget in targets)
                        hash ^= weaponTarget.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(STPT comparator1, STPT comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(STPT comparator1, STPT comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="STPT"/> object
        /// </summary>
        public STPT()
        {
            SectionFlag = "[STPT]";
            for (int i = 0; i < 24; i++)
                waypoints.Add(new STPTTarget(i));
            for (int i = 80; i < 99; i++)
                waypoints.Add(new STPTTarget(i));
            for (int i = 0; i < 15; i++)
                ppt.Add(new STPTPPT(i));
            for (int i = 0; i < 24; i++)
                lines.Add(new STPTLine(i));
            for (int i = 0; i < 100; i++)
                targets.Add(new STPTWeaponTarget(i));
        }
        /// <summary>
        /// Initializes an instance of the <see cref="STPT"/> object with the values in <paramref name="steerpoint"/>
        /// </summary>
        /// <param name="steerpoint">The <see cref="STPT"/> object with the values to copy</param>
        public STPT(STPT steerpoint)
        {
            SectionFlag = "[STPT]";
            foreach (STPTTarget t in steerpoint.waypoints)
                waypoints.Add(new STPTTarget(t));
            foreach (STPTPPT p in steerpoint.ppt)
                ppt.Add(new STPTPPT(p));
            foreach (STPTLine line in steerpoint.lines)
                lines.Add(new STPTLine(line));
            foreach (STPTWeaponTarget weapon in steerpoint.targets)
                targets.Add(new STPTWeaponTarget(weapon));
        }
        /// <summary>
        /// Initializes a new <see cref="STPT"/> object with the supplied values
        /// </summary>
        /// <param name="waypoints">Collection of <see cref="STPTTarget"/> objects to represent Waypoints</param>
        /// <param name="ppt">Collection of <see cref="STPTPPT"/> objects representing Pre Planned Threat Locations</param>
        /// <param name="lines">Collection of <see cref="STPTLine"/> objects representing the Map Line Coordinates</param>
        /// <param name="targets">Collection of <see cref="STPTWeaponTarget"/> objects representing the Weapon Target Coordinates assignable to certain IAM Weaponns, such as SPICE Bombs</param>
        public STPT(ICollection<STPTTarget> waypoints, ICollection<STPTPPT> ppt, ICollection<STPTLine> lines, ICollection<STPTWeaponTarget> targets)
        {
            SectionFlag = "[STPT]";
            foreach (STPTTarget t in waypoints)
                this.waypoints.Add(new STPTTarget(t));
            foreach (STPTPPT p in ppt)
                this.ppt.Add(new STPTPPT(p));
            foreach (STPTLine line in lines)
                this.lines.Add(new STPTLine(line));
            foreach (STPTWeaponTarget weapon in targets)
                this.targets.Add(new STPTWeaponTarget(weapon));

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="STPT"/> object with the values in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data</param>
        public STPT(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

        #region Sub-Classes
        /// <summary>
        /// A Generic Steerpoint Type used for Waypoints in the DTC
        /// </summary>
        public class STPTTarget
        {
            #region Properties
            /// <summary>
            /// Steerpoint ID
            /// </summary>
            public int SteerpointID { get => steerpointID; set => steerpointID = value; }            
            /// <summary>
            /// <para>The Y Value is associated with Latitude. Negative Values indicate South Latitude Values.</para>
            /// </summary>
            public double Y { get => geoPoint.Y; set => geoPoint.Y = value; }            
            /// <summary>
            /// <para>The X Value is associated with Longitude. Negative Values indicate West Longitude Values.</para>
            /// </summary>
            public double X { get => geoPoint.X; set => geoPoint.X = value; }
            /// <summary>
            /// <para>Ground Elevation at this location.</para>
            /// <para>This is NOT Altitude at the Steerpoint. 
            /// This is the Elevation of the ground below the steerpoint to
            /// facilitate 3D Targeting for weapons (JDAM, Spice, etc...)</para>
            /// </summary>
            public double Height { get => geoPoint.Elevation; set => geoPoint.Elevation = value; } // NOTE: -values are actually positive elevation, handle appropriately in your code      
            /// <summary>
            /// Action being performed at the Steerpoint
            /// </summary>
            public STPTAirAction Action {  get => action; set => action = value; }            
            /// <summary>
            /// Comment for this Steerpoint
            /// </summary>
            public string Comment {  get => comment; set => comment = value; }           
            /// <summary>
            /// Formatted Display of the Lattitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Latitude { get => geoPoint.Latitude; } 
            /// <summary>
            /// Formatted Display of the Longitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Longitude {  get =>  geoPoint.Longitude; }          

            #endregion Properties

            #region Fields
            private int steerpointID = 0;
            private GeoPoint geoPoint = new (0, 0, 0);
            private STPTAirAction action = STPTAirAction.Precision;
            private string comment = "Not Set";
            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                string val = "target_" + steerpointID + "=" + geoPoint.X.ToString("F6") + ", " + geoPoint.Y.ToString("F6") + ", " + geoPoint.Elevation.ToString("F6") + ", " + (int)action;
                if (comment is not null & comment != "")
                    val += ", " + comment;
                return val;
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
                sb.AppendLine("     Steerpoint ID: " + SteerpointID);
                sb.AppendLine("        X: " + X);
                sb.AppendLine("        Y: " + Y);
                sb.AppendLine("        Altitude: " + Height);
                sb.AppendLine("        Action: " + Action);
                sb.AppendLine("        Comment: " + Comment);
                sb.AppendLine("        Latitude: " + Latitude);
                sb.AppendLine("        Longitude: " + Longitude);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(STPTTarget? other)
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

                if (other is not STPTTarget comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + SteerpointID.GetHashCode();
                    hash = hash * 5483 + Y.GetHashCode();
                    hash = hash * 5483 + X.GetHashCode();
                    hash = hash * 5483 + Height.GetHashCode();
                    hash = hash * 5483 + Action.GetHashCode();
                    hash = hash * 5483 + Comment.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(STPTTarget comparator1, STPTTarget comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(STPTTarget comparator1, STPTTarget comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="STPTTarget"/> object
            /// </summary>
            public STPTTarget()
            {

            }
            /// <summary>
            /// Initializes a new <see cref="STPTTarget"/> object using the values in <paramref name="stpt"/>
            /// </summary>
            /// <param name="stpt">The <see cref="STPTTarget"/> object with the values to copy</param>
            public STPTTarget(STPTTarget stpt)
            {
                action = stpt.Action;
                comment = stpt.comment;
                geoPoint = new GeoPoint(stpt.geoPoint);
                steerpointID = stpt.steerpointID;
            }
            /// <summary>
            /// Initializes a <see cref="STPTTarget"/> object with default settings set to Waypoint provided in <paramref name="id"/>
            /// </summary>
            /// <param name="id"></param>
            public STPTTarget(int id)
            {
                steerpointID = id;
                if (id < 25)
                    comment = "";
                else
                    comment = "Not set";
            }
            /// <summary>
            /// Initializes a <see cref="STPTTarget"/> object with the supplied values
            /// </summary>
            /// <param name="id">Waypoint this Steerpoint represents</param>
            /// <param name="x">X Component of the Steerpoint</param>
            /// <param name="y">Y Component of the Steerpoint</param>
            /// <param name="altitude">Altitude of the Steerpoint</param>
            /// <param name="action">Action to be performed at this Steerpoint</param>
            /// <param name="comment">Comment associated with this Steerpoint</param>
            public STPTTarget(int id, double x, double y, double altitude, int action, string comment)
            {
                steerpointID = id;
                geoPoint = new GeoPoint(x, y, altitude);
                this.action = (STPTAirAction)action;
                this.comment = comment;
            }
            #endregion Constructors
        }

        /// <summary>
        /// A Steerpoint Entry for a Pre-Planned Threat Object in the DTC
        /// </summary>
        public class STPTPPT
        {
            #region Properties
            /// <summary>
            /// Steerpoint ID
            /// </summary>
            public int SteerpointID { get => steerpointID; set => steerpointID = value; }
            /// <summary>
            /// <para>The Y Value is associated with Latitude. Negative Values indicate South Latitude Values.</para>
            /// </summary>
            public double Y { get => geoPoint.Y; set => geoPoint.Y = value; }
            /// <summary>
            /// <para>The X Value is associated with Longitude. Negative Values indicate West Longitude Values.</para>
            /// </summary>
            public double X { get => geoPoint.X; set => geoPoint.X = value; }
            /// <summary>
            /// Radius of the Threat Circle.
            /// </summary>
            public double Radius { get => geoPoint.Elevation; set => geoPoint.Elevation = value; } // NOTE: -values are actually positive elevation, handle appropriately in your code   
            /// <summary>
            /// Comment for this Steerpoint
            /// </summary>
            public string Comment { get => comment; set => comment = value; }
            /// <summary>
            /// Formatted Display of the Lattitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Latitude { get => geoPoint.Latitude; }
            /// <summary>
            /// Formatted Display of the Longitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Longitude { get => geoPoint.Longitude; }

            #endregion Properties

            #region Fields
            private int steerpointID = 0;
            private readonly double unused = 0;
            private string comment = "";
            private GeoPoint geoPoint = new (0, 0, 0);
            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                return "ppt_" + steerpointID + "=" + geoPoint.X.ToString("F6") + ", " + geoPoint.Y.ToString("F6") + ", " + geoPoint.Elevation.ToString("F6") + ", " + unused.ToString("F6") + "," + comment;
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
                sb.AppendLine("     Steerpoint ID: " + SteerpointID);
                sb.AppendLine("        X: " + X);
                sb.AppendLine("        Y: " + Y);
                sb.AppendLine("        Radius: " + Radius);
                sb.AppendLine("        Comment: " + Comment);
                sb.AppendLine("        Latitude: " + geoPoint.Latitude);
                sb.AppendLine("        Longitude: " + geoPoint.Longitude);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(STPTPPT? other)
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

                if (other is not STPTPPT comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + SteerpointID.GetHashCode();
                    hash = hash * 5483 + Y.GetHashCode();
                    hash = hash * 5483 + X.GetHashCode();
                    hash = hash * 5483 + Radius.GetHashCode();
                    hash = hash * 5483 + Comment.GetHashCode();


                    return hash;
                }
            }
            public static bool operator ==(STPTPPT comparator1, STPTPPT comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(STPTPPT comparator1, STPTPPT comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="STPTPPT"/> object
            /// </summary>
            public STPTPPT()
            {

            }
            /// <summary>
            /// Initializes a <see cref="STPTPPT"/> object with the values in <paramref name="ppt"/>
            /// </summary>
            /// <param name="ppt">The <see cref="STPTPPT"/> object with the values to copy</param>
            public STPTPPT(STPTPPT ppt)
            {
                steerpointID = ppt.SteerpointID;
                geoPoint = new GeoPoint(ppt.geoPoint);
                unused = ppt.unused;
                comment = ppt.comment;
            }
            /// <summary>
            /// Initializes a generic instance of the <see cref="STPTPPT"/> object set to the Steerpoint ID in <paramref name="id"/>
            /// </summary>
            /// <param name="id">Steerpoint ID to set for this Pre-Planned Threat</param>
            public STPTPPT(int id)
            {
                steerpointID = id;
            }
            /// <summary>
            /// Initializes an instance of the <see cref="STPTPPT"/> object with the supplied values
            /// </summary>
            /// <param name="id">Steerpoint ID</param>
            /// <param name="x">X Map Coordinate</param>
            /// <param name="y">Y Map Coordinate</param>
            /// <param name="radius">Radius of the PPT Circle</param>
            /// <param name="comment">Comment</param>
            public STPTPPT(int id, double x, double y, double radius, string comment)
            {
                steerpointID = id;
                geoPoint = new GeoPoint(x, y, radius);
                this.comment = comment;
            }
            #endregion Constructors
        }

        /// <summary>
        /// Steerpoints that make up the Lines on the Map
        /// </summary>
        public class STPTLine
        {
            #region Properties
            /// <summary>
            /// Steerpoint ID
            /// </summary>
            public int SteerpointID { get => steerpointID; set => steerpointID = value; }
            /// <summary>
            /// <para>The Y Value is associated with Latitude. Negative Values indicate South Latitude Values.</para>
            /// </summary>
            public double Y { get => geoPoint.Y; set => geoPoint.Y = value; }
            /// <summary>
            /// <para>The X Value is associated with Longitude. Negative Values indicate West Longitude Values.</para>
            /// </summary>
            public double X { get => geoPoint.X; set => geoPoint.X = value; }
            /// <summary>
            /// <para>Ground Elevation at this location.</para>
            /// <para>This is NOT Altitude at the Steerpoint. 
            /// This is the Elevation of the ground below the steerpoint to
            /// facilitate 3D Targeting for weapons (JDAM, Spice, etc...)</para>
            /// </summary>
            public double Height { get => geoPoint.Elevation; set => geoPoint.Elevation = value; } // NOTE: -values are actually positive elevation, handle appropriately in your code   
            /// <summary>
            /// Formatted Display of the Lattitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Latitude { get => geoPoint.Latitude; }
            /// <summary>
            /// Formatted Display of the Longitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Longitude { get => geoPoint.Longitude; }

            #endregion Properties // Checked

            #region Fields
            private int steerpointID = 0;
            private int line = 1;
            private int point = 1;
            private GeoPoint geoPoint = new (0, 0, 0);
            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                return "lineSTPT_" + steerpointID + "=" + geoPoint.X.ToString("F6") + ", " + geoPoint.Y.ToString("F6") + ", " + geoPoint.Elevation.ToString("F6");
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
                sb.AppendLine("     Line ID: " + SteerpointID / 6);
                sb.AppendLine("        Line ID Point Index: " + SteerpointID % 6);
                sb.AppendLine("        Steerpoint ID: " + SteerpointID);
                sb.AppendLine("        X: " + X);
                sb.AppendLine("        Y: " + Y);
                sb.AppendLine("        Altitude: " + geoPoint.Elevation);
                sb.AppendLine("        Latitude: " + geoPoint.Latitude);
                sb.AppendLine("        Longitude: " + geoPoint.Longitude);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(STPTLine? other)
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

                if (other is not STPTLine comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + SteerpointID.GetHashCode();
                    hash = hash * 5483 + Y.GetHashCode();
                    hash = hash * 5483 + X.GetHashCode();
                    hash = hash * 5483 + Height.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(STPTLine comparator1, STPTLine comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(STPTLine comparator1, STPTLine comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="STPTLine"/> object
            /// </summary>
            public STPTLine()
            {

            }
            /// <summary>
            /// Initializes a new <see cref="STPTLine"/> object with the values in <paramref name="line"/>
            /// </summary>
            /// <param name="line">The <see cref="STPTLine"/> object with the values to copy</param>
            public STPTLine(STPTLine line)
            {
                steerpointID = line.steerpointID;
                geoPoint = new GeoPoint(line.geoPoint);
            }
            /// <summary>
            /// Initializes a new generic instance of the <see cref="STPTLine"/> object with the Steerpoint ID in <paramref name="id"/>
            /// </summary>
            /// <param name="id">ID of the Steerpoint entry</param>
            public STPTLine(int id)
            {
                steerpointID = id;
            }
            /// <summary>
            /// Initializes an instance of the <see cref="STPTLine"/> object with the supplied values
            /// </summary>
            /// <param name="id"><para>Steerpoint ID with regard to the first Line STPT</para>
            /// <para>Each Line has a total of 6 STPTs assigned, so a value of 9 would be the 3rd Point in Line 2</para></param>
            /// <param name="x">X Map Coordinate</param>
            /// <param name="y">Y Map Coordinate</param>
            public STPTLine(int id, double x, double y)
            {
                steerpointID = id;
                geoPoint = new GeoPoint(x, y);
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="STPTLine"/> object for a specific Line
            /// </summary>            
            /// <param name="x">X Map Coordinate</param>
            /// <param name="y">Y Map Coordinate</param>
            /// <param name="lineID">The Line to add this Steerpoint to</param>
            /// <param name="linePointID">Point Number of the Line to place this Steerpoint. Each Line can have up to 6 STPSs</param>
            public STPTLine(double x, double y, int lineID, int linePointID)
            {
                line = Math.Max(1, Math.Min(4, lineID));
                point = Math.Max(1, Math.Min(6, linePointID));

                steerpointID = (line - 1) * 6 + (point - 1);
                geoPoint = new GeoPoint(x, y);

            }
            #endregion Constructors
        }

        /// <summary>
        /// A Steerpoint Object for the Weapon List used in Spice Targeting
        /// </summary>
        public class STPTWeaponTarget
        {
            #region Properties
            /// <summary>
            /// Steerpoint ID
            /// </summary>
            public int SteerpointID { get => steerpointID; set => steerpointID = value; }
            /// <summary>
            /// <para>The Y Value is associated with Latitude. Negative Values indicate South Latitude Values.</para>
            /// </summary>
            public double Y { get => geoPoint.Y; set => geoPoint.Y = value; }
            /// <summary>
            /// <para>The X Value is associated with Longitude. Negative Values indicate West Longitude Values.</para>
            /// </summary>
            public double X { get => geoPoint.X; set => geoPoint.X = value; }
            /// <summary>
            /// <para>Ground Elevation at this location.</para>
            /// <para>This is NOT Altitude at the Steerpoint. 
            /// This is the Elevation of the ground below the steerpoint to
            /// facilitate 3D Targeting for weapons (JDAM, Spice, etc...)</para>
            /// </summary>
            public double Height { get => geoPoint.Elevation; set => geoPoint.Elevation = value; } // NOTE: -values are actually positive elevation, handle appropriately in your code      
            /// <summary>
            /// Action being performed at the Steerpoint
            /// </summary>
            public STPTAirAction Action { get => action; set => action = value; }
            /// <summary>
            /// Comment for this Steerpoint
            /// </summary>
            public string Comment { get => comment; set => comment = value; }
            /// <summary>
            /// Formatted Display of the Lattitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Latitude { get => geoPoint.Latitude; }
            /// <summary>
            /// Formatted Display of the Longitude Component in Degrees Minutes Second Format
            /// </summary>
            public string Longitude { get => geoPoint.Longitude; }

            #endregion Properties

            #region Fields
            private int steerpointID = 0;
            private STPTAirAction action = STPTAirAction.Precision;
            private string comment = "Not set";
            private GeoPoint geoPoint = new (0, 0, 0);
            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                string val = "wpntarget_" + steerpointID + "=" + geoPoint.X.ToString("F6") + ", " + geoPoint.Y.ToString("F6") + ", " + geoPoint.Elevation.ToString("F6") + ", " + (int)action;
                if (comment is not null & comment != "")
                    val += ", " + comment;
                return val;
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
                sb.AppendLine("     Steerpoint ID: " + SteerpointID);
                sb.AppendLine("        X: " + X);
                sb.AppendLine("        Y: " + Y);
                sb.AppendLine("        Altitude: " + Height);
                sb.AppendLine("        Action: " + Action);
                sb.AppendLine("        Comment: " + Comment);
                sb.AppendLine("        Latitude: " + Latitude);
                sb.AppendLine("        Longitude: " + Longitude);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(STPTWeaponTarget? other)
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

                if (other is not STPTWeaponTarget comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + SteerpointID.GetHashCode();
                    hash = hash * 5483 + Latitude.GetHashCode();
                    hash = hash * 5483 + Longitude.GetHashCode();
                    hash = hash * 5483 + Height.GetHashCode();
                    hash = hash * 5483 + Action.GetHashCode();
                    hash = hash * 5483 + Comment.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(STPTWeaponTarget comparator1, STPTWeaponTarget comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(STPTWeaponTarget comparator1, STPTWeaponTarget comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="STPTWeaponTarget"/> object
            /// </summary>
            public STPTWeaponTarget()
            {

            }
            /// <summary>
            /// Initializes an instance of the <see cref="STPTWeaponTarget"/> object with the values in <paramref name="target"/>
            /// </summary>
            /// <param name="target"></param>
            public STPTWeaponTarget(STPTWeaponTarget target)
            {
                steerpointID = target.SteerpointID;
                geoPoint = new GeoPoint(target.geoPoint);
                action = target.action;
                comment = target.comment;
            }
            /// <summary>
            /// Initializes a <see cref="STPTWeaponTarget"/> object with default settings set to Waypoint provided in <paramref name="id"/>
            /// </summary>
            /// <param name="id"></param>
            public STPTWeaponTarget(int id)
            {
                steerpointID = id;
            }
            /// <summary>
            /// Initializes an instance of the <see cref="STPTWeaponTarget"/> object with the values supplied
            /// </summary>
            /// <param name="id">Steerpoint ID</param>
            /// <param name="x">X Value of the Map Coordinate</param>
            /// <param name="y">Y Value of the Map Coordinate</param>
            /// <param name="altitude">Altitude</param>
            /// <param name="action">Action to perform at this steerpoint</param>
            /// <param name="comment">Comment</param>
            public STPTWeaponTarget(int id, double x, double y, double altitude, int action, string comment)
            {
                steerpointID = id;
                geoPoint = new GeoPoint(x, y, altitude);
                this.action = (STPTAirAction)action;
                this.comment = comment;

            }
            #endregion Constructors
        }

        #endregion Sub-Classes	

    }
}
