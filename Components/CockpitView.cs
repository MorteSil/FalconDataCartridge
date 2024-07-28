using System.Text;

namespace FalconDataCartridge.Components
{
    public class CockpitView : INIComponent, IEquatable<CockpitView>
    {
        #region Properties
        /// <summary>
        /// When <see langword="true"/>, defaults to the Wide-Angle View
        /// </summary>
        public bool WideView { get => wideView; set => wideView = value; }        
        #endregion Properties // Checked

        #region Fields
        private bool wideView = false;
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

                // Cockpit View
                while (s != null)
                {
                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    s = s.ToUpper();
                    // Start Parse here -- StartsWith / IndexOf are ~2x faster than split or tokenizer
                    _ = int.TryParse(s.AsSpan(s.IndexOf('=') + 1), out int val);

                    if (s.StartsWith("WIDE"))
                    {
                        wideView = Convert.ToBoolean(val);
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
            sb.AppendLine("WideView=" + (wideView == true ? 1 : 0));
            return sb.ToString();
        }

        
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("****************** Cockpit View Configuration ******************");
            sb.AppendLine("     WideView: " + wideView);

            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(CockpitView? other)
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

            if (other is not CockpitView comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539;
                hash = hash * 5483 + wideView.GetHashCode();
                return hash;
            }
        }
        public static bool operator ==(CockpitView comparator1, CockpitView comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(CockpitView comparator1, CockpitView comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="HUD"/> object
        /// </summary>
        public CockpitView()
        {
            SectionFlag = "[Cockpit View]";
        }
        /// <summary>
        /// Initializes an instance of the <see cref="CockpitView"/> object using the values in <paramref name="hud"/>
        /// </summary>
        /// <param name="hud">The <see cref="hud"/></param> object to copy the values from
        public CockpitView(CockpitView view)
        {
            SectionFlag = "[Cockpit View]";
            wideView = view.wideView;
        }
        /// <summary>
        /// Initializes a new <see cref="CockpitView"/> object using the supplied values
        /// </summary>
        /// <param name="viewSetting">When <see langword="true"/>, sets the Wide View angle as the default.</param>
        public CockpitView(bool viewSetting)
        {
            SectionFlag = "[Cockpit View]";
            wideView = viewSetting;
        }
        /// <summary>
        /// Initializes an instance of the <see cref="HUD"/> object with the data in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization data</param>
        public CockpitView(string initializationData) : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }

        #endregion Constructors
    }
}
