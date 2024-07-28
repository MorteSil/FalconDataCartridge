using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the Laser Section of an INI File
    /// </summary>
    public class Laser : INIComponent, IEquatable<Laser>
    {
        #region Properties

        /// <summary>
        /// Number of seconds prior to weapon impact the Laser automatically engages
        /// </summary>
        public int LaserTiming { get => laserTimer; set => laserTimer = value; }       
        /// <summary>
        /// Modulation Code used for the Laser in TGP Mode
        /// </summary>
        public int LaserTGP { get => laserTGP; set => laserTGP = value; }       
        /// <summary>
        /// Modulation Code used for the Laser in LST Mode
        /// </summary>
        public int LaserLST { get => laserLST; set => laserLST = value; }        

        #endregion Properties // Checked

        #region Fields       
        private int laserTimer = 8;
        private int laserTGP = 1520;
        private int laserLST = 1520;

        #endregion Fields

        #region Helper Methods     

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {

                // Check if the string is empty or does not contain the [Laser] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [Laser] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();
                double i = 0;

                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    _ = double.TryParse(s.AsSpan(s.IndexOf('=') + 1), out i);

                    if (s.StartsWith("LASERST"))
                    {
                        laserTimer = Convert.ToInt32(i);
                        continue;
                    }
                    else if (s.StartsWith("LASERTGP"))
                    {
                        laserTGP = Convert.ToInt32(i);
                        continue;
                    }
                    else if (s.StartsWith("LASERLST"))
                    {
                        laserLST = Convert.ToInt32(i);
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
            _ = sb.AppendLine("LaserST=" + laserTimer);
            _ = sb.AppendLine("LaserTGP=" + laserTGP);
            _ = sb.AppendLine("LaserLST=" + laserLST);

            return sb.ToString();
        }
        #endregion Helper Methods        

        #region Functional Methods       

        public override string ToString()
        {

            StringBuilder sb = new();
            sb.AppendLine("******************** Laser Configuration ***********************");
            sb.AppendLine("     LaserST: " + laserTimer);
            sb.AppendLine("     LaserTGP: " + laserTGP);
            sb.AppendLine("     LaserLST: " + laserLST);

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(Laser? other)
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

            if (other is not Laser comparator)
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
                hash = hash * 5483 + laserTimer.GetHashCode();
                hash = hash * 5483 + laserTGP.GetHashCode();
                hash = hash * 5483 + laserLST.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(Laser comparator1, Laser comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(Laser comparator1, Laser comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Instantiates an empty <see cref="Laser"/> object
        /// </summary>
        public Laser()
        {
            SectionFlag = "[Laser]";
        }
        /// <summary>
        /// Instantiates a new <see cref="Laser"/> object using the vaules in <paramref name="laser"/>
        /// </summary>
        /// <param name="icp"></param>
        public Laser(Laser laser)
        {
            SectionFlag = "[Laser]";
            laserTimer = laser.laserTimer;
            laserTGP = laser.laserTGP;
            laserLST = laser.laserLST;
        }
        /// <summary>
        /// Initializes a new <see cref="Laser"/>object using the supplied values
        /// </summary>
        /// <param name="laserTimer">Number of seconds prior to impact for the Laser to engage</param>
        /// <param name="lstCode">Modulation COde for the Laser in LST Mode</param>
        /// <param name="tgpCode">Modulation Code for the Laser in TGP Mode</param>
        public Laser(int laserTimer, int tgpCode, int lstCode)
        {
            SectionFlag = "[Laser]";
            this.laserTimer = laserTimer;
            laserTGP = tgpCode;
            laserLST = lstCode;
        }
        /// <summary>
        /// Instantiates a new <see cref="Laser"/> object with the data supplied in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object containing initialization data for an <see cref="Laser"/>object</param>
        public Laser(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }


        #endregion Constructors
    }
}
