using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the FCC_AGM Section of a DTC File
    /// </summary>
    public class FCCAGM : INIComponent, IEquatable<FCCAGM>
    {
        #region Properties
        /// <summary>
        /// Enables Auto-Power On for the AGM-65 Maverick Missile
        /// </summary>
        public bool AutoPowerOn { get => autoPowerOn; set => autoPowerOn = value; }        
        /// <summary>
        /// Waypoint at which the AGM-65 Maverick will automatically Power On
        /// </summary>
        public int PowerOnWaypoint { get => powerOnWaypoint; set => powerOnWaypoint = value; }       
        /// <summary>
        /// Relative Direction to PowerOnWaypoint where the AGM-64 Maverick will Automatically Power On
        /// </summary>
        public Utilities.GeoLib.CardinalDirection Direction {  get => direction; set => direction = value; }       
        #endregion Properties // Checked

        #region Fields
        private bool autoPowerOn = false;
        private int powerOnWaypoint = 0;
        private Utilities.GeoLib.CardinalDirection direction = Utilities.GeoLib.CardinalDirection.North;

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

                    if (s.Contains("AutoPwrWpt"))
                        powerOnWaypoint = int.Parse(s[(s.IndexOf('=') + 1)..]);
                    else if (s.Contains("AutoPwrDir"))
                        direction = (Utilities.GeoLib.CardinalDirection)int.Parse(s[(s.IndexOf('=') + 1)..]);
                    else if (s.Contains("AutoPwr"))
                        autoPowerOn = Convert.ToBoolean(int.Parse(s[(s.IndexOf('=') + 1)..]));

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
            sb.AppendLine("Maverick_AutoPwr=" + (autoPowerOn ? 1 : 0));
            sb.AppendLine("Maverick_AutoPwrDir=" + (int)direction);
            sb.AppendLine("Maverick_AutoPwrWpt=" + powerOnWaypoint);
            return sb.ToString();
        }

        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("************ FCC Air-to-Ground Missile Configuration ***********");
            sb.AppendLine("     Maverick AutoPwr: " + (AutoPowerOn ? 1 : 0));
            sb.AppendLine("     Maverick AutoPwrDir: " + Direction);
            sb.AppendLine("     Maverick AutoPwrWpt: " + PowerOnWaypoint);

            return sb.ToString();
        }
                
        #region Equality Functions
        public bool Equals(FCCAGM? other)
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

            if (other is not FCCAGM comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + autoPowerOn.GetHashCode();
                hash = hash * 5483 + powerOnWaypoint.GetHashCode();
                hash = hash * 5483 + direction.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(FCCAGM comparator1, FCCAGM comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(FCCAGM comparator1, FCCAGM comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="FCCAGM"/> object
        /// </summary>
        public FCCAGM()
        {
            SectionFlag = "[FCC_AGM]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="FCCAGM"/> object using the values contained in <paramref name="fcc"/>
        /// </summary>
        /// <param name="fcc">The <see cref="FCCAGM"/> object with the values to copy</param>
        public FCCAGM(FCCAGM fcc)
        {
            SectionFlag = "[FCC_AGM]";
            autoPowerOn = fcc.autoPowerOn;
            powerOnWaypoint = fcc.powerOnWaypoint;
            direction = fcc.direction;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FCCAGM"/> object with the supplies values
        /// </summary>
        /// <param name="autoPowerOnEnabled">Enables Auto-Power On for the AGM-65 Maverick Missile</param>                      
        /// <param name="autoPowerOnWaypoint">Waypoint at which the AGM-65 Maverick will automatically Power On</param>
        /// <param name="autoPowerOnDirection">Relative Direction to PowerOnWaypoint where the AGM-64 Maverick will Automatically Power On</param>  
        public FCCAGM(bool autoPowerOnEnabled, int autoPowerOnWaypoint, int autoPowerOnDirection)
        {
            SectionFlag = "[FCC_AGM]";
            autoPowerOn = autoPowerOnEnabled;
            powerOnWaypoint = autoPowerOnWaypoint;
            direction = (Utilities.GeoLib.CardinalDirection)autoPowerOnDirection;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FCCAGM"/> object with the supplied values
        /// </summary>
        /// <param name="autoPowerOnEnabled">Enables Auto-Power On for the AGM-65 Maverick Missile</param>                      
        /// <param name="autoPowerOnWaypoint">Waypoint at which the AGM-65 Maverick will automatically Power On</param>
        /// <param name="autoPowerOnDirection">Relative Direction to PowerOnWaypoint where the AGM-64 Maverick will Automatically Power On</param>  
        public FCCAGM(bool autoPowerOnEnabled, int autoPowerOnWaypoint, Utilities.GeoLib.CardinalDirection autoPowerOnDirection)
        {            
            autoPowerOn = autoPowerOnEnabled;
            powerOnWaypoint = autoPowerOnWaypoint;
            direction = autoPowerOnDirection;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FCCAGM"/> object with the values supplied in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization values</param>
        public FCCAGM(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors
    }
}
