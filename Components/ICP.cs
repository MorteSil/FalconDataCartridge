using FalconDataCartridge.Enums;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the ICP Configuration Section of an INI File
    /// </summary>
    public class ICP : INIComponent, IEquatable<ICP>
    {
        #region Properties
        /// <summary>
        /// Selected Master Dome
        /// </summary>
        public MFDMasterMode Mode {  get => mode; set => mode = value; }       
        /// <summary>
        /// Altitude Low Warning Level in AGL
        /// </summary>
        public double ALOWAGL { get => alowAGL; set => alowAGL = value; }        
        /// <summary>
        /// Altitude Low Warning Level in MSL
        /// </summary>
        public double ALOWMSL { get => alowMSL; set => alowMSL = value; }       
        /// <summary>
        /// Altitude Low Warning Level in Advanced Terrain Following Mode
        /// </summary>
        public double ALOWTFAdv { get => alowTFAdv; set => alowTFAdv = value; }        
        /// <summary>
        /// Manual Wingspan Setting for Gun Sight Ranne Calibration
        /// </summary>
        public double ManualWingspan { get => manWingspan; set => manWingspan = value; }        
        /// <summary>
        /// Fuel Level that triggers a Bingo Warning
        /// </summary>
        public double Bingo { get => bingo; set => bingo = value; }       
        #endregion Properties // Checked

        #region Fields
        private MFDMasterMode mode = MFDMasterMode.NAV;
        private double alowAGL = 100.0;
        private double alowMSL = 10000;
        private double alowTFAdv = 400;
        private double manWingspan = 35.0;
        private double bingo = 2500;

        #endregion Fields

        #region Helper Methods     

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {

                // Check if the string is empty or does not contain the [ICP] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [ICP] tag and read each line
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

                    if (s.StartsWith("MASTERMODE"))
                    {
                        mode = (MFDMasterMode)Convert.ToInt32(i);
                        continue;
                    }
                    else if (s.StartsWith("ALOW AGL"))
                    {
                        alowAGL = i;
                        continue;
                    }
                    else if (s.StartsWith("ALOW MSL"))
                    {
                        alowMSL = i;
                        continue;
                    }
                    else if (s.StartsWith("ALOW TFADV"))
                    {
                        alowTFAdv = i;
                        continue;
                    }
                    else if (s.StartsWith("MANUAL"))
                    {
                        manWingspan = i;
                        continue;
                    }
                    else if (s.StartsWith("BINGO"))
                    {
                        bingo = i;
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
            _ = sb.AppendLine("MasterMode=" + (int)mode);
            _ = sb.AppendLine("Alow AGL=" + alowAGL.ToString("F6"));
            _ = sb.AppendLine("Alow MSL=" + alowMSL);
            _ = sb.AppendLine("Alow TFAdv=" + alowTFAdv);
            _ = sb.AppendLine("Manual Wingspan=" + manWingspan.ToString("F6"));
            _ = sb.AppendLine("Bingo_Fuel=" + bingo.ToString("F6"));
            return sb.ToString();
        }
        #endregion Helper Methods        

        #region Functional Methods       

        public override string ToString()
        {

            StringBuilder sb = new();
            sb.AppendLine("*********************** ICP Configuration **********************");
            sb.AppendLine("     MasterMode: " + mode);
            sb.AppendLine("     Alow AGL: " + alowAGL.ToString("F6"));
            sb.AppendLine("     Alow MSL: " + alowMSL);
            sb.AppendLine("     Alow TFAdv: " + alowTFAdv);
            sb.AppendLine("     Manual Wingspan: " + manWingspan.ToString("F6"));
            sb.AppendLine("     Bingo Fuel: " + bingo.ToString("F6"));
            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(ICP? other)
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

            if (other is not ICP comparator)
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
                hash = hash * 5483 + Mode.GetHashCode();
                hash = hash * 5483 + alowAGL.GetHashCode();
                hash = hash * 5483 + alowMSL.GetHashCode();
                hash = hash * 5483 + alowTFAdv.GetHashCode();
                hash = hash * 5483 + ManualWingspan.GetHashCode();
                hash = hash * 5483 + bingo.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(ICP comparator1, ICP comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(ICP comparator1, ICP comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Instantiates an empty <see cref="ICP"/> object
        /// </summary>
        public ICP()
        {
            SectionFlag = "[ICP]";
        }
        /// <summary>
        /// Instantiates a new <see cref="ICP"/> object using the vaules in <paramref name="icp"/>
        /// </summary>
        /// <param name="icp"></param>
        public ICP(ICP icp)
        {
            SectionFlag = "[ICP]";
            mode = icp.mode;
            alowAGL = icp.alowAGL;
            alowMSL = icp.alowMSL;
            alowTFAdv = icp.alowTFAdv;
            manWingspan = icp.manWingspan;
            bingo = icp.bingo;
        }
        /// <summary>
        /// Initializes a new <see cref="ICP"/>object using the supplied values
        /// </summary>
        /// <param name="mode">Selected System MAstermode</param>
        /// <param name="alowAGL">Altitude Low Warning Level in AGL</param>
        /// <param name="alowMSL">Altitude Low Warning Level in MSL</param>
        /// <param name="alowTF">Altitude Low Warning Level in Advanced Terrain Following Mode</param>
        /// <param name="wingSpan">Manual Wingspan Setting for Gun Sight Ranne Calibration</param>
        /// <param name="bingo">Fuel Level that triggers a Bingo Warning</param>
        public ICP(MFDMasterMode mode, double alowAGL, double alowMSL, double alowTF, double wingSpan, double bingo)
        {
            SectionFlag = "[ICP]";
            this.mode = mode;
            this.alowAGL = alowAGL;
            this.alowMSL = alowMSL;
            alowTFAdv = alowTF;
            manWingspan = wingSpan;
            this.bingo = bingo;
            this.mode = mode;
        }
        /// <summary>
        /// Instantiates a new <see cref="ICP"/> object with the data supplied in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object containing initialization data for an <see cref="ICP"/>object</param>
        public ICP(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }


        #endregion Constructors

    }
}
