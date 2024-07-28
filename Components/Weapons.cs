using FalconDataCartridge.Enums;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the Weapons Section of an INI File
    /// </summary>
    public class Weapons : INIComponent, IEquatable<Weapons>
    {
        #region Properties
        /// <summary>
        /// Sets the default Master Arm Mode when entering the 3D World.
        /// </summary>
        public MasterArmSetting MasterArm { get => masterArmSetting; set => masterArmSetting = value; }       
        #endregion Properties

        #region Fields
        private MasterArmSetting masterArmSetting = MasterArmSetting.OFF;
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

                // Weapons
                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out int val);

                    if (s.StartsWith("MASTERARM"))
                    {
                        masterArmSetting = (MasterArmSetting)val;
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
            sb.AppendLine("MasterArm=" + (int)masterArmSetting);
            return sb.ToString();
        }

        #endregion Helper Methods

        #region Functional Methods        

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("******************** Weapons Configuration *********************");
            sb.AppendLine("     MasterArm: " + masterArmSetting);

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(Weapons? other)
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

            if (other is not Weapons comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + masterArmSetting.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(Weapons comparator1, Weapons comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(Weapons comparator1, Weapons comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="Weapons"/> object
        /// </summary>
        public Weapons()
        {
            SectionFlag = "[Weapons]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="Weapons"/> object using the values in <paramref name="weapons"/>
        /// </summary>
        /// <param name="weapons">The <see cref="Weapons"/></param> object to copy the values from
        public Weapons(Weapons weapons)
        {
            SectionFlag = "[Weapons]";
            masterArmSetting = weapons.masterArmSetting;
        }
        /// <summary>
        /// Initializes a new <see cref="Weapons"/> object using the supplied values
        /// </summary>
        /// <param name="weaponSetting">Sets the Default Master Arm Switch setting in the 3D World.</param>
        public Weapons(MasterArmSetting weaponSetting)
        {
            SectionFlag = "[Weapons]";
            masterArmSetting = weaponSetting;
        }
        /// <summary>
        /// Initializes an instance of the <see cref="Weapons"/> object with the data in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data</param>
        public Weapons(string initializationData) : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors
    }
}
