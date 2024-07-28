using FalconDataCartridge.Enums;
using System.Text;

namespace FalconDataCartridge.Components
{
    public class COMMS : INIComponent, IEquatable<COMMS>
    {
        #region Properties

        /// <summary>
        /// Default Preset to load into Comm Radio 1.
        /// </summary>
        public uint Comm1Default { get => comm1Default; set => comm1Default = value; }        
        /// <summary>
        /// Default Preset to load into Comm Radio 2.
        /// </summary>
        public uint Comm2Default { get => comm2Default; set => comm2Default = value; }       
        /// <summary>
        /// Comment for Default Preset to load into Comm Radio 1 (Not Used).
        /// </summary>
        public string Comm1Comment { get => comm1Comment; set => comm1Comment = value; }       
        /// <summary>
        /// Comment for Default Preset to load into Comm Radio 2 (Not Used).
        /// </summary>
        public string Comm2Comment { get => comm2Comment; set => comm2Comment = value; }       
        /// <summary>
        /// <para>Assignable TACAN Channel.</para>
        /// <para>Valid Range: 1-126</para>
        /// </summary>
        public uint TACANChannel { get => tacanChannel; set => tacanChannel = Math.Min(126, Math.Max(1, value)); }       
        /// <summary>
        /// <para>TACAN ChannelConfiguration.</para>
        /// <para>0: X, 1: Y</para>
        /// </summary>
        public TACANBand TACANBand { get => tacanBand; set => tacanBand = value; }       
        /// <summary>
        /// <para>TACAN Mode Selection</para>
        /// <para>0: A-A, 1: A-G</para>
        /// </summary>
        public TACANMode TACANMode { get => tacanMode; set => tacanMode = value; }       
        /// <summary>
        /// <para>Preset ILS Frequency in MHz.</para>
        /// <para>Valid Range: 108.000-117.975 MHz in increments of 0.025</para>
        /// <para>Frequencies that do not adhere to 25 kHz spacing will be rounded down to the next lowest channel.</para>
        /// </summary>
        public double ILSFrequency
        {
            get => ilsFrequency; 
            set
            {
                if (value == 0)
                {
                    ilsFrequency = 0;
                    return;
                }
                double val = Math.Round(value * 1000, 3);
                // Must be increments of 25KHz
                while (val % 25 != 0) val--;
                val /= 1000;
                ilsFrequency = Math.Max(108.000, Math.Min(117.975, val));

            }
        }
        /// <summary>
        /// <para>Preset ILS Cours Setting.</para>
        /// <para>Valid Range: 1.0-360.0</para>
        /// </summary>
        public double ILSCourse { get => ilsCourse; set => ilsCourse = Math.Min(360, Math.Max(1, Math.Round(value, 1))); }
        #endregion Properties

        #region Fields
        private uint comm1Default = 1;
        private uint comm2Default = 1;
        private string comm1Comment = "";
        private string comm2Comment = "";
        private uint tacanChannel = 1;
        private TACANBand tacanBand = TACANBand.Y;
        private TACANMode tacanMode = TACANMode.AA;
        private double ilsFrequency = 108.000;
        private double ilsCourse = 0;
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

