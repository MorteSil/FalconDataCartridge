using System.Text;

namespace FalconDataCartridge.Components
{
    public class MissionName : INIComponent, IEquatable<MissionName>
    {
        #region Properties
        /// <summary>
        /// Conatains the Mission Name for a TE or Mission
        /// </summary>
        public string Name { get => name; set => name = value; }      

        #endregion Properties

        #region Fields
        private string name = "";
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

                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer                
                    if (s is not null)
                        name = s.Substring(s.IndexOf('=') + 1);
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
            sb.AppendLine("title=" + name);

            return sb.ToString();
        }

        #endregion Helper Methods

        #region Functional Methods        
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("************************* Mission Name *************************");
            sb.AppendLine("Mission Name: " + name);

            return sb.ToString();
        }

        

        #region Equality Functions
        public bool Equals(MissionName? other)
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

            if (other is not MissionName comparator)
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
                hash = hash * 5483 + name.GetHashCode();
                return hash;
            }
        }

        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="MissionName"/> object with default values
        /// </summary>
        public MissionName()
        {
            SectionFlag = "[MISSION]";
        }
        /// <summary>
        /// Initializes a new <see cref="MissionName"/> object with the supplied values
        /// </summary>
        /// <param name="name">Indicates if the Bullseye should be displayed on the MFD Pages</param>
        public MissionName(MissionName name)
        {
            SectionFlag = "[MISSION]";
            this.name = name.name;
        }
        /// <summary>
        /// <para>Initializes a new <see cref="MissionName"/> object with the data contained in <paramref name="initializationData"/>.</para>
        /// <para>This can be used to parse Initialization data from a large INI file or create a new instance directly from a supplied <see cref="string"/>. 
        /// If <paramref name="initializationData"/> does not contain the [MISSION] section tag, the <see cref="string"/> is treated as a direct initializer.</para>
        /// </summary>
        /// <param name="initializationData"></param>
        public MissionName(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            if (!initializationData.Contains(SectionFlag))
                initializationData = SectionFlag + Environment.NewLine + initializationData;
            Read(initializationData);
        }
        #endregion Constructors
    }
}
