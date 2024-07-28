using System;
using System.IO;
using System.Text;

namespace FalconDataCartridge.Components
{
    public class Bullseye : INIComponent, IEquatable<Bullseye>
    {
        #region Properties
        /// <summary>
        /// Displays the Bullseye on the MFD Screens when set to 1
        /// </summary>
        public bool ShowBullseye { get => bullseye; set => bullseye = value; }
       
        #endregion Properties

        #region Fields
        private bool bullseye = false;
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
                    if (s is not null)
                        bullseye = Convert.ToBoolean(int.Parse(s.AsSpan(s.Length - 1)));
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
            sb.AppendLine("BullseyeInfoOnMFD=" + Convert.ToInt16(bullseye));

            return sb.ToString();
        }
        #endregion Helper Methods

        #region Functional Methods        
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("*************************** Bullseye ***************************");
            sb.AppendLine("Bullseye Displayed on MFD: " + bullseye);

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(Bullseye? other)
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

            if (other is not Bullseye comparator)
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
                hash = hash * 5483 + ShowBullseye.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(Bullseye comparator1, Bullseye comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(Bullseye comparator1, Bullseye comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="Bullseye"/> object with default values
        /// </summary>
        public Bullseye()
        {
            SectionFlag = "[Bullseye]";
        }
        /// <summary>
        /// Initializes a new <see cref="Bullseye"/> object with the values in <paramref name="bullseye"/>
        /// </summary>
        /// <param name="bullseye"><see cref="Bullseye"/> object with the values to copy from</param>
        public Bullseye(Bullseye bullseye)
        {
            SectionFlag = "[Bullseye]";
            this.bullseye = bullseye.bullseye;
        }
        /// <summary>
        /// Initializes a new <see cref="Bullseye"/> object with the supplied values
        /// </summary>
        /// <param name="bullseye">Indicates if the Bullseye should be displayed on the MFD Pages</param>
        public Bullseye(bool bullseye)
        {
            SectionFlag = "[Bullseye]";
            this.bullseye = bullseye;
        }
        /// <summary>
        /// Initializes a new <see cref="Bullseye"/> object with the data contained in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"></param>
        public Bullseye(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

    }
}
