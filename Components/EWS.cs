using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Class to interact with the EWS Data in the DTC.
    /// </summary>
    public class EWS : INIComponent, IEquatable<EWS>
    {

        #region Properties
        /// <summary>
        /// <see langword="false"/> for automatic release, <see langword="true"/> if release requires user interaction.
        /// </summary>
        public bool RequestCounter {  get => regCtr; set => regCtr = value; }        
        /// <summary>
        /// <para>When set to <see langword="true"/>, enables both the “LOW” VMU message used to indicate that an 
        /// expendable has reached the bingo quantity and the “OUT” VMU message used 
        /// to indicate that an expendable is depleted.</para> 
        /// </summary>
        public bool BingoNotification { get => bingo; set => bingo = value; }        
        /// <summary>
        /// When <see langword="true"/>, enables the “CHAFF FLARE” VMU message, used to indicate that an expendable program has been initiated.
        /// </summary>
        public bool Feedback { get => feedback; set => feedback = value; }       
        /// <summary>
        /// <para>Low Flare Threshold for EWS to notify Pilot that flares are low.</para>
        /// <para>Bingo quantity can be set to any value between 0 and 99.</para>
        /// </summary>
        public int FlareBingo
        {
            get => flareBingo; 
            set
            {
                if (value < 1) flareBingo = 0;
                else { flareBingo = Math.Min(value, 99); }
            }
        }
        /// <summary>
        /// <para>Low Chaff Threshold for EWS to notify Pilot that chaff is low.</para>
        /// <para>Bingo quantity can be set to any value between 0 and 99.</para>
        /// </summary>
        public int ChaffBingo
        {
            get => chaffBingo; 
            set
            {
                if (value < 1) chaffBingo = 0;
                else { chaffBingo = Math.Min(value, 99); }
            }
        }
        /// <summary>
        /// When <see langword="true"/>, EWS will Request activation of the Jammer when a threat is detected.
        /// </summary>
        public bool RequestJammer { get => reqJammer; set => reqJammer = value; }       
        /// <summary>
        /// Collection of Programs that can be loaded into the EWS from the DTC.
        /// </summary>
        public Collection<EWSProgram> Programs { get => programs; set => programs = value; } // Note: Mutable List

        #endregion Properties // Checked

        #region Fields
        private bool regCtr = false;
        private bool bingo = false;
        private bool feedback = false;
        private int flareBingo = 0;
        private int chaffBingo = 0;
        private bool reqJammer = false;
        private Collection<EWSProgram> programs = [];
        #endregion Fields

        #region Helper Methods     
        /// <summary>
        /// Reads the string provided in <paramref name="data"/> and attempts to parse the text for the data for this component.
        /// </summary>
        /// <param name="data"><see cref="string"/> data to be parsed.</param>
        /// <returns></returns>
        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);
            
            try
            {

                // Check if the string is empty or does not contain the [EWS] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [EWS] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();
                int val = 0;

                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    if (s.StartsWith("REQCTR"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        regCtr = Convert.ToBoolean(val);
                        continue;
                    }
                    else if (s.StartsWith("BINGO"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        bingo = Convert.ToBoolean(val);
                        continue;
                    }
                    else if (s.StartsWith("FDBK"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        feedback = Convert.ToBoolean(val);
                        continue;
                    }
                    else if (s.StartsWith("REQJAM"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        reqJammer = Convert.ToBoolean(val);
                        continue;
                    }
                    else if (s.StartsWith("FLARE BINGO"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        flareBingo = val;
                        continue;
                    }
                    else if (s.StartsWith("CHAFF BINGO"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        chaffBingo = val;
                        continue;
                    }
                    else
                    {
                        string[] delims = [" ", "="];
                        string[] vals = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                        int id = int.Parse(vals[1]);                       
                        if (vals.Length == 5)
                        {
                            _ = int.TryParse(vals[4], out val);

                            switch (vals[2], vals[3])
                            {
                                case ("CHAFF", "BQ"):
                                    programs[id].ChaffBurstCount = val;
                                    break;
                                case ("CHAFF", "BI"):
                                    programs[id].ChaffBurstInterval = val;
                                    break;
                                case ("CHAFF", "SQ"):
                                    programs[id].ChaffSequenceCount = val;
                                    break;
                                case ("CHAFF", "SI"):
                                    programs[id].ChaffSequenceInterval = val;
                                    break;
                                case ("FLARE", "BQ"):
                                    programs[id].FlareBurstCount = val;
                                    break;
                                case ("FLARE", "BI"):
                                    programs[id].FlareBurstInterval = val;
                                    break;
                                case ("FLARE", "SQ"):
                                    programs[id].FlareSequenceCount = val;
                                    break;
                                case ("FLARE", "SI"):
                                    programs[id].FlareSequenceInterval = val;
                                    break;
                            }
                        }
                        else if (vals.Length == 4) programs[id].Comment = vals[3];
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
        /// <summary>
        /// Formats the object for output to a file.
        /// </summary>
        /// <returns></returns>
        internal override string Write()
        {
            if (!_IncludeInOutput)
                return string.Empty;
            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            _ = sb.AppendLine("Reqctr=" + Convert.ToInt16(RequestCounter));
            _ = sb.AppendLine("Bingo=" + Convert.ToInt16(BingoNotification));
            _ = sb.AppendLine("Fdbk=" + Convert.ToInt16(Feedback));
            _ = sb.AppendLine("Flare Bingo=" + FlareBingo);
            _ = sb.AppendLine("Chaff Bingo=" + ChaffBingo);
            for (int i = 0; i < programs.Count; i++)
            {
                _ = sb.Append(programs[i].Write(i, false));
            }
            _ = sb.AppendLine("Reqjam=" + Convert.ToInt16(RequestJammer));
            for (int i = 0; i < Programs.Count; i++)
            {
                _ = sb.Append(programs[i].Write(i, true));
            }

            return sb.ToString();
        }
        
        #endregion Helper Methods

        #region Functional Methods   
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("*********************** EWS Configuration **********************");
            sb.AppendLine("     Request Counter Enabled: " + regCtr);
            sb.AppendLine("     Bingo Notification Enabled: " + bingo);
            sb.AppendLine("     Feedback Enabled: " + feedback);
            sb.AppendLine("     Flare Bingo: " + flareBingo);
            sb.AppendLine("     Chaff Bingo: " + chaffBingo);
            sb.AppendLine("     Request Jammer Enabled: " + reqJammer);
            for (int i = 0; i < programs.Count; i++)
            {
                sb.AppendLine("     EWS Program " + (i + 1) + ":" + Environment.NewLine);
                sb.Append(programs[i].ToString());
            }

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(EWS? other)
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

            if (other is not EWS comparator)
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
                hash = hash * 5483 + regCtr.GetHashCode();
                hash = hash * 5483 + bingo.GetHashCode();
                hash = hash * 5483 + feedback.GetHashCode();
                hash = hash * 5483 + flareBingo.GetHashCode();
                hash = hash * 5483 + chaffBingo.GetHashCode();
                hash = hash * 5483 + reqJammer.GetHashCode();
                if (programs is not null)
                    if (programs.Count != 0)
                        foreach (var program in programs)
                            hash ^= program.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(EWS comparator1, EWS comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(EWS comparator1, EWS comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Instantiates an empty <see cref="EWS"/>.
        /// </summary>
        public EWS()
        {
            SectionFlag = "[EWS]";
            for (int i = 0; i < 6; i++)
                programs.Add(new EWSProgram());
        }
        /// <summary>
        /// Instantiates a new <see cref="EWS"/> using the same values in <paramref name="ews"/>.
        /// </summary>
        /// <param name="ews">An <see cref="EWS"/> with existing data.</param>
        public EWS(EWS ews)
        {
            SectionFlag = "[EWS]";
            regCtr = ews.RequestCounter;
            bingo = ews.BingoNotification;
            feedback = ews.Feedback;
            flareBingo = ews.FlareBingo;
            chaffBingo = ews.ChaffBingo;
            reqJammer = ews.RequestJammer;
            foreach (EWSProgram p in ews.programs)
                programs.Add(new(p));
        }
        /// <summary>
        /// Instantiates a partially populated <see cref="EWS"/> using the values supplied; Programs is initialized with a list of default <see cref="EWSProgram"/> items.
        /// </summary>
        /// <param name="reqCtr">0 for automatic release, 1 if release requires user interaction.</param>
        /// <param name="bingo"><para>When set to 1, enables both the “LOW” VMU message used to indicate that an 
        /// expendable has reached the bingo quantity and the “OUT” VMU message used 
        /// to indicate that an expendable is depleted.</para>
        /// </param>
        /// <param name="feedback">When set to 1, enables the “CHAFF FLARE” VMU message, used to indicate that an expendable program has been initiated.</param>
        /// <param name="flareBingo"><para>Low Flare Threshold for EWS to notify Pilot that flares are low.</para>
        /// <para>Bingo quantity can be set to any value between 0 and 99.</para></param>
        /// <param name="chaffBingo"><para>Low Chaff Threshold for EWS to notify Pilot that chaff is low.</para>
        /// <para>Bingo quantity can be set to any value between 0 and 99.</para></param>
        /// <param name="reqJammer">When set to 1, EWS will enable the Jammer when the program is executed.</param>
        public EWS(bool reqCtr, bool bingo, bool feedback, int flareBingo, int chaffBingo, bool reqJammer, ICollection<EWSProgram> programs)
        {
            SectionFlag = "[EWS]";
            regCtr = reqCtr;
            this.bingo = bingo;
            this.feedback = feedback;
            this.flareBingo = flareBingo;
            this.chaffBingo = chaffBingo;
            this.reqJammer = reqJammer;
            foreach (EWSProgram p in programs)
                this.programs.Add(new(p));

        }
        /// <summary>
        /// <para>Attempts to Initialize an <see cref="EWS"/> object using the data contained in the supplied <see cref="string"/> object.</para>
        /// <para>If the provided Text does not contain the required data, a <see cref="EWS"/> object with default values is returned.</para>
        /// </summary>
        /// <param name="initializationData">A <see cref="string"/> containing Text that can be parsed to instantiate a <see cref="EWS"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public EWS(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

        #region Sub-Classes
        /// <summary>
        /// Class to interact with pre-defined <see cref="EWS"/> programs .
        /// </summary>
        public class EWSProgram
        {
            #region Properties
            /// <summary>
            /// <para>Number of release pulses in a single burst.</para>
            /// <para>Burst Count must be between 0 and 99</para>
            /// </summary>
            public int FlareBurstCount
            {
                get => flareBurstCount; 
                set
                {
                    if (value < 1) flareBurstCount = 0;
                    else { flareBurstCount = Math.Min(value, 99); }
                }
            }
            /// <summary>
            /// <para>Milliseconds between release pules in a single burst.</para>
            /// <para>Burst Interval must be between 20 and 10000</para>
            /// </summary>
            public int FlareBurstInterval
            {
                get => flareBurstInterval; 
                set
                {
                    if (value < 20)
                        flareBurstInterval = 0;
                    else { flareBurstInterval = Math.Min(value, 10000); }
                }
            }
            /// <summary>
            /// <para>Number of bursts to execute.</para>
            /// <para>Sequence Count must be between 0 and 99</para>
            /// </summary>
            public int FlareSequenceCount
            {
                get => flareSequenceCount; 
                set
                {
                    if (value < 1) flareSequenceCount = 0;
                    else { flareSequenceCount = Math.Min(value, 99); }
                }
            }
            /// <summary>
            /// <para>Milliseconds between bursts.</para>
            /// <para>Sequence Interval must be between 20 and 10000</para>
            /// </summary>
            public int FlareSequenceInterval
            {
                get => flareSequenceInterval; 
                set
                {
                    if (value < 20)
                        flareSequenceInterval = 0;
                    else { flareSequenceInterval = Math.Min(value, 10000); }
                }
            }
            /// <summary>
            /// <para>Number of release pulses in a single burst.</para>
            /// <para>Burst Count must be between 0 and 99</para>
            /// </summary>
            public int ChaffBurstCount
            {
                get => chaffBurstCount; 
                set
                {
                    if (value < 1) chaffBurstCount = 0;
                    else { chaffBurstCount = Math.Min(value, 99); }
                }
            }
            /// <summary>
            /// <para>Milliseconds between release pules in a single burst.</para>
            /// <para>Burst Interval must be between 20 and 10000</para>
            /// </summary>
            public int ChaffBurstInterval
            {
                get => chaffBurstInterval; 
                set
                {
                    if (value < 20)
                        chaffBurstInterval = 0;
                    else { chaffBurstInterval = Math.Min(value, 10000); }
                }
            }
            /// <summary>
            /// <para>Number of bursts to execute.</para>
            /// <para>Sequence Count must be between 0 and 99</para>
            /// </summary>
            public int ChaffSequenceCount
            {
                get => chaffSequenceCount; 
                set
                {
                    if (value < 1) chaffSequenceCount = 0;
                    else { chaffSequenceCount = Math.Min(value, 99); }
                }
            }
            /// <summary>
            /// <para>Milliseconds between bursts.</para>
            /// <para>Sequence Interval must be between 20 and 10000</para>
            /// </summary>
            public int ChaffSequenceInterval
            {
                get => chaffSequenceInterval; 
                set
                {
                    if (value < 20)
                        chaffSequenceInterval = 0;
                    else { chaffSequenceInterval = Math.Min(value, 10000); }
                }
            }
            /// <summary>
            /// Comments for the DTC View.
            /// </summary>
            public string Comment { get; set; } = "";
            #endregion Properties // Checked

            #region Fields
            private int flareBurstCount = 0;
            private int flareBurstInterval = 0;
            private int flareSequenceCount = 0;
            private int flareSequenceInterval = 0;
            private int chaffBurstCount = 0;
            private int chaffBurstInterval = 0;
            private int chaffSequenceCount = 0;
            private int chaffSequenceInterval = 0;

            #endregion Fields

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write(int programID, bool commentsOnly)
            {
                StringBuilder sb = new();
                if (!commentsOnly)
                {
                    _ = sb.AppendFormat("PGM {0} Chaff BQ={1}" + Environment.NewLine, programID, ChaffBurstCount);
                    _ = sb.AppendFormat("PGM {0} Chaff BI={1}" + Environment.NewLine, programID, ChaffBurstInterval);
                    _ = sb.AppendFormat("PGM {0} Chaff SQ={1}" + Environment.NewLine, programID, ChaffSequenceCount);
                    _ = sb.AppendFormat("PGM {0} Chaff SI={1}" + Environment.NewLine, programID, ChaffSequenceInterval);
                    _ = sb.AppendFormat("PGM {0} Flare BQ={1}" + Environment.NewLine, programID, FlareBurstCount);
                    _ = sb.AppendFormat("PGM {0} Flare BI={1}" + Environment.NewLine, programID, FlareBurstInterval);
                    _ = sb.AppendFormat("PGM {0} Flare SQ={1}" + Environment.NewLine, programID, FlareSequenceCount);
                    _ = sb.AppendFormat("PGM {0} Flare SI={1}" + Environment.NewLine, programID, FlareSequenceInterval);
                }
                else
                {
                    _ = sb.AppendFormat("PGM {0} Comment={1}" + Environment.NewLine, programID, Comment);
                }

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

                sb.AppendLine("   Chaff BQ: " + ChaffBurstCount);
                sb.AppendLine("   Chaff BI: " + ChaffBurstInterval);
                sb.AppendLine("   Chaff SQ: " + ChaffSequenceCount);
                sb.AppendLine("   Chaff SI: " + ChaffSequenceInterval);
                sb.AppendLine("   Flare BQ: " + FlareBurstCount);
                sb.AppendLine("   Flare BI: " + FlareBurstInterval);
                sb.AppendLine("   Flare SQ: " + FlareSequenceCount);
                sb.AppendLine("   Flare SI: " + FlareSequenceInterval);
                sb.AppendLine("   Comment: " + Comment);

                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(EWSProgram? other)
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

                if (other is not EWSProgram comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                int hash = 2539;
                unchecked
                {                    
                    hash = hash *5483 +flareBurstCount;
                    hash = hash *5483 +flareBurstInterval;
                    hash = hash *5483 +flareSequenceCount;
                    hash = hash *5483 +flareSequenceInterval;
                    hash = hash *5483 +chaffBurstCount;
                    hash = hash *5483 +chaffBurstInterval;
                    hash = hash *5483 +chaffSequenceCount;
                    hash = hash *5483 +chaffSequenceInterval;
                    hash = hash *5483 +Comment.GetHashCode();
                }
                return hash;
            }
            public static bool operator ==(EWSProgram comparator1, EWSProgram comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(EWSProgram comparator1, EWSProgram comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion

            #region Constructors
            /// <summary>
            /// Instantiates an empty <see cref="EWSProgram"/>.
            /// </summary>
            public EWSProgram() { }
            /// <summary>
            /// Instantiates a new <see cref="EWSProgram"/> using the same values in <paramref name="ewsProgram"/>.
            /// </summary>
            /// <param name="ewsProgram">An <see cref="EWSProgram"/> with existing data.</param>
            public EWSProgram(EWSProgram ewsProgram)
            {
                flareBurstCount = ewsProgram.FlareBurstCount;
                flareBurstInterval = ewsProgram.FlareBurstInterval;
                flareSequenceCount = ewsProgram.FlareSequenceCount;
                flareSequenceInterval = ewsProgram.FlareSequenceInterval;
                chaffBurstCount = ewsProgram.ChaffBurstCount;
                chaffBurstInterval = ewsProgram.ChaffBurstInterval;
                chaffSequenceCount = ewsProgram.ChaffSequenceCount;
                chaffSequenceInterval = ewsProgram.ChaffSequenceInterval;
                Comment = ewsProgram.Comment;
            }
            /// <summary>
            /// Instantiates an <see cref="EWSProgram"/> using the values supplied.
            /// </summary>
            /// <param name="falreBurstQuantity"><para>Number of release pulses in a single burst.</para>
            /// <para>Burst Count must be between 0 and 99</para></param>
            /// <param name="flareBurstInterval"><para>Milliseconds between release pules in a single burst.</para>
            /// <para>Burst Interval must be between 20 and 10000</para></param>
            /// <param name="flareSequenceQuantity"><para>Number of bursts to execute.</para>
            /// <para>Sequence Count must be between 0 and 99</para></param>
            /// <param name="flareSequenceInterval"><para>Milliseconds between bursts.</para>
            /// <para>Sequence Interval must be between 20 and 10000</para></param>
            /// <param name="chaffBurstQuantity"><para>Number of release pulses in a single burst.</para>
            /// <para>Burst Count must be between 0 and 99</para></param>
            /// <param name="chaffBurstInterval"><para>Milliseconds between release pules in a single burst.</para>
            /// <para>Burst Interval must be between 20 and 10000</para></param>
            /// <param name="cahaffSequenceQuantity"><para>Number of bursts to execute.</para>
            /// <para>Sequence Count must be between 0 and 99</para></param>
            /// <param name="chaffSequenceInterval"><para>Milliseconds between bursts.</para>
            /// <para>Sequence Interval must be between 20 and 10000</para></param>
            /// <param name="comment">Comments for the DTC View.</param>
            public EWSProgram(int falreBurstQuantity, int flareBurstInterval, int flareSequenceQuantity, int flareSequenceInterval, int chaffBurstQuantity, int chaffBurstInterval, int cahaffSequenceQuantity, int chaffSequenceInterval, string comment)
            {
                flareBurstCount = falreBurstQuantity;
                this.flareBurstInterval = flareBurstInterval;
                flareSequenceCount = flareSequenceQuantity;
                this.flareSequenceInterval = flareSequenceInterval;
                chaffBurstCount = chaffBurstQuantity;
                this.chaffBurstInterval = chaffBurstInterval;
                chaffSequenceCount = cahaffSequenceQuantity;
                this.chaffSequenceInterval = chaffSequenceInterval;
                Comment = comment;
            }
            #endregion Constructors

        }
        #endregion Sub-Classes	

    }
}
