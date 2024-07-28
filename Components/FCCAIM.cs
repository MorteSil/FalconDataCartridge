using FalconDataCartridge.Enums;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the FCC_AIM Section of an INI File
    /// </summary>
    public class FCCAIM : INIComponent, IEquatable<FCCAIM>
    {
        #region Properties
        /// <summary>
        /// Selected Search mode for the AIM-9 Seeker Head
        /// </summary>
        public AIM9_SearchMode SearchMode { get => searchMode; set => searchMode = value; }       
        /// <summary>
        /// Selected Threshold Detection mode for the AIM-9 Seeker Head
        /// </summary>
        public AIM9_ThresholdMode ThresholdDetectionMode { get => thresholdDetectionMode; set => thresholdDetectionMode = value; }       
        /// <summary>
        /// Default Target Size passed to the AIM-120 Targeting Computer
        /// </summary>
        public AIM120_TargetSize TargetSize {  get => targetSize; set => targetSize = value; }        
        #endregion Properties // Checked

        #region Fields
        private AIM9_SearchMode searchMode = AIM9_SearchMode.Scan;
        private AIM9_ThresholdMode thresholdDetectionMode = AIM9_ThresholdMode.TD;
        private AIM120_TargetSize targetSize = AIM120_TargetSize.Unknown;

        #endregion Fields

        #region Helper Methods           
        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [FCC_AIM] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [FCC_AIM] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();
                                
                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    if (s.Contains("Spot"))
                        searchMode = (AIM9_SearchMode)int.Parse(s[(s.IndexOf('=') + 1)..]);
                    if (s.Contains("TD"))
                        thresholdDetectionMode = (AIM9_ThresholdMode)int.Parse(s[(s.IndexOf('=') + 1)..]);
                    if (s.Contains("AIM120"))
                        targetSize = (AIM120_TargetSize)int.Parse(s[(s.IndexOf('=') + 1)..]);
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
            sb.AppendLine("AIM-9_Spot/Scan=" + (int)SearchMode);
            sb.AppendLine("AIM-9_TD/BP=" + (int)ThresholdDetectionMode);
            sb.AppendLine("AIM120_TargetSize=" + (int)TargetSize);
            return sb.ToString();
        }
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("************** FCC Air Intercept Mode Configuration ************");
            sb.AppendLine("     AIM-9 Spot/Scan: " + SearchMode);
            sb.AppendLine("     AIM-9 TD/BP: " + ThresholdDetectionMode);
            sb.AppendLine("     AIM-120 TargetSize: " + TargetSize);

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(FCCAIM? other)
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

            if (other is not FCCAIM comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + searchMode.GetHashCode();
                hash = hash * 5483 + thresholdDetectionMode.GetHashCode();
                hash = hash * 5483 + targetSize.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(FCCAIM comparator1, FCCAIM comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(FCCAIM comparator1, FCCAIM comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="FCCAIM"/> object
        /// </summary>
        public FCCAIM()
        {
            SectionFlag = "[FCC_AIM]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="FCCAIM"/> object using the values contained in <paramref name="fcc"/>
        /// </summary>
        /// <param name="fcc">The <see cref="FCCAIM"/> object with the values to copy</param>
        public FCCAIM(FCCAIM fcc)
        {
            SectionFlag = "[FCC_AIM]";
            searchMode = fcc.searchMode;
            thresholdDetectionMode = fcc.thresholdDetectionMode;
            targetSize = fcc.targetSize;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FCCAIM"/> object with the supplies values
        /// </summary>
        /// <param name="searchMode"><para>The Seeker Search Mode</para>
        /// <para>0: Spot, 1: Scan</para></param>
        /// <param name="thresholdMode"><para>The Seeker Threshold Detection Mode</para>
        /// <para>0: Threshold Detection, 1: Bypass</para></param>
        /// <param name="targetSize">The default target size to send to the AIM-120 Targeting Computer</param>
        public FCCAIM(int searchMode, int thresholdMode, int targetSize)
        {
            SectionFlag = "[FCC_AIM]";
            this.searchMode = (AIM9_SearchMode)searchMode;
            thresholdDetectionMode = (AIM9_ThresholdMode)thresholdMode;
            this.targetSize = (AIM120_TargetSize)targetSize;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FCCAIM"/> object with the supplies values
        /// </summary>
        /// <param name="searchMode"><para>The Seeker Search Mode</para></param> 
        /// <param name="thresholdMode"><para>The Seeker Threshold Detection Mode</para></param>
        /// <param name="targetSize">The default target size to send to the AIM-120 Targeting Computer</param>
        public FCCAIM(AIM9_SearchMode searchMode, AIM9_ThresholdMode thresholdMode, AIM120_TargetSize targetSize)
        {
            SectionFlag = "[FCC_AIM]";
            this.searchMode = searchMode;
            thresholdDetectionMode = thresholdMode;
            this.targetSize = targetSize;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FCCAIM"/> object with the values supplied in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization values</param>
        public FCCAIM(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

    }
}
