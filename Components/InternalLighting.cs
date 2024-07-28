using FalconDataCartridge.Enums;
using System.Collections;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the Internal Lighting Section of an INI File
    /// </summary>
    public class InternalLighting : INIComponent, IEquatable<InternalLighting>
    {
        #region Properties      

        /// <summary>
        /// Collection of Lighting Settings amd their respective Switch Values
        /// </summary>
        public Dictionary<string, int> LightSettings { get => lightSettings; set => lightSettings = value; }
        #endregion Properties // Checked

        #region Fields 
        private Dictionary<string, int> lightSettings = [];
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

                // Find the [Hud] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();

                // Lighting
                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    string[] delims = ["="];
                    string[] PGM = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    lightSettings.Add(PGM[0], int.Parse(PGM[1]));
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
            foreach (KeyValuePair<string, int> d in lightSettings)
                sb.AppendLine(d.Key + "=" + d.Value);
            return sb.ToString();
        }

        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine("**************** Internal Lighting Configuration ***************");
            foreach (KeyValuePair<string, int> d in lightSettings)
                sb.AppendLine("   " + d.Key + "=" + d.Value);

            return sb.ToString();

        }

        
        #region Equality Functions
        public bool Equals(InternalLighting? other)
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

            if (other is not InternalLighting comparator)
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
                foreach (KeyValuePair<string, int> d in lightSettings)
                {
                    hash = hash * 5483 + d.Key.GetHashCode();
                    hash = hash * 5483 + d.Value.GetHashCode();
                }                   
                return hash;
            }
        }
        public static bool operator ==(InternalLighting comparator1, InternalLighting comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(InternalLighting comparator1, InternalLighting comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="InternalLighting"/> object
        /// </summary>
        public InternalLighting()
        {
            SectionFlag = "[INT_LIGHTING]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="InternalLighting"/> object with the values in <paramref name="config"/>
        /// </summary>
        /// <param name="config">The <see cref="InternalLighting"/> object with the values to copy</param>
        public InternalLighting(InternalLighting config)
        {
            SectionFlag = "[INT_LIGHTING]";
            foreach (KeyValuePair<string, int> b in config.lightSettings)
                lightSettings.Add(b.Key, b.Value);
            
        }
        
        /// <summary>
        /// Initializes an instance of the <see cref="InternalLighting"/> object with the data in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data</param>
        public InternalLighting(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors

    }
}
