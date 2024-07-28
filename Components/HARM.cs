using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the HARM Configuration in an INI File
    /// </summary>
    public class HARM : INIComponent, IEquatable<HARM>
    {
        #region Properties
        /// <summary>
        /// <para>Selected Delivery Mode</para>
        /// <para>0: HAS, 1:POS</para>
        /// </summary>
        public HARMMode Mode {  get => mode; set => mode = value; }        
        /// <summary>
        /// <para>Selected Delivery Sub-Mode</para>
        /// <para>0: PB, 1:EOM, 2:RUK</para>
        /// </summary>
        public HARMSubMode SubMode {  get => subMode; set => subMode = value; }       
        /// <summary>
        /// <para>Selected Table (TER)</para>
        /// <para>0: None, 1:Table 1, 2:Table 2, 3:Table 3</para>
        /// </summary>
        public int SelectedTable
        {
            //TODO: Verify these indexes match the game -- convert to Enum?
            get => selectedTable; 
            set
            {
                if (value < 1) selectedTable = 0;
                else selectedTable = Math.Min(value, 3);
            }
        }
        /// <summary>
        /// HARM Tables in the DTC Config
        /// </summary>
        public Collection<HarmTable> ThreatTables { get => threatTables; set => threatTables = value; } // Note: Mutable List        
        #endregion Properties // Checked

        #region Fields
        private HARMMode mode = HARMMode.HAS;
        private HARMSubMode subMode = HARMSubMode.PN;
        private int selectedTable = 0;
        private Collection<HarmTable> threatTables = [];

        #endregion Fields

        #region Helper Methods

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [HARM] flag                   
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the section tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();
                int val = 0;

                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s == null || s.Contains('['))
                        break;

                    s = s.ToUpper();

                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    if (s.StartsWith("MODE"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        mode = (HARMMode)val;
                        continue;
                    }
                    else if (s.StartsWith("SUBMODE"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        subMode = (HARMSubMode)val;
                        continue;
                    }
                    else if (s.StartsWith("TER"))
                    {
                        _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out val);
                        selectedTable = val;
                        continue;
                    }
                    else
                    {
                        string[] delims = [" ", "="];
                        string[] PGM = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                        int id = int.Parse(PGM[1]);
                        if (PGM.Length == 4)
                        {
                            _ = int.TryParse(PGM[3], out val);

                            switch (PGM[2])
                            {
                                case "0":
                                    threatTables[id].Threat1 = val;
                                    break;
                                case "1":
                                    threatTables[id].Threat2 = val;
                                    break;
                                case "2":
                                    threatTables[id].Threat3 = val;
                                    break;
                                case "3":
                                    threatTables[id].Threat4 = val;
                                    break;
                                case "4":
                                    threatTables[id].Threat5 = val;
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
            for (int i = 0; i < threatTables.Count; i++)
            {
                _ = sb.Append(threatTables[i].Write(i));
            }
            _ = sb.AppendLine("MODE=" + (int)mode);
            _ = sb.AppendLine("SUBMODE=" + (int)subMode);
            _ = sb.AppendLine("TER=" + selectedTable);

            return sb.ToString();
        }
                
        #endregion Helper Methods

        #region Functional Methods        

        public override string ToString()
        {

            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            sb.AppendLine("********************** HARM Configuration **********************");
            sb.AppendLine("     Mode: " + mode);
            sb.AppendLine("     Sub Mode: " + subMode);
            sb.AppendLine("     Selected Table: " + subMode);
            for (int i = 0; i < threatTables.Count; i++)
            {
                sb.AppendLine("     Threat Table " + i + ":");
                sb.Append(threatTables[i].ToString());
            }


            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(HARM? other)
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

            if (other is not HARM comparator)
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
                hash = hash * 5483 + SubMode.GetHashCode();
                hash = hash * 5483 + SelectedTable.GetHashCode();
                if (ThreatTables is not null)
                    if (ThreatTables.Count != 0)
                        foreach (HarmTable h in ThreatTables)
                            hash ^= h.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(HARM comparator1, HARM comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(HARM comparator1, HARM comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Instantiates an empty <see cref="HARM"/>.
        /// </summary>
        public HARM()
        {
            SectionFlag = "[HARM]";
            for (int i = 0; i < 3; i++)
                threatTables.Add(new HarmTable());
        }
        /// <summary>
        /// Instantiates a new <see cref="HARM"/> using the same values in <paramref name="harm"/>.
        /// </summary>
        /// <param name="ews">An <see cref="HARM"/> with existing data.</param>
        public HARM(HARM harm)
        {
            SectionFlag = "[HARM]";
            mode = harm.Mode;
            subMode = harm.SubMode;
            selectedTable = harm.SelectedTable;
            foreach (HarmTable t in harm.threatTables)
                threatTables.Add(new(t));
        }
        /// <summary>
        /// Instantiates a new <see cref="HARM"/> object with the supplied values. Threat tables are intitialed to their default settings.
        /// </summary>
        /// <param name="mode">HARM Mode to use</param>
        /// <param name="subMode">Sub-Mode to use</param>
        /// <param name="selectedTable">The Selected Threat Table</param>
        public HARM(HARMMode mode, HARMSubMode subMode, int selectedTable, Collection<HarmTable> tables)
        {
            SectionFlag = "[HARM]";
            this.mode = mode;
            this.subMode = subMode;
            this.selectedTable = selectedTable;
            foreach (HarmTable t in tables)
                threatTables.Add(new(t));
        }
        /// <summary>
        /// <para>Attempts to Initialize a <see cref="HARM"/> object using the data contained in <paramref name="initializationData"/>.</para>
        /// <para>If the provided Text does not contain the required data, a <see cref="HARM"/> object with default values is returned.</para>
        /// </summary>
        /// <param name="initializationData">A <see cref="string"/> containing Text that can be parsed to instantiate a <see cref="HARM"/> object.</param>
        public HARM(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);

        }

        #endregion Constructors

        #region Sub-Classes
        /// <summary>
        /// Represents a single Threat Table in the HARM Configuration for a DTC .ini File
        /// </summary>
        public class HarmTable
        {
            #region Properties
            /// <summary>
            /// ALIC Code Threat Entry on the Threat Table
            /// </summary>
            public int Threat1 { get; set; }
            /// <summary>
            /// ALIC Code Threat Entry on the Threat Table
            /// </summary>
            public int Threat2 { get; set; }
            /// <summary>
            /// ALIC Code Threat Entry on the Threat Table
            /// </summary>
            public int Threat3 { get; set; }
            /// <summary>
            /// ALIC Code Threat Entry on the Threat Table
            /// </summary>
            public int Threat4 { get; set; }
            /// <summary>
            /// ALIC Code Threat Entry on the Threat Table
            /// </summary>
            public int Threat5 { get; set; }
            #endregion Properties // Checked

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write(int tableID)
            {
                StringBuilder sb = new();
                _ = sb.AppendFormat("THREAT {0} 0={1}" + Environment.NewLine, tableID, Threat1.ToString("D4"));
                _ = sb.AppendFormat("THREAT {0} 1={1}" + Environment.NewLine, tableID, Threat2.ToString("D4"));
                _ = sb.AppendFormat("THREAT {0} 2={1}" + Environment.NewLine, tableID, Threat3.ToString("D4"));
                _ = sb.AppendFormat("THREAT {0} 3={1}" + Environment.NewLine, tableID, Threat4.ToString("D4"));
                _ = sb.AppendFormat("THREAT {0} 4={1}" + Environment.NewLine, tableID, Threat5.ToString("D4"));

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
                sb.AppendLine("        Threat 0: " + Threat1);
                sb.AppendLine("        Threat 1: " + Threat2);
                sb.AppendLine("        Threat 2: " + Threat3);
                sb.AppendLine("        Threat 3: " + Threat4);
                sb.AppendLine("        Threat 4: " + Threat5);

                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(HarmTable? other)
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

                if (other is not HarmTable comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + Threat1.GetHashCode();
                    hash = hash * 5483 + Threat2.GetHashCode();
                    hash = hash * 5483 + Threat3.GetHashCode();
                    hash = hash * 5483 + Threat4.GetHashCode();
                    hash = hash * 5483 + Threat5.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(HarmTable comparator1, HarmTable comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(HarmTable comparator1, HarmTable comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initiates a default <see cref="HarmTable"/> object
            /// </summary>
            public HarmTable()
            {
                Threat1 = Threat2 = Threat3 = Threat4 = Threat5 = 0;
            }
            /// <summary>
            /// Initiates a <see cref="HarmTable"/> object with the values in <paramref name="table"/>
            /// </summary>
            /// <param name="table">The <see cref="HarmTable"/> object to make a copy of.</param>
            public HarmTable(HarmTable table)
            {
                Threat1 = table.Threat1;
                Threat2 = table.Threat2;
                Threat3 = table.Threat3;
                Threat4 = table.Threat4;
                Threat5 = table.Threat5;
            }
            /// <summary>
            /// Initiates a new <see cref="HarmTable"/> object with the supplied ALIC Codes
            /// </summary>
            /// <param name="threat1">ALIC Code to assign to Threat 1</param>
            /// <param name="threat2">ALIC Code to assign to Threat 2</param>
            /// <param name="threat3">ALIC Code to assign to Threat 3</param>
            /// <param name="threat4">ALIC Code to assign to Threat 4</param>
            /// <param name="threat5">ALIC Code to assign to Threat 5</param>
            public HarmTable(int threat1, int threat2, int threat3, int threat4, int threat5)
            {
                Threat1 = threat1;
                Threat2 = threat2;
                Threat3 = threat3;
                Threat4 = threat4;
                Threat5 = threat5;
            }
            #endregion Constructors
        }
        #endregion Sub-Classes						

    }
}
