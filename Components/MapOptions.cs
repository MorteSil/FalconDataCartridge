using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Holds the Map Display Settings listed in an INI File
    /// </summary>
    public class MapOptions : INIComponent, IEquatable<MapOptions>
    {
        #region Properties
        /// <summary>
        /// <para>List of settings to determine which things to show on the Map.</para>
        /// <para>NOTE: BMS Implementation only allows you to select 1 from the
        /// following list:
        /// <list type="bullet">
        /// <item>GroundUnitDivision</item>
        /// <item>GroundUnitBrigade</item>
        /// <item>GroundUnitBatallion</item>
        /// </list></para>
        /// <para>Your code should should handle the exclusive selection. This 
        /// class will only save the first entry it finds.</para>
        /// </summary>
        public Collection<MapSetting> Settings { get => settings; set => settings = value; } // Note: Mutable List
        #endregion Properties // Checked

        #region Fields
        private Collection<MapSetting> settings = [];

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

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    string[] delims = ["_", "="];
                    string[] PGM = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    int opt = int.Parse(PGM[1]);
                    int val = int.Parse(PGM[2]);

                    foreach (MapSetting setting in settings)
                    {
                        if (setting.Name == (MapViewSettings)opt)
                        {
                            setting.IsEnabled = Convert.ToBoolean(val);
                            break;
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
            foreach (MapSetting m in Settings)
            {
                if (m.Name == MapViewSettings.GroundDivisions & m.IsEnabled)
                {
                    Settings[Settings.IndexOf(m) + 1].IsEnabled = false;
                    Settings[Settings.IndexOf(m) + 2].IsEnabled = false;
                }
                if (m.Name == MapViewSettings.GroundBrigades & m.IsEnabled)
                    Settings[Settings.IndexOf(m) + 1].IsEnabled = false;

                sb.AppendLine(m.Write());
            }

            return sb.ToString();
        }
       
        #endregion Helper Methods

        #region Functional Methods
        public override string ToString()
        {


            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            sb.AppendLine("************************* Map Options **************************");
            foreach (MapSetting m in Settings)
            {
                if (m.Name == MapViewSettings.GroundDivisions & m.IsEnabled)
                {
                    Settings[Settings.IndexOf(m) + 1].IsEnabled = false;
                    Settings[Settings.IndexOf(m) + 2].IsEnabled = false;
                }
                if (m.Name == MapViewSettings.GroundBrigades & m.IsEnabled)
                    Settings[Settings.IndexOf(m) + 1].IsEnabled = false;

                sb.AppendLine(m.ToString());
            }

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(MapOptions? other)
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

            if (other is not MapOptions comparator)
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
                if (settings != null)
                    if (settings.Count != 0)
                        foreach (MapSetting m in settings)
                            hash ^= m.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(MapOptions comparator1, MapOptions comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(MapOptions comparator1, MapOptions comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of a <see cref="MapOptions"/> object
        /// </summary>
        public MapOptions()
        {
            SectionFlag = "[MAP_POP]";
            for (int i=0;i<34;i++)
                settings.Add(new((MapViewSettings)i, false));
        }
        /// <summary>
        /// Initializes a new <see cref="MapOptions"/> object with the values in <paramref name="options"/>
        /// </summary>
        /// <param name="options"><see cref="MapOptions"/> objects with the values to copy</param>
        public MapOptions(MapOptions options)
        {
            SectionFlag = "[MAP_POP]";
            foreach (MapSetting m in options.Settings)
                settings.Add(new(m));
        }
        /// <summary>
        /// Initializes a new <see cref="MapOptions"/> object with the <see cref="MapSetting"/> list in <paramref name="settings"/>
        /// </summary>
        /// <param name="settings">List of <see cref="MapSetting"/> objects to copy</param>
        public MapOptions(ICollection<MapSetting> settings)
        {
            SectionFlag = "[MAP_POP]";
            foreach (MapSetting m in settings)
                this.settings.Add(new(m));
        }
        /// <summary>
        /// Initializes a new <see cref="MapOptions"/> Object using the values supplied in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> containing initialization data</param>
        public MapOptions(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

        #region Sub Classes
        /// <summary>
        /// Represents a single Map Display Option wihtin the DTC .ini File
        /// </summary>
        public class MapSetting
        {
            #region Properties
            /// <summary>
            /// <para>Setting ID of this <see cref="MapSetting"/> object</para>
            /// <para>See <see cref="MapViewSettings"/> for a list of the Available Settings and their related IDs</para>
            /// </summary>
            public MapViewSettings Name { get; private set; } = MapViewSettings.UNUSED;
            /// <summary>
            /// Whether this setting is nebaled or not
            /// </summary>
            public bool IsEnabled { get; set; } = false;
            #endregion Properties // Checked

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                int val = IsEnabled ? 1 : 0;

                return "MapOpt_" + (int)Name + "=" + val;
            }
            /// <summary>
            /// <para>Formats the data contained within this Component object into Readable Text.</para>
            /// <para>Readable Text does not always match the underlying file format and should not be used to save text based files such as .ini, .lst, or .txtpb files.</para>
            /// <para>Instead, use Write() to format all text or binary data for writing to a file.</para>
            /// </summary>
            /// <returns>A formatted <see cref="string"/> with Human Readable Text.</returns>
            public override string ToString()
            {
                int val = IsEnabled ? 1 : 0;
                return "     " + Name + ": " + val;
            }

            #region Equality Functions
            public bool Equals(MapSetting? other)
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

                if (other is not MapSetting comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + Name.GetHashCode();
                    hash = hash * 5483 + IsEnabled.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(MapSetting comparator1, MapSetting comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(MapSetting comparator1, MapSetting comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Functional Methods

            #region Constructors
            /// <summary>
            /// Initializes an new <see cref="MapSetting"/> object with the UNUSED Setting ID of 14 set to <see langword="false"/>
            /// </summary>
            public MapSetting()
            {

            }
            /// <summary>
            /// Initializes a new <see cref="MapSetting"/> object with the values in <paramref name="setting"/>
            /// </summary>
            /// <param name="setting"><see cref="MapSetting"/> object with the values to copy</param>
            public MapSetting(MapSetting setting)
            {
                Name = setting.Name;
                IsEnabled = setting.IsEnabled;
            }
            /// <summary>
            /// <para>Initializes a new <see cref="MapSetting"/> object with the supplied values</para>
            /// <para>See <see cref="MapViewSettings"/> for a list of the Available Settings and their related Index</para>
            /// </summary>
            /// <param name="id">Setting ID of this <see cref="MapSetting"/> object</param>
            /// <param name="isSelected">Whether or not this <see cref="MapSetting"/> is enabled</param>
            public MapSetting(MapViewSettings settingName, bool isSelected)
            {
                Name = settingName;
                IsEnabled = isSelected;
            }

            #endregion Constructors
        }
        #endregion Sub Classes
    }
}
