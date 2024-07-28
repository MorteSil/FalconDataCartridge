using FalconDataCartridge.Components;
using FalconDataCartridge.Enums;
using System.Text;

namespace FalconDataCartridge.Files
{
    public class TEMission : AppFile, IEquatable<TEMission>
    {
        #region Properties
        /// <summary>
        /// TE Mission Title
        /// </summary>
        public MissionName MissionName { get => mission; set => mission = value; }
        /// <summary>
        /// Settings for the DTC Steerpoint Page
        /// </summary>
        public STPT SteerpointConfig { get => steerpointConfig; set => steerpointConfig = value; }

        public override bool IsDefaultInitialization => throw new NotImplementedException();
        #endregion Properties

        #region Fields

        private MissionName mission = new();
        private STPT steerpointConfig = new();

        #endregion Fields        

        #region Helper Methods

        protected override bool Read(string data)
        {
            try
            {
                mission = new MissionName(data);
                steerpointConfig = new STPT(data);
            }
            catch (Exception ex)
            {
                Utilities.Logging.ErrorLog.CreateLogFile(ex, "This error occurred while attempting to load " + _FileType + " from the following string:\n" + data);
                if (ex is IOException)
                    return false;
                throw;
            }
            return true;
        }

        protected override bool Read(byte[] data)
        {
            if (data is null)
                return false;

            string? dataString = data.ToString();

            if (!string.IsNullOrEmpty(dataString))
                return Read(dataString.ToString());

            return false;
        }

        protected override byte[] Write()
        {
            StringBuilder sb = new();
            sb.Append(mission.Write());
            sb.Append(steerpointConfig.Write());
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(mission.ToString());
            sb.Append(steerpointConfig.ToString());
            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(TEMission? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;

            return other.ToString() == ToString() && other.GetHashCode() == GetHashCode();
        }
        public override bool Equals(object? other)
        {
            if (other == null)
                return false;

            if (other is not TEMission comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + mission.GetHashCode();
                hash = hash * 5483 + steerpointConfig.GetHashCode();
                return hash;
            }
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="TEMission"/> object
        /// </summary>
        public TEMission()
        {
            _FileType = ApplicationFileType.TEMissionINI;
            _StreamType = FileStreamType.Text;
        }
        /// <summary>
        /// Initializes an instance of the <see cref="TEMission"/> object using the values in <paramref name="temission"/>.
        /// </summary>
        /// <param name="temission">The <see cref="TEMission"/> object with the values to copy.</param>
        public TEMission(TEMission temission)
            : this()
        {
            mission = new(temission.mission);
            steerpointConfig = new(temission.steerpointConfig);
            _InitialHash = GetHashCode();
        }
        /// <summary>
        /// <para>Initializes an instance of the <see cref="TEMission"/> object.</para>
        /// <para>Accepts a File Path as a valid input and checks if the file exists and has valid <see cref="TEMission"/> parameters.</para>
        /// <para>If the <see cref="string"/> object is not a File Path, the function will attempt to parse the supplied <see cref="string"/> for <see cref="TEMission"/> Initialization Data.</para>
        /// </summary>
        /// <param name="data">Can be either a valid path to an existing DTC.ini file or the contents of a DTC.ini file</param>
        public TEMission(string path)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(path);
            if (File.Exists(path))
                Load(path);
            else
                throw new FileNotFoundException(path);
        }
        
        #endregion Constructors
    }
}
