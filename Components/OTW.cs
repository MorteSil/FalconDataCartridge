using FalconDataCartridge.Enums;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the OTW Section of an INI File
    /// </summary>
    public class OTW : INIComponent, IEquatable<OTW>
    {
        #region Properties
        /// <summary>
        /// Default 3D View
        /// </summary>
        public OTWViewSetting OTWSetting { get => otwViewSetting; set => otwViewSetting = value; }       
        #endregion Properties // Checked

        #region Fields
        private OTWViewSetting otwViewSetting = OTWViewSetting.Pit3D;
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

                // OTW
                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out int val);

                    if (s.StartsWith("MODE"))
                    {
                        otwViewSetting = (OTWViewSetting)val;
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
            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            sb.AppendLine("Mode=" + (int)otwViewSetting);
            return sb.ToString();
        }

        #endregion Helper Methods

        #region Functional Methods
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("*********************** OTW Configuration **********************");
            sb.AppendLine("     Mode: " + otwViewSetting);
            return sb.ToString();
        }

       
        #region Equality Functions
        public bool Equals(OTW? other)
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

            if (other is not OTW comparator)
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
                hash = hash * 5483 + otwViewSetting.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(OTW comparator1, OTW comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(OTW comparator1, OTW comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="OTW"/> object
        /// </summary>
        public OTW()
        {
            SectionFlag = "[OTW]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="OTW"/> object with the values in <paramref name="config"/>
        /// </summary>
        /// <param name="config">The <see cref="OTW"/> object with the values to copy</param>
        public OTW(OTW config)
        {
            SectionFlag = "[OTW]";
            otwViewSetting = config.otwViewSetting;
        }
        /// <summary>
        /// Initializes a <see cref="OTW"/> object with the supplied values
        /// </summary>
        /// <param name="otw">Sets the default 3D World View</param>
        public OTW(OTWViewSetting otw)
        {
            SectionFlag = "[OTW]";
            otwViewSetting = otw;
        }
        /// <summary>
        /// Initializes an instance of the <see cref="OTW"/> object with the data in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data</param>
        public OTW(string initializationData) : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors
    }
}
