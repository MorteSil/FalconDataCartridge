using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the Link16 Configuraiton Section of an INI File
    /// </summary>
    public class Link16 : INIComponent, IEquatable<Link16>
    {
        #region Properties
        /// <summary>
        /// Collection of Link-16 Files
        /// </summary>
        public Collection<Link16File> Files { get => files; set => files = value; }        
        #endregion Properties // Checked

        #region Fields
        private Collection<Link16File> files = [];

        #endregion Fields

        #region Helper Methods

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [EWS] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [LINK16] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();

                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper().Replace("FILE_", "");
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer

                    int id = s[0] - 'A';
                    _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out int val);
                    if (s.Contains("GROUP_A_CHANNEL"))
                    {
                        files[id].GroupAChannel = val;
                        continue;
                    }
                    else if (s.Contains("GROUP_B_CHANNEL"))
                    {
                        files[id].GroupBChannel = val;
                        continue;
                    }
                    else if (s.Contains("MISSION_CHANNEL"))
                    {
                        files[id].MissionChannel = val;
                        continue;
                    }
                    else if (s.Contains("FIGHTER_CHANNEL"))
                    {
                        files[id].FighterChannel = val;
                        continue;
                    }
                    else if (s.Contains("SPECIAL_CHANNEL"))
                    {
                        files[id].SpecialChannel = val;
                        continue;
                    }
                    else if (s.Contains("CALLSIGN="))
                    {
                        files[id].Callsign = s[(s.IndexOf('=') + 1)..];
                        continue;
                    }
                    else if (s.Contains("CALLSIGN_NUMBER"))
                    {
                        files[id].CallsignID = val;
                        continue;
                    }
                    else if (s.Contains("FLIGHT_LEAD"))
                    {
                        files[id].IsFlightLead = Convert.ToBoolean(val);
                        continue;
                    }
                    else if (s.Contains("EXT_TIME"))
                    {
                        files[id].ExternalTimeReference = Convert.ToBoolean(val);
                        continue;
                    }
                    else if (s.Contains("TACAN_CHANNEL"))
                    {
                        files[id].TACANChannel = val;
                        continue;
                    }
                    else if (s.Contains("TACAN_BAND"))
                    {
                        files[id].TACANBand = (TACANBand)Enum.GetNames(typeof(TACANBand)).ToList().IndexOf(s.Substring(s.IndexOf('=') + 1));
                        continue;
                    }
                    else if (s.Contains("FLIGHT_1_STN"))
                    {
                        files[id].Flight_1_STN = val;
                        continue;
                    }
                    else if (s.Contains("FLIGHT_2_STN"))
                    {
                        files[id].Flight_2_STN = val;
                        continue;
                    }
                    else if (s.Contains("FLIGHT_3_STN"))
                    {
                        files[id].Flight_3_STN = val;
                        continue;
                    }
                    else if (s.Contains("FLIGHT_4_STN"))
                    {
                        files[id].Flight_4_STN = val;
                        continue;
                    }
                    else if (s.Contains("TEAM_1_STN"))
                    {
                        files[id].Team_1_STN = val;
                        continue;
                    }
                    else if (s.Contains("TEAM_2_STN"))
                    {
                        files[id].Team_2_STN = val;
                        continue;
                    }
                    else if (s.Contains("TEAM_3_STN"))
                    {
                        files[id].Team_3_STN = val;
                        continue;
                    }
                    else if (s.Contains("TEAM_4_STN"))
                    {
                        files[id].Team_4_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_1_STN"))
                    {
                        files[id].Donor_1_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_2_STN"))
                    {
                        files[id].Donor_2_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_3_STN"))
                    {
                        files[id].Donor_3_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_4_STN"))
                    {
                        files[id].Donor_4_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_5_STN"))
                    {
                        files[id].Donor_5_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_6_STN"))
                    {
                        files[id].Donor_6_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_7_STN"))
                    {
                        files[id].Donor_7_STN = val;
                        continue;
                    }
                    else if (s.Contains("DONOR_8_STN"))
                    {
                        files[id].Donor_8_STN = val;
                        continue;
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
            StringBuilder sb = new("[LINK16]" + Environment.NewLine);
            foreach (Link16File f in files)
                sb.Append(f.Write());

            return sb.ToString();
        }
        #endregion Helper Methods

        #region Functional Methods
        public override string ToString()
        {


            StringBuilder sb = new();

            sb.AppendLine("******************** Link-16 Configuration *********************");
            foreach (Link16File f in files)
                sb.Append(f.ToString());

            return sb.ToString();
        }

       

        #region Equality Functions
        public bool Equals(Link16? other)
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

            if (other is not Link16 comparator)
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
                if (files != null)
                    if (files.Count > 0)
                        foreach (Link16File f in files)
                            hash ^= f.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(Link16 comparator1, Link16 comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(Link16 comparator1, Link16 comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="Link16"/> object
        /// </summary>
        public Link16()
        {
            SectionFlag = "[LINK16]";
            for (int i = 0; i < 2; i++)
            {
                files.Add(new Link16File());
                files[i].FileID = (char)('A' + i);
            }
        }
        /// <summary>
        /// Initializes a new <see cref="Link16"/> object with the values from <paramref name="l16"/>
        /// </summary>
        /// <param name="l16">The <see cref="Link16"/> object with the values to copy</param>
        public Link16(Link16 l16)
        {
            SectionFlag = "[LINK16]";
            foreach (Link16File f in l16.files)
                files.Add(new(f));
        }
        /// <summary>
        /// Initializes a new <see cref="Link16"/> object with the supplied <see cref="Link16File"/> objects
        /// </summary>
        /// <param name="files"></param>
        public Link16(ICollection<Link16File> files)
        {
            SectionFlag = "[LINK16]";
            foreach (Link16File f in files)
                this.files.Add(new(f));
        }
        /// <summary>
        /// Initializes an instance of the <see cref="Link16"/> object with the values in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> with initialization data</param>
        public Link16(string initializationData) : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

        #region Sub Classes
        /// <summary>
        /// Represents a single Link-16 File for use as the A or B File
        /// </summary>
        public class Link16File
        {
            #region Properties
            /// <summary>
            /// Link-16 Channel used for Primary Voice Communications
            /// </summary>
            public int GroupAChannel
            { get; set; } = 0;
            /// <summary>
            /// Link-16 Channel used for Secondary Voice Communications
            /// </summary>
            public int GroupBChannel
            { get; set; } = 0;
            /// <summary>
            /// Link-16 Channel used for Mission Link-16 Tracks
            /// </summary>
            public int MissionChannel
            { get; set; } = 0;
            /// <summary>
            /// Link-16 Channel used for Fighter Link-16 Tracks
            /// </summary>
            public int FighterChannel
            { get; set; } = 0;
            /// <summary>
            /// Link-16 Channel used for Special Link-16 Tracks
            /// </summary>
            public int SpecialChannel
            { get; set; } = 0;
            /// <summary>
            /// Callsign associated with this Link-16 Participant
            /// </summary>
            public string Callsign
            { get; set; } = "";
            /// <summary>
            /// Numeric Callsign Identifier
            /// </summary>
            public int CallsignID
            { get; set; } = 0;
            /// <summary>
            /// Identifies if this Link-16 Participant is a Flight Lead
            /// </summary>
            public bool IsFlightLead
            { get; set; } = true;
            /// <summary>
            /// When <see langword="true"/>, uses an External Time Reference
            /// </summary>
            public bool ExternalTimeReference
            { get; set; } = false;
            /// <summary>
            /// Link-16 Participant TACAN Channel
            /// </summary>
            public int TACANChannel
            { get; set; } = 0;
            /// <summary>
            /// Link-16 Participant TACAN Band
            /// </summary>
            public TACANBand TACANBand
            { get; set; } = TACANBand.X;
            /// <summary>
            /// Flight Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Flight_1_STN
            { get; set; } = 0;
            /// <summary>
            /// Flight Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Flight_2_STN
            { get; set; } = 0;
            /// <summary>
            /// Flight Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Flight_3_STN
            { get; set; } = 0;
            /// <summary>
            /// Flight Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Flight_4_STN
            { get; set; } = 0;
            /// <summary>
            /// Team Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Team_1_STN
            { get; set; } = 0;
            /// <summary>
            /// Team Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Team_2_STN
            { get; set; } = 0;
            /// <summary>
            /// Team Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Team_3_STN
            { get; set; } = 0;
            /// <summary>
            /// Team Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Team_4_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_1_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_2_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_3_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_4_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_5_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_6_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_7_STN
            { get; set; } = 0;
            /// <summary>
            /// Station ID of a Link-16 Participant to include in Track Processing
            /// </summary>
            public int Donor_8_STN
            { get; set; } = 0;
            /// <summary>
            /// Identifies if this is File A or File B
            /// </summary>
            public char FileID
            { get; set; } = 'A';
            #endregion Properties // Checked

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                StringBuilder sb = new();

                sb.AppendLine("FILE_" + FileID + "_VOICE_GROUP_A_CHANNEL=" + (GroupAChannel < 0 ? GroupAChannel.ToString("D2") : GroupAChannel.ToString("D3")));
                sb.AppendLine("FILE_" + FileID + "_VOICE_GROUP_B_CHANNEL=" + (GroupBChannel < 0 ? GroupBChannel.ToString("D2") : GroupBChannel.ToString("D3")));
                sb.AppendLine("FILE_" + FileID + "_MISSION_CHANNEL=" + (MissionChannel < 0 ? MissionChannel.ToString("D2") : MissionChannel.ToString("D3")));
                sb.AppendLine("FILE_" + FileID + "_FIGHTER_CHANNEL=" + (FighterChannel < 0 ? FighterChannel.ToString("D2") : FighterChannel.ToString("D3")));
                sb.AppendLine("FILE_" + FileID + "_SPECIAL_CHANNEL=" + (SpecialChannel < 0 ? SpecialChannel.ToString("D2") : SpecialChannel.ToString("D3")));
                sb.AppendLine("FILE_" + FileID + "_CALLSIGN=" + Callsign);
                sb.AppendLine("FILE_" + FileID + "_CALLSIGN_NUMBER=" + CallsignID.ToString("D2"));
                sb.AppendLine("FILE_" + FileID + "_FLIGHT_LEAD=" + (IsFlightLead ? 1 : 0));
                sb.AppendLine("FILE_" + FileID + "_EXT_TIME_REFERENCE=" + (ExternalTimeReference ? 1 : 0));
                sb.AppendLine("FILE_" + FileID + "_TACAN_CHANNEL=" + TACANChannel.ToString("D2"));
                sb.AppendLine("FILE_" + FileID + "_TACAN_BAND=" + TACANBand);
                sb.AppendLine("FILE_" + FileID + "_FLIGHT_1_STN=" + (Flight_1_STN < 0 ? Flight_1_STN.ToString("D4") : Flight_1_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_FLIGHT_2_STN=" + (Flight_2_STN < 0 ? Flight_2_STN.ToString("D4") : Flight_2_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_FLIGHT_3_STN=" + (Flight_3_STN < 0 ? Flight_3_STN.ToString("D4") : Flight_3_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_FLIGHT_4_STN=" + (Flight_4_STN < 0 ? Flight_4_STN.ToString("D4") : Flight_4_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_TEAM_1_STN=" + (Team_1_STN < 0 ? Team_1_STN.ToString("D4") : Team_1_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_TEAM_2_STN=" + (Team_2_STN < 0 ? Team_2_STN.ToString("D4") : Team_2_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_TEAM_3_STN=" + (Team_3_STN < 0 ? Team_3_STN.ToString("D4") : Team_3_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_TEAM_4_STN=" + (Team_4_STN < 0 ? Team_4_STN.ToString("D4") : Team_4_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_1_STN=" + (Donor_1_STN < 0 ? Donor_1_STN.ToString("D4") : Donor_1_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_2_STN=" + (Donor_2_STN < 0 ? Donor_2_STN.ToString("D4") : Donor_2_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_3_STN=" + (Donor_3_STN < 0 ? Donor_3_STN.ToString("D4") : Donor_3_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_4_STN=" + (Donor_4_STN < 0 ? Donor_4_STN.ToString("D4") : Donor_4_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_5_STN=" + (Donor_5_STN < 0 ? Donor_5_STN.ToString("D4") : Donor_5_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_6_STN=" + (Donor_6_STN < 0 ? Donor_6_STN.ToString("D4") : Donor_6_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_7_STN=" + (Donor_7_STN < 0 ? Donor_7_STN.ToString("D4") : Donor_7_STN.ToString("D5")));
                sb.AppendLine("FILE_" + FileID + "_DONOR_8_STN=" + (Donor_8_STN < 0 ? Donor_8_STN.ToString("D4") : Donor_8_STN.ToString("D5")));

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
                sb.AppendLine("     Link-16 File " + FileID + ":");
                sb.AppendLine("        Voice Group A Channel: " + GroupAChannel);
                sb.AppendLine("        Voice Group B Channel: " + GroupBChannel);
                sb.AppendLine("        Mission Channel: " + MissionChannel);
                sb.AppendLine("        Fighter Channel: " + FighterChannel);
                sb.AppendLine("        Special Channel: " + SpecialChannel);
                sb.AppendLine("        Callsign: " + Callsign);
                sb.AppendLine("        Callsign Number: " + CallsignID);
                sb.AppendLine("        Flight Lead: " + IsFlightLead);
                sb.AppendLine("        Ext Time Reference: " + ExternalTimeReference);
                sb.AppendLine("        TACAN Channel: " + TACANChannel);
                sb.AppendLine("        TACAN Band: " + TACANBand);
                sb.AppendLine("        Flight 1 STN: " + Flight_1_STN);
                sb.AppendLine("        Flight 2 STN: " + Flight_2_STN);
                sb.AppendLine("        Flight 3 STN: " + Flight_3_STN);
                sb.AppendLine("        Flight 4 STN: " + Flight_4_STN);
                sb.AppendLine("        Team 1 STN: " + Team_1_STN);
                sb.AppendLine("        Team 2 STN: " + Team_2_STN);
                sb.AppendLine("        Team 3 STN: " + Team_3_STN);
                sb.AppendLine("        Team 4 STN: " + Team_4_STN);
                sb.AppendLine("        Donor 1 STN: " + Donor_1_STN);
                sb.AppendLine("        Donor 2 STN: " + Donor_2_STN);
                sb.AppendLine("        Donor 3 STN: " + Donor_3_STN);
                sb.AppendLine("        Donor 4 STN: " + Donor_4_STN);
                sb.AppendLine("        Donor 5 STN: " + Donor_5_STN);
                sb.AppendLine("        Donor 6 STN: " + Donor_6_STN);
                sb.AppendLine("        Donor 7 STN: " + Donor_7_STN);
                sb.AppendLine("        Donor 8 STN: " + Donor_8_STN);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(Link16File? other)
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

                if (other is not Link16File comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + GroupAChannel.GetHashCode();
                    hash = hash * 5483 + GroupBChannel.GetHashCode();
                    hash = hash * 5483 + MissionChannel.GetHashCode();
                    hash = hash * 5483 + FighterChannel.GetHashCode();
                    hash = hash * 5483 + SpecialChannel.GetHashCode();
                    hash = hash * 5483 + Callsign.GetHashCode();
                    hash = hash * 5483 + CallsignID.GetHashCode();
                    hash = hash * 5483 + IsFlightLead.GetHashCode();
                    hash = hash * 5483 + ExternalTimeReference.GetHashCode();
                    hash = hash * 5483 + TACANChannel.GetHashCode();
                    hash = hash * 5483 + TACANBand.GetHashCode();
                    hash = hash * 5483 + Flight_1_STN.GetHashCode();
                    hash = hash * 5483 + Flight_2_STN.GetHashCode();
                    hash = hash * 5483 + Flight_3_STN.GetHashCode();
                    hash = hash * 5483 + Flight_4_STN.GetHashCode();
                    hash = hash * 5483 + Team_1_STN.GetHashCode();
                    hash = hash * 5483 + Team_2_STN.GetHashCode();
                    hash = hash * 5483 + Team_3_STN.GetHashCode();
                    hash = hash * 5483 + Team_4_STN.GetHashCode();
                    hash = hash * 5483 + Donor_1_STN.GetHashCode();
                    hash = hash * 5483 + Donor_2_STN.GetHashCode();
                    hash = hash * 5483 + Donor_3_STN.GetHashCode();
                    hash = hash * 5483 + Donor_4_STN.GetHashCode();
                    hash = hash * 5483 + Donor_5_STN.GetHashCode();
                    hash = hash * 5483 + Donor_6_STN.GetHashCode();
                    hash = hash * 5483 + Donor_7_STN.GetHashCode();
                    hash = hash * 5483 + Donor_8_STN.GetHashCode();
                    hash = hash * 5483 + FileID.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(Link16File comparator1, Link16File comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(Link16File comparator1, Link16File comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="Link16File"/> object
            /// </summary>
            public Link16File() { }
            /// <summary>
            /// Initializes an instance of the <see cref="Link16File"/> objecy using the values in <paramref name="file"/>
            /// </summary>
            /// <param name="file">The <see cref="Link16File"/> object with the values to copy</param>
            public Link16File(Link16File file)
            {
                GroupAChannel = file.GroupAChannel;
                GroupBChannel = file.GroupBChannel;
                MissionChannel = file.MissionChannel;
                FighterChannel = file.FighterChannel;
                SpecialChannel = file.SpecialChannel;
                Callsign = file.Callsign;
                CallsignID = file.CallsignID;
                IsFlightLead = file.IsFlightLead;
                ExternalTimeReference = file.ExternalTimeReference;
                TACANChannel = file.TACANChannel;
                TACANBand = file.TACANBand;
                Flight_1_STN = file.Flight_1_STN;
                Flight_2_STN = file.Flight_2_STN;
                Flight_3_STN = file.Flight_3_STN;
                Flight_4_STN = file.Flight_4_STN;
                Team_1_STN = file.Team_1_STN;
                Team_2_STN = file.Team_2_STN;
                Team_3_STN = file.Team_3_STN;
                Team_4_STN = file.Team_4_STN;
                Donor_1_STN = file.Donor_1_STN;
                Donor_2_STN = file.Donor_2_STN;
                Donor_3_STN = file.Donor_3_STN;
                Donor_4_STN = file.Donor_4_STN;
                Donor_5_STN = file.Donor_5_STN;
                Donor_6_STN = file.Donor_6_STN;
                Donor_7_STN = file.Donor_7_STN;
                Donor_8_STN = file.Donor_8_STN;
                FileID = file.FileID;

            }
            /// <summary>
            /// Initializes an instance of the <see cref="Link16File"/> object with the supplied values
            /// </summary>
            /// <param name="groupAChannel">Link-16 Channel used for Primary Voice Communications</param>
            /// <param name="groupBChannel">Link-16 Channel used for Secondary Voice Communications</param>
            /// <param name="missionChannel">Link-16 Channel used for Mission Link-16 Tracks</param>
            /// <param name="fighterChannel">Link-16 Channel used for Fighter Link-16 Tracks</param>
            /// <param name="specialChannel">Link-16 Channel used for Special Link-16 Tracks</param>
            /// <param name="callsign">Callsign associated with this Link-16 Participant</param>
            /// <param name="isFlightLead">Identifies if this Link-16 Participant is a Flight Lead</param>
            /// <param name="externalTimeReference">When <see langword="true"/>, uses an External Time Reference</param>
            /// <param name="tacanChannel">Link-16 Participant TACAN Channel</param>
            /// <param name="tacanBand">Link-16 Participant TACAN Band</param>
            /// <param name="flight_1_STN">Flight Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="flight_2_STN">Flight Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="flight_3_STN">Flight Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="flight_4_STN">Flight Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="team_1_STN">Team Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="team_2_STN">Team Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="team_3_STN">Team Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="team_4_STN">Team Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_1_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_2_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_3_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_4_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_5_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_6_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_7_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="donor_8_STN">Station ID of a Link-16 Participant to include in Track Processing</param>
            /// <param name="fileID">Identifies if this is File A or File B</param>
            public Link16File(int groupAChannel, int groupBChannel, int missionChannel,
                int fighterChannel, int specialChannel, string callsign, int callsignID, bool isFlightLead,
                bool externalTimeReference, int tacanChannel, TACANBand tacanBand,
                int flight_1_STN, int flight_2_STN, int flight_3_STN, int flight_4_STN,
                int team_1_STN, int team_2_STN, int team_3_STN, int team_4_STN,
                int donor_1_STN, int donor_2_STN, int donor_3_STN, int donor_4_STN,
                int donor_5_STN, int donor_6_STN, int donor_7_STN, int donor_8_STN, char fileID)
            {
                GroupAChannel = groupAChannel;
                GroupBChannel = groupBChannel;
                MissionChannel = missionChannel;
                FighterChannel = fighterChannel;
                SpecialChannel = specialChannel;
                Callsign = callsign;
                CallsignID = callsignID;
                IsFlightLead = isFlightLead;
                ExternalTimeReference = externalTimeReference;
                TACANChannel = tacanChannel;
                TACANBand = tacanBand;
                Flight_1_STN = flight_1_STN;
                Flight_2_STN = flight_2_STN;
                Flight_3_STN = flight_3_STN;
                Flight_4_STN = flight_4_STN;
                Team_1_STN = team_1_STN;
                Team_2_STN = team_2_STN;
                Team_3_STN = team_3_STN;
                Team_4_STN = team_4_STN;
                Donor_1_STN = donor_1_STN;
                Donor_2_STN = donor_2_STN;
                Donor_3_STN = donor_3_STN;
                Donor_4_STN = donor_4_STN;
                Donor_5_STN = donor_5_STN;
                Donor_6_STN = donor_6_STN;
                Donor_7_STN = donor_7_STN;
                Donor_8_STN = donor_8_STN;
                FileID = fileID;
            }


            #endregion Constructors
        }
        #endregion Sub Classes
    }
}