                    double val;
                    if (dataLine.Length == 3)
                    {
                        _ = double.TryParse(dataLine[2], out val);
                        _ = int.TryParse(dataLine[1], out int id);

                        switch (dataLine[0].ToUpper())
                        {
                            case "COMM1":
                                comm1Comment = dataLine[2];
                                break;
                            case "COMM2":
                                comm2Comment = dataLine[2];
                                break;
                        }
                    }
                    else if (dataLine.Length == 2)
                    {
                        _ = double.TryParse(dataLine[1], out val);

                        switch (dataLine[0].ToUpper())
                        {
                            case "COMM1":
                                comm1Default = Convert.ToUInt32(val);
                                break;
                            case "COMM2":
                                comm2Default = Convert.ToUInt32(val);
                                break;
                            case "TACAN CHANNEL":
                                tacanChannel = Convert.ToUInt32(val);
                                break;
                            case "TACAN BAND":
                                tacanBand = (TACANBand)val;
                                break;
                            case "TACAN DOMAIN":
                                tacanMode = (TACANMode)val;
                                break;
                            case "ILS FREQUENCY":
                                // Non-Standard to address Inconsistent BMS Behavior (Stored in Hecto Hz vs Kilo Hz)
                                ilsFrequency = val / 100;
                                break;
                            case "ILS CRS":
                                ilsCourse = val;
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
            sb.AppendLine("Comm1=" + comm1Default);
            sb.AppendLine("Comm1_Comment=" + comm1Comment);
            sb.AppendLine("Comm2=" + comm2Default);
            sb.AppendLine("Comm2_Comment=" + comm2Comment);
            sb.AppendLine("TACAN Channel=" + tacanChannel);
            sb.AppendLine("TACAN Band=" + (int)tacanBand);
            sb.AppendLine("TACAN Domain=" + (int)tacanMode);
            sb.AppendLine("ILS Frequency=" + ILSFrequency * 100);
            sb.AppendLine("ILS CRS=" + ILSCourse);
            return sb.ToString();
        }
        #endregion Helper Methods

        #region Functional Methods        
        public override string ToString()
        {

            StringBuilder sb = new();
            sb.AppendLine("********************** Comms Configuration *********************");
            sb.AppendLine("     Comm1: " + comm1Default);
            sb.AppendLine("     Comm1_Comment: " + comm1Comment);
            sb.AppendLine("     Comm2: " + comm2Default);
            sb.AppendLine("     Comm2_Comment: " + comm2Comment);
            sb.AppendLine("     TACAN Channel: " + tacanChannel);
            sb.AppendLine("     TACAN Band: " + tacanBand);
            sb.AppendLine("     TACAN Domain: " + tacanMode);
            sb.AppendLine("     ILS Frequency: " + ilsFrequency);
            sb.AppendLine("     ILS CRS: " + ilsCourse);
            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(COMMS? other)
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

            if (other is not COMMS comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + comm1Default.GetHashCode();
                hash = hash * 5483 + comm2Default.GetHashCode();
                hash = hash * 5483 + comm1Comment.GetHashCode();
                hash = hash * 5483 + comm2Comment.GetHashCode();
                hash = hash * 5483 + tacanChannel.GetHashCode();
                hash = hash * 5483 + tacanBand.GetHashCode();
                hash = hash * 5483 + tacanMode.GetHashCode();
                hash = hash * 5483 + ilsFrequency.GetHashCode();
                hash = hash * 5483 + ilsCourse.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(COMMS comparator1, COMMS comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(COMMS comparator1, COMMS comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="COMMS"/> object
        /// </summary>
        public COMMS()
        {
            SectionFlag = "[COMMS]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="COMMS"/> object with the values in <paramref name="config"/>
        /// </summary>
        /// <param name="config">The <see cref="COMMS"/> object with the values to copy</param>
        public COMMS(COMMS config)
        {
            SectionFlag = "[COMMS]";
            comm1Default = config.comm1Default;
            comm1Comment = config.comm1Comment;
            comm2Default = config.comm2Default;
            comm2Comment = config.comm2Comment;
            tacanChannel = config.tacanChannel;
            tacanBand = config.tacanBand;
            tacanMode = config.tacanMode;
            ilsFrequency = config.ilsFrequency;
            ilsCourse = config.ilsCourse;
        }
        /// <summary>
        /// Initializes a new <see cref="COMMS"/> object with the supplied values
        /// </summary>
        /// <param name="comm1Default">Default Channel for Comm Radio 1</param>
        /// <param name="comm2Default">Default Channel for Comm Radio 2</param>
        /// <param name="tacanChannel">TACAN Channel</param>
        /// <param name="tacanBand">TACAN Band</param>
        /// <param name="tacanMode">TACAN Mode</param>
        /// <param name="ils">ILS Frequency in MHz</param>
        /// <param name="course">ILS Course</param>
        public COMMS(uint comm1Default, uint comm2Default, uint tacanChannel, uint tacanBand, uint tacanMode, double ils, uint course)
        {
            SectionFlag = "[COMMS]";
            this.comm1Default = comm1Default;
            this.comm2Default = comm2Default;
            comm1Comment = "(open)";
            comm2Comment = "(open)";
            this.tacanChannel = tacanChannel;
            this.tacanBand = (TACANBand)tacanBand;
            this.tacanMode = (TACANMode)tacanMode;
            ilsFrequency = ils;
            ilsCourse = course;
        }
        /// <summary>
        /// Initializes a new <see cref="COMMS"/> object with the supplied values
        /// </summary>
        /// <param name="comm1Default">Default Channel for Comm Radio 1</param>
        /// <param name="comm2Default">Default Channel for Comm Radio 2</param>
        /// <param name="tacanChannel">TACAN Channel</param>
        /// <param name="tacanBand">TACAN Band</param>
        /// <param name="tacanMode">TACAN Mode</param>
        /// <param name="ils">ILS Frequency in MHz</param>
        /// <param name="course">ILS Course</param>
        public COMMS(uint comm1Default, uint comm2Default, uint tacanChannel, TACANBand tacanBand, TACANMode tacanMode, double ils, double course, string comm1Comment, string comm2Comment)
        {
            SectionFlag = "[COMMS]";
            this.comm1Default = comm1Default;
            this.comm2Default = comm2Default;
            this.tacanChannel = tacanChannel;
            this.tacanBand = tacanBand;
            this.tacanMode = tacanMode;
            ilsFrequency = ils;
            ilsCourse = course;
            this.comm1Comment = comm1Comment;
            this.comm2Comment = comm2Comment;
        }
        /// <summary>
        /// Initializes a new <see cref="COMMS"/> object with the data contained in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data for this object</param>
        public COMMS(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors


    }
}
