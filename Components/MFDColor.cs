using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;
using Utilities.Attributes;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the MFD Color Settings contained in an INI File
    /// </summary>
    public class MFDColor : INIComponent, IEquatable<MFDColor>
    {
        #region Properties
        /// <summary>
        /// Collection of Color Settings applied to MFDs
        /// </summary>
        public Collection<KeyValuePair<MFDColorSetting, MFDColorOption>> Settings { get => options; set => options = value; }     
        #endregion Properties

        #region Fields
        private Collection<KeyValuePair<MFDColorSetting, MFDColorOption>> options = [];
        private readonly string defaultColorSection = "[COLORS]\r\nColorConfig=78 3 3 5 4 4 -1 -1 3 -1 -1 -1 -1 1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 1 -1 -1 1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1\r\n";
        #endregion Fields

        #region Helper Methods

        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [COLOR] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [COLOR] tag and read each line
                _IncludeInOutput = true;
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();

                options.Clear();
                s = sr.ReadLine();
                if (s is null || s.Length == 0 || s.Contains('['))
                    return false;

                s = s.Replace("ColorConfig=", "");
                string[] vals = s.Split(' ');
                for (int i = 0; i < vals.Length - 1; i++)
                {
                    int val = int.Parse(vals[i + 1]);
                    options.Add(new KeyValuePair<MFDColorSetting, MFDColorOption>((MFDColorSetting)i, (MFDColorOption)val));
                }

            }
            catch (Exception ex)
            {
                Utilities.Logging.ErrorLog.CreateLogFile(ex, "This error occurred while attempting to load " + SectionFlag + " from the following string:\n" + data);
                if (ex is InvalidDataException)
                {
                    // No Colors Section
                    Read(defaultColorSection);
                    _IncludeInOutput = false;
                    return true;
                }
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
            sb.Append("ColorConfig=" + options.Count + ' ');
            for (int i = 0; i < options.Count; i++)
                sb.Append(((int)options[i].Value).ToString() + ' ');
            string s = sb.ToString().TrimEnd() + Environment.NewLine;


            return s;
        }
        #endregion Helper Methods

        #region Functional Methods
        /// <summary>
        /// Sets the MFD Color Setting for <paramref name="Property"/> to <paramref name="Color"/>.
        /// </summary>
        /// <param name="Property">The Property to set.</param>
        /// <param name="Color">The Color to set.</param>
        /// <returns></returns>
        public bool SetPropertyColor(MFDColorSetting Property, MFDColorOption Color)
        {
            if (options is null || options.Count - 1 < (int)Property)
                return false;

            options[(int)Property] = new KeyValuePair<MFDColorSetting, MFDColorOption>(Property, Color);
            return true;
        }
        /// <summary>
        /// Gets the <see cref="MFDColorOption"/> Color assigned to <paramref name="Property"/>
        /// </summary>
        /// <param name="Property">The <see cref="MFDColorSetting"/> Property to retrieve the Color Setting for</param>
        /// <returns></returns>
        public MFDColorOption GetPropertyColor(MFDColorSetting Property)
        {
            if (options.Count < (int)Property)
                return MFDColorOption.DEFAULT;

            return options[(int)Property].Value;
        }

        public override string ToString()
        {

            StringBuilder sb = new();

            sb.AppendLine("********************* Color Configuration **********************");
            sb.AppendLine("Color Configuration Entries: " + options.Count);
            for (int i = 0; i < options.Count; i++)
                sb.AppendLine(options[i].Key.GetStringValue() + ": " + options[i].Value.GetStringValue());


            return sb.ToString();
        }

       

        #region Equality Functions
        public bool Equals(MFDColor? other)
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

            if (other is not MFDColor comparator)
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
                if (options is not null)
                    if (options.Count != 0)
                        foreach (var option in options)
                            hash ^= option.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(MFDColor comparator1, MFDColor comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(MFDColor comparator1, MFDColor comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="MFDColor"/> object
        /// </summary>
        public MFDColor()
        {
            SectionFlag = "[COLORS]";
        }
        /// <summary>
        /// Initializes a new <see cref="MFDColor"/> object with the values supplied in <paramref name="colorSettings"/>
        /// </summary>
        /// <param name="colorSettings">The <see cref="MFDColor"/> object with the values to copy</param>
        public MFDColor(MFDColor colorSettings)
        {
            SectionFlag = "[COLORS]";
            foreach (KeyValuePair<MFDColorSetting, MFDColorOption> k in colorSettings.options)
                options.Add(new KeyValuePair<MFDColorSetting, MFDColorOption>(k.Key, k.Value));
        }
        /// <summary>
        /// Initializes a new <see cref="MFDColor"/> object with the values in <paramref name="options"/>
        /// </summary>
        /// <param name="options">Collection of Items and Settings used to initialize the <see cref="MFDColor"/> object</param>
        public MFDColor(ICollection<KeyValuePair<MFDColorSetting, MFDColorOption>> options)
        {
            SectionFlag = "[COLORS]";
            foreach (KeyValuePair<MFDColorSetting, MFDColorOption> k in options)
                this.options.Add(new KeyValuePair<MFDColorSetting, MFDColorOption>(k.Key, k.Value));
        }
        /// <summary>
        /// Initializes an instance of the <see cref="MFDColor"/> object using the values in <paramref name="InitializationData"/>
        /// </summary>
        /// <param name="InitializationData"><see cref="string"/> object containing the values to load</param>
        public MFDColor(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }


        #endregion Constructors
    }
}
