using FalconDataCartridge.Enums;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the HUD Configuration settigns in an INI File
    /// </summary>
    public class HUD : INIComponent, IEquatable<HUD>
    {
        #region Properties
        /// <summary>
        /// HUD Vertical Speed Cockpit Setting
        /// </summary>
        public HUDScales Scales {  get => scales; set => scales = value; }        
        /// <summary>
        /// HUD Brightness Cockpit Setting
        /// </summary>
        public int Brightness {  get => brightness; set => brightness = value; }       
        /// <summary>
        /// FPM Cockpit Setting
        /// </summary>
        public HUDFPM FPM {  get => fpm; set => fpm = value; }       
        /// <summary>
        /// VVI Indicator Cockpit Setting
        /// </summary>
        public HUDVelocityType VelocityIndicator {  get => velocityIndicator; set => velocityIndicator = value; }       
        /// <summary>
        /// Altitude Indicator Cockpit Setting
        /// </summary>
        public HUDAltitude AltitudeIndicator { get => altitudeIndicator; set => altitudeIndicator = value; }       
        /// <summary>
        /// Sym Wheel Position
        /// </summary>
        public int SymWheel {  get => symWheel; set => symWheel = value; }       
        /// <summary>
        /// DED HUD Cockpit Setting
        /// </summary>
        public HUDDED DED {  get => ded; set => ded = value; }       

        #endregion Properties // Checked

        #region Fields
        private HUDScales scales = HUDScales.OFF;
        private int brightness = 1;
        private HUDFPM fpm = HUDFPM.OFF;
        private HUDVelocityType velocityIndicator = HUDVelocityType.CAS;
        private HUDAltitude altitudeIndicator = HUDAltitude.Auto;
        private int symWheel = 0;
        private HUDDED ded = HUDDED.OFF;
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

                // Hud
                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out int val);

                    if (s.StartsWith("SCALES"))
                    {
                        scales = (HUDScales)val;
                        continue;
                    }
                    else if (s.StartsWith("BRIGHTNESS"))
                    {
                        brightness = val;
                        continue;
                    }
                    else if (s.StartsWith("FPM"))
                    {
                        fpm = (HUDFPM)val;
                        continue;
                    }
                    else if (s.StartsWith("VELOCITY"))
                    {
                        velocityIndicator = (HUDVelocityType)val;
                        continue;
                    }
                    else if (s.StartsWith("ALT"))
                    {
                        altitudeIndicator = (HUDAltitude)val;
                        continue;
                    }
                    else if (s.StartsWith("SYMWHEELPOS"))
                    {
                        symWheel = val;
                        continue;
                    }
                    else if (s.StartsWith("DED"))
                    {
                        ded = (HUDDED)val;
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
            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            sb.AppendLine("Scales=" + (int)scales);
            sb.AppendLine("Brightness=" + brightness);
            sb.AppendLine("FPM=" + (int)fpm);
            sb.AppendLine("Velocity=" + (int)velocityIndicator);
            sb.AppendLine("Alt=" + (int)altitudeIndicator);
            sb.AppendLine("SymWheelPos=" + symWheel);
            sb.AppendLine("DED=" + (int)ded);

            return sb.ToString();
        }

        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("*********************** Hud Configuration **********************");
            sb.AppendLine("     Scales: " + scales);
            sb.AppendLine("     Brightness: " + brightness);
            sb.AppendLine("     FPM: " + fpm);
            sb.AppendLine("     Velocity: " + velocityIndicator);
            sb.AppendLine("     Alt: " + altitudeIndicator);
            sb.AppendLine("     Sym Wheel Pos: " + symWheel);
            sb.AppendLine("     DED: " + ded);

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(HUD? other)
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

            if (other is not HUD comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + scales.GetHashCode();
                hash = hash * 5483 + brightness.GetHashCode();
                hash = hash * 5483 + fpm.GetHashCode();
                hash = hash * 5483 + velocityIndicator.GetHashCode();
                hash = hash * 5483 + symWheel.GetHashCode();
                hash = hash * 5483 + ded.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(HUD comparator1, HUD comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(HUD comparator1, HUD comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="HUD"/> object
        /// </summary>
        public HUD()
        {
            SectionFlag = "[Hud]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="HUD"/> object using the values in <paramref name="hud"/>
        /// </summary>
        /// <param name="hud">The <see cref="hud"/></param> object to copy the values from
        public HUD(HUD hud)
        {
            SectionFlag = "[Hud]";
            scales = hud.scales;
            brightness = hud.brightness;
            fpm = hud.fpm;
            velocityIndicator = hud.velocityIndicator;
            altitudeIndicator = hud.altitudeIndicator;
            symWheel = hud.symWheel;
            ded = hud.ded;
        }
        /// <summary>
        /// Initializes a new <see cref="HUD"/> object using the supplied values
        /// </summary>
        /// <param name="scales">Velocity Indicator Cockpit Setting</param>
        /// <param name="brightness">HUD Brightness</param>
        /// <param name="fpm">Flight Path Marker Setting</param>
        /// <param name="velocityIndicator">VVI Indicator Setting</param>
        /// <param name="altitudeIndicator">Altitude Indicator Setting</param>
        /// <param name="symWheel">Sym Wheel Position</param>
        /// <param name="ded">DED Switch Setting</param>
        public HUD(HUDScales scales, int brightness, HUDFPM fpm, HUDVelocityType velocityIndicator, HUDAltitude altitudeIndicator, int symWheel, HUDDED ded)
        {
            SectionFlag = "[Hud]";
            this.scales = scales;
            this.brightness = brightness;
            this.fpm = fpm;
            this.velocityIndicator = velocityIndicator;
            this.altitudeIndicator = altitudeIndicator;
            this.symWheel = symWheel;
            this.ded = ded;
        }
        /// <summary>
        /// Initializes an instance of the <see cref="HUD"/> object with the data in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data</param>
        public HUD(string initializationData) : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors
    }
}
