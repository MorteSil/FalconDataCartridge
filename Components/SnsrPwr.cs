using FalconDataCartridge.Enums;
using System.Data;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// The SNSR_PWR Section of an INI File
    /// </summary>
    public class SnsrPwr : INIComponent, IEquatable<SnsrPwr>
    {
        #region Properties      
        /// <summary>
        /// Collection of Sensor Power Settings amd their respective Switch Values
        /// </summary>
        public Dictionary<string, int> SensorSettings { get => sensorSettings; set => sensorSettings = value; }
        #endregion Properties // Checked

        #region Fields 
        private Dictionary<string, int> sensorSettings = [];
        #endregion Fields

        #region Helper Methods

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [Hud] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [SNSR_PWR] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();

                // SNSR
                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    string[] delims = ["="];
                    string[] PGM = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    sensorSettings.Add(PGM[0], int.Parse(PGM[1]));
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
            foreach (KeyValuePair<string, int> d in sensorSettings)
                sb.AppendLine(d.Key + "=" + d.Value);
            return sb.ToString();
        }
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();


            sb.AppendLine("****************** Sensor Power Configuration ******************");
            foreach (KeyValuePair<string, int> d in sensorSettings)
                sb.AppendLine("   " + d.Key + "=" + d.Value);

            return sb.ToString();

        }

        #region Equality Functions
        public bool Equals(SnsrPwr? other)
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

            if (other is not SnsrPwr comparator)
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
                foreach (KeyValuePair<string, int> d in sensorSettings)
                {
                    hash = hash * 5483 + d.Key.GetHashCode();
                    hash = hash * 5483 + d.Value.GetHashCode();
                }
                return hash;
            }
        }
        public static bool operator ==(SnsrPwr comparator1, SnsrPwr comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(SnsrPwr comparator1, SnsrPwr comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="SnsrPwr"/> object
        /// </summary>
        public SnsrPwr()
        {
            SectionFlag = "[SNSR_PWR]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="SnsrPwr"/> object with the values in <paramref name="config"/>
        /// </summary>
        /// <param name="config">The <see cref="SnsrPwr"/> object with the values to copy</param>
        public SnsrPwr(SnsrPwr config)
        {
            SectionFlag = "[SNSR_PWR]";
            foreach (KeyValuePair<string, int> k in config.sensorSettings)
                sensorSettings.Add(k.Key, k.Value);
        }
        
        /// <summary>
        /// Initializes an instance of the <see cref="SnsrPwr"/> object with the data in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data</param>
        public SnsrPwr(string initializationData) : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors


    }
}
