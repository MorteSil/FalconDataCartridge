using FalconDataCartridge.Components;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace FalconDataCartridge.Files
{
    /// <summary>
    /// Represents a complete DTC .ini File
    /// </summary>
    public class DTC : AppFile, IEquatable<DTC>
    {
        #region Properties
        /// <summary>
        /// Settings for the DTC EWS Page
        /// </summary>
        public EWS EWSConfig { get => ewsConfig; set => ewsConfig = value; }
        /// <summary>
        /// Settings for the DTC MFD Page
        /// </summary>
        public MFD MFDConfig { get => mfdConfig; set => mfdConfig = value; }
        /// <summary>
        /// Bullseye Setting in a DTC File
        /// </summary>
        public Bullseye Bullseye { get => bullseye; set => bullseye = value; }
        /// <summary>
        /// Settings for the DTC IFF Page
        /// </summary>
        public IFF IFFConfig { get => iffConfig; set => iffConfig = value; }
        /// <summary>
        /// Settings for the DTC HARM Page
        /// </summary>
        public HARM HarmConfig { get => harmConfig; set => harmConfig = value; }
        /// <summary>
        /// Settings for the DTC Steerpoint Page
        /// </summary>
        public STPT SteerpointConfig { get => steerpointConfig; set => steerpointConfig = value; }
        /// <summary>
        /// Settings for the DTC Radio Page
        /// </summary>
        public Radio RadioConfig { get => radioConfig; set => radioConfig = value; }
        /// <summary>
        /// Comm Panel Settings in a DTC File
        /// </summary>
        public COMMS CommConfig { get => commsConfig; set => commsConfig = value; }
        /// <summary>
        /// Settings for the Map stored in the DTC
        /// </summary>
        public MapOptions MapConfig { get => mapConfig; set => mapConfig = value; }
        /// <summary>
        /// Settings for the DTC Nav Offset Page
        /// </summary>
        public NavOffset NavOffset { get => navOffset; set => navOffset = value; }
        /// <summary>
        /// Settings for the DTC ICP Configuration Page
        /// </summary>
        public ICP ICPConfig { get => icpConfig; set => icpConfig = value; }
        /// <summary>
        /// Laser Configuration in a DTC
        /// </summary>
        public Laser LaserConfig { get => laserConfig; set => laserConfig = value; }
        /// <summary>
        /// FCC_AIM Configuration in a DTC File.
        /// </summary>
        public FCCAIM AIM { get => aimConfig; set => aimConfig = value; }
        /// <summary>
        /// FCC_AGM Configuration in a DTC File
        /// </summary>
        public FCCAGM AGM { get => agmConfig; set => agmConfig = value; }
        /// <summary>
        /// FCC_AGB Configuraiton in a DTC File
        /// </summary>
        public FCCAGB AGB { get => agbConfig; set => agbConfig = value; }
        /// <summary>
        /// HUD Configuration Settings in a DTC File
        /// </summary>
        public HUD HUDConfig { get => hud; set => hud = value; }
        /// <summary>
        /// Cockpit View Settings in a DTC File.
        /// </summary>
        public CockpitView CockpitView { get => cockpitView; set => cockpitView = value; }
        /// <summary>
        /// Default 3D View Setting in a DTC File
        /// </summary>
        public OTW OTWConfig { get => otw; set => otw = value; }
        /// <summary>
        /// Weapon Settings stored in a DTC File.
        /// </summary>
        public Weapons WeaponConfig { get => weapons; set => weapons = value; }
        /// <summary>
        /// Sensor Power Settings stored in a DTC File
        /// </summary>
        public SnsrPwr SensorPower { get => sensorPower; set => sensorPower = value; }
        /// <summary>
        /// Internal Lighting Settings of a DTC File
        /// </summary>
        public InternalLighting Lighting { get => internalLighting; set => internalLighting = value; }
        /// <summary>
        /// Settings for the DTC Link-16 Page
        /// </summary>
        public Link16 Link16Config { get => link16Config; set => link16Config = value; }
        /// <summary>
        /// Settings for the DTC Nav Offset Page
        /// </summary>
        public MFDColor MFDColors { get => mfdColors; set => mfdColors = value; }
        public override bool IsDefaultInitialization
        {
            get => false;
        }


        #endregion Properties

        #region Fields

        private EWS ewsConfig = new();
        private MFD mfdConfig = new();
        private Bullseye bullseye = new();
        private IFF iffConfig = new();
        private HARM harmConfig = new();
        private STPT steerpointConfig = new();
        private Radio radioConfig = new();
        private COMMS commsConfig = new();
        private MapOptions mapConfig = new();
        private NavOffset navOffset = new();
        private ICP icpConfig = new();
        private Laser laserConfig = new();
        private FCCAIM aimConfig = new();
        private FCCAGM agmConfig = new();
        private FCCAGB agbConfig = new();
        private HUD hud = new();
        private CockpitView cockpitView = new();
        private OTW otw = new();
        private Weapons weapons = new();
        private SnsrPwr sensorPower = new();
        private InternalLighting internalLighting = new();
        private Link16 link16Config = new();
        private MFDColor mfdColors = new();

        #endregion Fields        

        #region Helper Methods

        protected override bool Read(string data)
        {
            bool result = true;
            try
            {
                result &= ewsConfig.Read(data);
                result &= mfdConfig.Read(data);
                result &= bullseye.Read(data);
                result &= iffConfig.Read(data);
                result &= harmConfig.Read(data);
                result &= steerpointConfig.Read(data);
                result &= radioConfig.Read(data);
                result &= commsConfig.Read(data);
                result &= mapConfig.Read(data);
                result &= navOffset.Read(data);
                result &= icpConfig.Read(data);
                result &= laserConfig.Read(data);
                result &= aimConfig.Read(data);
                result &= agmConfig.Read(data);
                result &= agbConfig.Read(data);
                result &= hud.Read(data);
                result &= cockpitView.Read(data);
                result &= otw.Read(data);
                result &= weapons.Read(data);
                result &= sensorPower.Read(data);
                result &= internalLighting.Read(data);
                result &= link16Config.Read(data);
                result &= mfdColors.Read(data);
            }
            catch (Exception ex)
            {                
                Utilities.Logging.ErrorLog.CreateLogFile(ex, "This error occurred while attempting to load " + _FileType + " from the following string:\n" + data);                
                if (ex is InvalidDataException)
                    return false;   // Tried to load a TEMission File
                throw;
            }
            return result;
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
            StringBuilder sb = new ();
            sb.Append(ewsConfig.Write());
            sb.Append(mfdConfig.Write());
            sb.Append(bullseye.Write());
            sb.Append(iffConfig.Write());
            sb.Append(harmConfig.Write());
            sb.Append(steerpointConfig.Write());
            sb.Append(radioConfig.Write());
            sb.Append(commsConfig.Write());
            sb.Append(mapConfig.Write());
            sb.Append(navOffset.Write());
            sb.Append(icpConfig.Write());
            sb.Append(laserConfig.Write());
            sb.Append(aimConfig.Write());
            sb.Append(agmConfig.Write());
            sb.Append(agbConfig.Write());
            sb.Append(hud.Write());
            sb.Append(cockpitView.Write());
            sb.Append(otw.Write());
            sb.Append(weapons.Write());
            sb.Append(sensorPower.Write());
            sb.Append(internalLighting.Write());
            sb.Append(Link16Config.Write());
            sb.Append(mfdColors.Write());
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(ewsConfig.ToString());
            sb.Append(mfdConfig.ToString());
            sb.Append(bullseye.ToString());
            sb.Append(iffConfig.ToString());
            sb.Append(harmConfig.ToString());
            sb.Append(steerpointConfig.ToString());
            sb.Append(radioConfig.ToString());
            sb.Append(commsConfig.ToString());
            sb.Append(mapConfig.ToString());
            sb.Append(navOffset.ToString());
            sb.Append(icpConfig.ToString());
            sb.Append(laserConfig.ToString());
            sb.Append(aimConfig.ToString());
            sb.Append(agmConfig.ToString());
            sb.Append(agbConfig.ToString());
            sb.Append(hud.ToString());
            sb.Append(cockpitView.ToString());
            sb.Append(otw.ToString());
            sb.Append(weapons.ToString());
            sb.Append(sensorPower.ToString());
            sb.Append(internalLighting.ToString());
            sb.Append(Link16Config.ToString());
            sb.Append(mfdColors.ToString());
            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(DTC? other)
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

            if (other is not DTC comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + ewsConfig.GetHashCode();
                hash = hash * 5483 + mfdConfig.GetHashCode();
                hash = hash * 5483 + bullseye.GetHashCode();
                hash = hash * 5483 + iffConfig.GetHashCode();
                hash = hash * 5483 + harmConfig.GetHashCode();
                hash = hash * 5483 + steerpointConfig.GetHashCode();
                hash = hash * 5483 + radioConfig.GetHashCode();
                hash = hash * 5483 + commsConfig.GetHashCode();
                hash = hash * 5483 + mapConfig.GetHashCode();
                hash = hash * 5483 + navOffset.GetHashCode();
                hash = hash * 5483 + icpConfig.GetHashCode();
                hash = hash * 5483 + aimConfig.GetHashCode();
                hash = hash * 5483 + agmConfig.GetHashCode();
                hash = hash * 5483 + agbConfig.GetHashCode();
                hash = hash * 5483 + hud.GetHashCode();
                hash = hash * 5483 + cockpitView.GetHashCode();
                hash = hash * 5483 + otw.GetHashCode();
                hash = hash * 5483 + weapons.GetHashCode();
                hash = hash * 5483 + sensorPower.GetHashCode();
                hash = hash * 5483 + internalLighting.GetHashCode();
                hash = hash * 5483 + link16Config.GetHashCode();
                hash = hash * 5483 + mfdColors.GetHashCode();
               

                return hash;
            }
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="DTC"/> object
        /// </summary>
        public DTC()
        {
            _FileType = ApplicationFileType.DTCINI;
            _StreamType = FileStreamType.Text;            
        }
        /// <summary>
        /// Initializes an instance of the <see cref="DTC"/> object using the values in <paramref name="dtc"/>.
        /// </summary>
        /// <param name="dtc">The <see cref="DTC"/> object with the values to copy.</param>
        public DTC(DTC dtc)
            : this()
        {
            ewsConfig = new(dtc.ewsConfig);
            mfdConfig = new(dtc.mfdConfig);
            bullseye = new(dtc.bullseye);
            iffConfig = new(dtc.iffConfig);
            harmConfig = new(dtc.harmConfig);
            steerpointConfig = new(dtc.steerpointConfig);
            radioConfig = new(dtc.radioConfig);
            commsConfig = new(dtc.commsConfig);
            mapConfig = new(dtc.mapConfig);
            navOffset = new(dtc.navOffset);
            icpConfig = new(dtc.icpConfig);
            laserConfig = new(dtc.laserConfig);
            aimConfig = new(dtc.aimConfig);
            agmConfig = new(dtc.agmConfig);
            agbConfig = new(dtc.agbConfig);
            hud = new(dtc.hud);
            cockpitView = new(dtc.cockpitView);
            otw = new(dtc.otw);
            weapons = new(dtc.weapons);
            sensorPower = new(dtc.sensorPower);
            internalLighting = new(dtc.internalLighting);
            link16Config = new(dtc.link16Config);
            mfdColors = new(dtc.mfdColors);
            _InitialHash = GetHashCode();
        }
        /// <summary>
        /// <para>Initializes an instance of the <see cref="DTC"/> object.</para>
        /// <para>Accepts a File Path as a valid input and checks if the file exists and has valid <see cref="DTC"/> parameters.</para>
        /// </summary>
        /// <param name="path">A valid path to an existing DTC.ini file.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public DTC(string path)
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
