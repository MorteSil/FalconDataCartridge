using System.Collections.ObjectModel;
using System.Text;
using FalconDataCartridge.Enums;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the IFF Configuraiton Section of an INI File
    /// </summary>
    public class IFF : INIComponent, IEquatable<IFF>
    {
        #region Properties
        /// <summary>
        /// Indicates if this Mode is active
        /// </summary>
        public bool Mode1Enabled { get => mode1Enabled; set => mode1Enabled = value; }        
        /// <summary>
        /// Indicates if this Mode is active
        /// </summary>
        public bool Mode2Enabled { get => mode2Enabled; set => mode2Enabled = value; }        
        /// <summary>
        /// Indicates if this Mode is active
        /// </summary>
        public bool Mode3AEnabled { get => mode3AEnabled; set => mode3AEnabled = value; }       
        /// <summary>
        /// Indicates if this Mode is active
        /// </summary>
        public bool Mode4Enabled { get => mode4Enabled; set => mode4Enabled = value; }        
        /// <summary>
        /// Indicates if this Mode is active
        /// </summary>
        public bool ModeCEnabled { get => ModeCEnabled; set => ModeCEnabled = value; }       
        /// <summary>
        /// Indicates if this Mode is active
        /// </summary>
        public bool ModeSEnabled { get => modeSEnabled; set => modeSEnabled = value; }        
        /// <summary>
        /// Mode 1 Code
        /// </summary>
        public int Mode1Code
        {
            get => mode1Code; 
            set
            {
                if (value < 1) mode1Code = 0;
                else { mode1Code = Math.Min(value, 63); }
            }
        }
        /// <summary>
        /// Mode 2 Code
        /// </summary>
        public int Mode2Code
        {
            get => mode2Code; 
            set
            {
                if (value < 1) mode2Code = 0;
                else { mode2Code = Math.Min(value, 4095); }
            }
        }
        /// <summary>
        /// Mode 3A Code
        /// </summary>
        public int Mode3ACode
        {
            get => mode3ACode; 
            set
            {
                if (value < 1) mode3ACode = 0;
                else { mode3ACode = Math.Min(value, 4095); }
            }
        }
        /// <summary>
        /// Mode 4 Key
        /// </summary>
        public int Mode4Code
        {
            get => mode4Code; 
            set
            {
                if (value < 1) mode4Code = 0;
                else { mode4Code = 1; }
            }
        }
        /// <summary>
        /// <para>Enables Automatic Changing of codes according to the IFF Plan</para>
        /// <para>0: MAN, 1: POS, 2: TIM, 3: P/T</para>
        /// </summary>
        public IFFAutoChange AutoChangeEnabled { get => autoChange; set => autoChange = value; }       
        /// <summary>
        /// Time Blocks to use when AutoChange is enabled
        /// </summary>
        public Collection<TimeBlock> TimeBlocks { get => timeBlocks; set => timeBlocks = value; } // Note: Mutable List
        /// <summary>
        /// Position references to use when AutoChange is enabled
        /// </summary>
        public Collection<PositionBlock> PositionBlocks {  get => positionBlocks; set => positionBlocks = value; } // Note: Mutable List
        #endregion Properties // Checked

        #region Fields
        private bool mode1Enabled = false;
        private bool mode2Enabled = false;
        private bool mode3AEnabled = false;
        private bool mode4Enabled = false;
        private bool modeCEnabled = false;
        private bool modeSEnabled = false;
        private int mode1Code = 0;
        private int mode2Code = 0;
        private int mode3ACode = 0;
        private int mode4Code = 0;
        private IFFAutoChange autoChange = IFFAutoChange.MAN;
        private Collection<TimeBlock> timeBlocks = [];
        private Collection<PositionBlock> positionBlocks = [];

        #endregion Fields

        #region Helper Methods

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {

                // Check if the string is empty or does not contain the [IFF] flag                   
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [IFF] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();
                int i = 0;


                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s == null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer

                    if (s.StartsWith("AUTOCHANGE"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out i);
                        autoChange = (IFFAutoChange)i;
                        continue;
                    }
                    else if (s.StartsWith("MODEC"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out i);
                        modeCEnabled = Convert.ToBoolean(i);
                        continue;
                    }
                    else if (s.StartsWith("MODES"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out i);
                        modeSEnabled = Convert.ToBoolean(i);
                        continue;
                    }
                    else
                    {
                        string[] delims = [" ", "="];
                        string[] dataLine = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        int val;
                        if (dataLine.Length == 3)
                        {
                            _ = int.TryParse(dataLine[2], out val);

                            switch (dataLine[0], dataLine[1])
                            {
                                case ("MODE1", "ON"):
                                    mode1Enabled = Convert.ToBoolean(val);
                                    break;
                                case ("MODE1", "CODE"):
                                    mode1Code = val;
                                    break;
                                case ("MODE2", "ON"):
                                    mode2Enabled = Convert.ToBoolean(val);
                                    break;
                                case ("MODE2", "CODE"):
                                    mode2Code = val;
                                    break;
                                case ("MODE3A", "ON"):
                                    mode3AEnabled = Convert.ToBoolean(val);
                                    break;
                                case ("MODE3A", "CODE"):
                                    mode3ACode = val;
                                    break;
                                case ("MODE4", "ON"):
                                    mode4Enabled = Convert.ToBoolean(val);
                                    break;
                                case ("MODE4", "KEY"):
                                    mode4Code = val;
                                    break;
                            }
                        }
                        else if (dataLine.Length == 5)
                        {
                            int id = int.Parse(dataLine[1]);
                            _ = int.TryParse(dataLine[4], out val);

                            switch (dataLine[0], dataLine[2])
                            {
                                case ("TIME", "MODE1"):
                                    timeBlocks[id].Mode1Code = val;
                                    break;
                                case ("TIME", "MODE3A"):
                                    timeBlocks[id].Mode3ACode = val;
                                    break;
                                case ("TIME", "MODE4"):
                                    timeBlocks[id].Mode4Key = val;
                                    break;

                            }
                        }
                        else if (dataLine.Length == 4)
                        {
                            int id = int.Parse(dataLine[1]);
                            _ = int.TryParse(dataLine[3], out val);
                            switch (dataLine[0], dataLine[2])
                            {
                                case ("TIME", "CRITERIA"):
                                    timeBlocks[id].BlockStart = new TimeOnly(val / 100, val % 100);
                                    break;
                                case ("POS", "MODE1"):
                                    positionBlocks[id].Mode1Code = val;
                                    break;
                                case ("POS", "MODE2"):
                                    positionBlocks[id].Mode2Code = val;
                                    break;
                                case ("POS", "MODE3A"):
                                    positionBlocks[id].Mode3ACode = val;
                                    break;
                                case ("POS", "MODE4"):
                                    positionBlocks[id].Mode4Key = val;
                                    break;
                                case ("POS", "MODES"):
                                    positionBlocks[id].ModeSCode = val;
                                    break;
                                case ("POS", "MODEC"):
                                    positionBlocks[id].ModeCCode = val;
                                    break;
                                case ("POS", "WAYPOINT"):
                                    positionBlocks[id].Waypoint = val;
                                    break;
                                case ("POS", "DIRECTION"):
                                    positionBlocks[id].Direction = (Utilities.GeoLib.CardinalDirection)val;
                                    break;
                            }
                        }
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
            _ = sb.AppendLine("Mode1 On=" + Convert.ToInt16(mode1Enabled));
            _ = sb.AppendLine("Mode2 On=" + Convert.ToInt16(mode2Enabled));
            _ = sb.AppendLine("Mode3A On=" + Convert.ToInt16(mode3AEnabled));
            _ = sb.AppendLine("Mode4 On=" + Convert.ToInt16(mode4Enabled));
            _ = sb.AppendLine("ModeC On=" + Convert.ToInt16(modeCEnabled));
            _ = sb.AppendLine("ModeS On=" + Convert.ToInt16(modeSEnabled));
            _ = sb.AppendLine("Mode1 Code=" + mode1Code);
            _ = sb.AppendLine("Mode2 Code=" + mode2Code);
            _ = sb.AppendLine("Mode3A Code=" + mode3ACode);
            _ = sb.AppendLine("Mode4 Key=" + mode4Code);
            _ = sb.AppendLine("AutoChange=" + (int)autoChange);
            for (int i = 0; i < timeBlocks.Count; i++)
                sb.Append(timeBlocks[i].Write(i));

            for (int i = 0; i < positionBlocks.Count; i++)
                sb.Append(positionBlocks[i].Write(i));

            return sb.ToString();
        }

        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {

            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            sb.AppendLine("********************** IFF Configuration ***********************");
            _ = sb.AppendLine("     Mode 1 Enabled: " + mode1Enabled);
            _ = sb.AppendLine("     Mode 2 Enabled: " + mode2Enabled);
            _ = sb.AppendLine("     Mode 3A Enabled: " + mode3AEnabled);
            _ = sb.AppendLine("     Mode 4 Enabled: " + mode4Enabled);
            _ = sb.AppendLine("     Mode C Enabled: " + modeCEnabled);
            _ = sb.AppendLine("     Mode S Enabled: " + modeSEnabled);
            _ = sb.AppendLine("     Mode 1 Code:" + mode1Code);
            _ = sb.AppendLine("     Mode 2 Code:" + mode2Code);
            _ = sb.AppendLine("     Mode 3A Code:" + mode3ACode);
            _ = sb.AppendLine("     Mode 4 Key: " + mode4Code);
            _ = sb.AppendLine("     AutoC hange Setting: " + autoChange);
            for (int i = 0; i < TimeBlocks.Count; i++)
            {
                sb.Append(TimeBlocks[i].ToString());
            }

            for (int i = 0; i < PositionBlocks.Count; i++)
            {
                sb.Append(positionBlocks[i].ToString());
            }

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(IFF? other)
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

            if (other is not IFF comparator)
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
                hash = hash * 5483 + mode1Enabled.GetHashCode();
                hash = hash * 5483 + mode2Enabled.GetHashCode();
                hash = hash * 5483 + mode3AEnabled.GetHashCode();
                hash = hash * 5483 + mode4Enabled.GetHashCode();
                hash = hash * 5483 + modeCEnabled.GetHashCode();
                hash = hash * 5483 + ModeSEnabled.GetHashCode();
                hash = hash * 5483 + Mode1Code.GetHashCode();
                hash = hash * 5483 + Mode2Code.GetHashCode();
                hash = hash * 5483 + Mode3ACode.GetHashCode();
                hash = hash * 5483 + mode4Code.GetHashCode();
                hash = hash * 5483 + AutoChangeEnabled.GetHashCode();
                foreach (TimeBlock b in TimeBlocks)
                    hash = hash * 5483 + b.GetHashCode();
                foreach (PositionBlock b in PositionBlocks)
                    hash = hash * 5483 + b.GetHashCode();
                return hash;
            }
            
        }
        public static bool operator ==(IFF comparator1, IFF comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(IFF comparator1, IFF comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Instantiates an empty <see cref="IFF"/> Object
        /// </summary>
        public IFF()
        {
            SectionFlag = "[IFF]";
            TimeOnly t = new(7, 0);

            for (int i = 0; i < 12; i++)
            {
                timeBlocks.Add(new TimeBlock(0, 0, 0, t));
                t = t.Add(new TimeSpan(0, 30, 0));
            }
            for (int i = 0; i < 2; i++)
            {
                positionBlocks.Add(new PositionBlock());
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="IFF"/> object using the values in <paramref name="iff"/>
        /// </summary>
        /// <param name="iff"><see cref="IFF"/> object to make a copy of</param>
        public IFF(IFF iff)
        {
            SectionFlag = "[IFF]";
            mode1Enabled = iff.Mode1Enabled; mode2Enabled = iff.mode2Enabled; mode3AEnabled = iff.mode3AEnabled;
            mode4Enabled = iff.mode4Enabled; modeCEnabled = iff.mode4Enabled; modeSEnabled = iff.mode4Enabled;
            mode1Code = iff.mode1Code; mode2Code = iff.mode2Code; mode3ACode = iff.mode3ACode;
            mode4Code = iff.mode4Code; autoChange = iff.autoChange;

            foreach (TimeBlock t in iff.TimeBlocks)
                timeBlocks.Add(new(t));
            foreach (PositionBlock p in iff.PositionBlocks)
                positionBlocks.Add(new(p));
        }
        /// <summary>
        /// Initializes a new <see cref="IFF"/> with the supplied values
        /// </summary>
        /// <param name="mode1Enabled">Enables Mode 1</param>
        /// <param name="mode2Enabled">Enables Mode 2</param>
        /// <param name="mode3Enabled">Enables Mode 3A</param>
        /// <param name="mode4Enabled">Enables Mode 4</param>
        /// <param name="mode5Enabled">Enables Mode S</param>
        /// <param name="modecEnabled">Enables Mode C</param>
        /// <param name="modesEnabled">Enables Mode S</param>
        /// <param name="mode1Code">Mode 1 Code</param>
        /// <param name="mode2Code">Mode 2 Code</param>
        /// <param name="mode3Code">Mode 3A Code</param>
        /// <param name="mode4Code">Mode 4 Key</param>
        /// <param name="autoChangeType">Auto Change Mode</param>
        public IFF(bool mode1Enabled, bool mode2Enabled, bool mode3Enabled, bool mode4Enabled, bool mode5Enabled, bool modecEnabled, bool modesEnabled, int mode1Code, int mode2Code, int mode3Code, int mode4Code, IFFAutoChange autoChangeType, ICollection<TimeBlock> timeBlocks, ICollection<PositionBlock> positionBlocks)
        {
            SectionFlag = "[IFF]";
            this.mode1Enabled = mode1Enabled; this.mode2Enabled = mode2Enabled; mode3AEnabled = mode3Enabled;
            this.mode4Enabled = mode4Enabled; modeCEnabled = modecEnabled; modeSEnabled = modesEnabled;
            this.mode1Code = mode1Code; this.mode2Code = mode2Code; mode3ACode = mode3Code;
            this.mode4Code = mode4Code; autoChange = autoChangeType;

            foreach (TimeBlock b in timeBlocks)
                this.timeBlocks.Add(new(b));

            foreach (PositionBlock p in positionBlocks)
                this.positionBlocks.Add(new(p));

        }
        /// <summary>
        /// Instantiates a new <see cref="IFF"/> object using the values in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> containing initialization values for an <see cref="IFF"/> object</param>
        public IFF(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

        #region Sub-Classes
        /// <summary>
        /// Represents a Time Block Configuration witin the DTC IFF Configuration Section
        /// </summary>
        public class TimeBlock
        {
            #region Properties
            /// <summary>
            /// Mode 1 Code
            /// </summary>
            public int Mode1Code { get; set; } = 0;
            /// <summary>
            /// Mode 3A Code
            /// </summary>
            public int Mode3ACode { get; set; } = 0;
            /// <summary>
            /// Mode 4 Key
            /// </summary>
            public int Mode4Key { get; set; } = 0;
            /// <summary>
            /// Time this <see cref="TimeBlock"/> becomes effective
            /// </summary>
            public TimeOnly BlockStart { get; set; } = new(0, 0);
            /// <summary>
            /// Read Only <see cref="string"/> representation of the time this <see cref="TimeBlock"/> becomes effective
            /// </summary>
            public string Criteria {  get => BlockStart.Hour.ToString() + BlockStart.Minute.ToString("D2"); }
            
            #endregion Properties

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write(int blockID)
            {
                StringBuilder sb = new();
                _ = sb.AppendFormat("TIME {0} Mode1 Code={1}" + Environment.NewLine, blockID, Mode1Code);
                _ = sb.AppendFormat("TIME {0} Mode3A Code={1}" + Environment.NewLine, blockID, Mode3ACode);
                _ = sb.AppendFormat("TIME {0} Mode4 Key={1}" + Environment.NewLine, blockID, Mode4Key);
                _ = sb.AppendFormat("TIME {0} Criteria={1}" + Environment.NewLine, blockID, Criteria);
                return sb.ToString();
            }
            /// <summary>
            /// <para>Formats the data contained within this Component object into Readable Text.</para>
            /// <para>Readable Text does not always match the underlying file format and should not be used to save text based files such as .ini, .lst, or .txtpb files.</para>
            /// <para>Instead, use Write() to format all text or binary data for writing to a file.</para>
            /// </summary>
            /// <returns>A formatted <see cref="string"/> with Human Readable Text.</returns>
            public override string ToString()
            {
                StringBuilder sb = new();
                sb.AppendLine("     Time Block " + BlockStart);
                sb.AppendLine("        Mode 1 Code: " + Mode1Code);
                sb.AppendLine("        Mode 3A Code: " + Mode3ACode);
                sb.AppendLine("        Mode 4 Key: " + Mode4Key);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(TimeBlock? other)
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

                if (other is not TimeBlock comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + Mode1Code.GetHashCode();
                    hash = hash * 5483 + Mode3ACode.GetHashCode();
                    hash = hash * 5483 + Mode4Key.GetHashCode();
                    hash = hash * 5483 + BlockStart.GetHashCode();
                    hash = hash * 5483 + Criteria.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(TimeBlock comparator1, TimeBlock comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(TimeBlock comparator1, TimeBlock comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes a default version of the <see cref="TimeBlock"/> object
            /// </summary>
            public TimeBlock() { }
            /// <summary>
            /// Initializes a new <see cref="TimeBlock"/> object with teh values in <paramref name="timeBlock"/>
            /// </summary>
            /// <param name="timeBlock">The <see cref="TimeBlock"/> object with the values to copy</param>
            public TimeBlock(TimeBlock timeBlock)
            {
                Mode1Code = timeBlock.Mode1Code;
                Mode3ACode = timeBlock.Mode3ACode;
                Mode4Key = timeBlock.Mode4Key;
                BlockStart = new(timeBlock.BlockStart.Ticks);
            }
            /// <summary>
            /// Initializes a <see cref="TimeBlock"/> object with the supplied values
            /// </summary>
            /// <param name="mode1">Mode 1 Code</param>
            /// <param name="mode3">Mode 3A Code</param>
            /// <param name="mode4">Mode 4 Key</param>
            /// <param name="time">Time when the <see cref="TimeBlock"/> object becomes active</param>
            public TimeBlock(int mode1, int mode3, int mode4, TimeOnly time)
            {
                Mode1Code = mode1;
                Mode3ACode = mode3;
                Mode4Key = mode4;
                BlockStart = new(time.Ticks);
            }
            #endregion Constructors

        }

        /// <summary>
        /// Represents a Position Block Configuration witin the DTC IFF Configuration Section
        /// </summary>
        public class PositionBlock
        {
            #region Properties
            /// <summary>
            /// Mode 1 Code
            /// </summary>
            public int Mode1Code { get; set; } = 0;
            /// <summary>
            /// Mode 2 Code
            /// </summary>
            public int Mode2Code { get; set; } = 0;
            /// <summary>
            /// Mode 3A Code
            /// </summary>
            public int Mode3ACode { get; set; } = 0;
            /// <summary>
            /// Mode 4 Key
            /// </summary>
            public int Mode4Key { get; set; } = 0;
            /// <summary>
            /// Mode C Code
            /// </summary>
            public int ModeCCode { get; set; } = 0;
            /// <summary>
            /// Mode S Code
            /// </summary>
            public int ModeSCode { get; set; } = 0;
            /// <summary>
            /// Waypoint used in Position Auto Change Events
            /// </summary>
            public int Waypoint { get; set; } = 0;
            /// <summary>
            /// <para>Direction Reference used to trigger Position Auto Change Events</para>
            /// <para>When the Aircraft position is in this direction relative to the Waypoint defined above, the Position Auto Change event occurs</para>
            /// </summary>
            public Utilities.GeoLib.CardinalDirection Direction { get; set; } = 0;

            #endregion Properties // Checked

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write(int blockID)
            {
                StringBuilder sb = new();
                _ = sb.AppendFormat("POS {0} Mode1={1}" + Environment.NewLine, blockID, Mode1Code);
                _ = sb.AppendFormat("POS {0} Mode2={1}" + Environment.NewLine, blockID, Mode2Code);
                _ = sb.AppendFormat("POS {0} Mode3A={1}" + Environment.NewLine, blockID, Mode3ACode);
                _ = sb.AppendFormat("POS {0} Mode4={1}" + Environment.NewLine, blockID, Mode4Key);
                _ = sb.AppendFormat("POS {0} ModeC={1}" + Environment.NewLine, blockID, Mode1Code);
                _ = sb.AppendFormat("POS {0} ModeS={1}" + Environment.NewLine, blockID, Mode1Code);
                _ = sb.AppendFormat("POS {0} WayPoint={1}" + Environment.NewLine, blockID, Waypoint);
                _ = sb.AppendFormat("POS {0} Direction={1}" + Environment.NewLine, blockID, (int)Direction);

                return sb.ToString();
            }
            /// <summary>
            /// <para>Formats the data contained within this Component object into Readable Text.</para>
            /// <para>Readable Text does not always match the underlying file format and should not be used to save text based files such as .ini, .lst, or .txtpb files.</para>
            /// <para>Instead, use Write() to format all text or binary data for writing to a file.</para>
            /// </summary>
            /// <returns>A formatted <see cref="string"/> with Human Readable Text.</returns>
            public override string ToString()
            {

                StringBuilder sb = new();
                sb.AppendLine("     Position Block: ");
                sb.AppendLine("        Mode 1 Code: " + Mode1Code);
                sb.AppendLine("        Mode 2 Code: " + Mode2Code);
                sb.AppendLine("        Mode 3A Code: " + Mode3ACode);
                sb.AppendLine("        Mode 4 Key: " + Mode4Key);
                sb.AppendLine("        Mode C Code: " + ModeCCode);
                sb.AppendLine("        Mode S Code: " + ModeSCode);
                sb.AppendLine("        Waypoint: " + Waypoint);
                sb.AppendLine("        Direction: " + Direction);
                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(PositionBlock? other)
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

                if (other is not PositionBlock comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + Mode1Code.GetHashCode();
                    hash = hash * 5483 + Mode2Code.GetHashCode();
                    hash = hash * 5483 + Mode3ACode.GetHashCode();
                    hash = hash * 5483 + Mode4Key.GetHashCode();
                    hash = hash * 5483 + ModeCCode.GetHashCode();
                    hash = hash * 5483 + ModeSCode.GetHashCode();
                    hash = hash * 5483 + Waypoint.GetHashCode();
                    hash = hash * 5483 + Direction.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(PositionBlock comparator1, PositionBlock comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(PositionBlock comparator1, PositionBlock comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes an empty <see cref="PositionBlock"/> object
            /// </summary>
            public PositionBlock() { }
            /// <summary>
            /// Initializes a new <see cref="PositionBlock"/> object using the values in <paramref name="block"/>
            /// </summary>
            /// <param name="block"><see cref="PositionBlock"/></param> object with the values to copy
            public PositionBlock(PositionBlock block)
            {
                Mode1Code = block.Mode1Code;
                Mode2Code = block.Mode2Code;
                Mode3ACode = block.Mode3ACode;
                Mode4Key = block.Mode4Key;
                ModeCCode = block.ModeCCode;
                ModeSCode = block.ModeSCode;
                Waypoint = block.Waypoint;
                Direction = block.Direction;
            }
            /// <summary>
            /// Initializes a new <see cref="PositionBlock"/> object with the supplied values
            /// </summary>
            /// <param name="mode1Code">Mode 1 Code</param>
            /// <param name="mode2Code">Mode 2 Code</param>
            /// <param name="mode3Code">Mode 3A Code</param>
            /// <param name="mode4Code">Mode 4 Key</param>
            /// <param name="modecCode">Mode C Code</param>
            /// <param name="modesCode">Mode S Code</param>
            /// <param name="waypoint">Reference Waypoint</param>
            /// <param name="direction">Reference Direction</param>
            public PositionBlock(int mode1Code, int mode2Code, int mode3Code, int mode4Code, int modecCode, int modesCode, int waypoint, int direction)
            {
                Mode1Code = mode1Code;
                Mode2Code = mode2Code;
                Mode3ACode = mode3Code;
                Mode4Key = mode4Code;
                ModeCCode = mode3Code;
                ModeSCode = mode4Code;
                Waypoint = waypoint;
                Direction = (Utilities.GeoLib.CardinalDirection)direction;
            }
            /// <summary>
            /// Initializes a new <see cref="PositionBlock"/> object with the supplied values
            /// </summary>
            /// <param name="mode1Code">Mode 1 Code</param>
            /// <param name="mode2Code">Mode 2 Code</param>
            /// <param name="mode3Code">Mode 3A Code</param>
            /// <param name="mode4Code">Mode 4 Key</param>
            /// <param name="modecCode">Mode C Code</param>
            /// <param name="modesCode">Mode S Code</param>
            /// <param name="waypoint">Reference Waypoint</param>
            /// <param name="direction">Reference Direction</param>
            public PositionBlock(int mode1Code, int mode2Code, int mode3Code, int mode4Code, int modecCode, int modesCode, int waypoint, Utilities.GeoLib.CardinalDirection direction)
            {
                Mode1Code = mode1Code;
                Mode2Code = mode2Code;
                Mode3ACode = mode3Code;
                Mode4Key = mode4Code;
                ModeCCode = mode3Code;
                ModeSCode = mode4Code;
                Waypoint = waypoint;
                Direction = direction;
            }
            #endregion Constructors

        }
        #endregion Sub-Classes						

    }
}
