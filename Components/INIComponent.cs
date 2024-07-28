namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Abstract Base Class for all Tpyes representing a Component within an INI File
    /// </summary>
    public abstract class INIComponent : IEquatable<INIComponent>
    {
        #region Properties
        /// <summary>
        /// <para>When <see langword="true"/>, this component will be included in the DTC output file</para>
        /// <para>By setting this to <see langword="false"/>, you can selectively print individual or groups of DTC Sections for output to a separate file</para>
        /// </summary>
        public bool IncludeInOutput { get => _IncludeInOutput; set => _IncludeInOutput = value; }              
        /// <summary>
        /// The Section Header used to identify this Component in a text file.
        /// </summary>
        public string SectionFlag { get => _sectionFlag; set => _sectionFlag = value; }
        #endregion Properties

        #region Fields

        protected string _sectionFlag = "";
        protected bool _IncludeInOutput = true;

        #endregion Fields

        #region Helper Methods

        /// <summary>
        /// Reads the Component Data from a <see cref="string"/> object.
        /// </summary>
        /// <param name="data">Text to parse for the Data.</param>
        /// <returns><see langword="true"/> if <paramref name="data"/> is successfully parsed, <see langword="false"/> otherwise.</returns>
        internal virtual bool Read(string data)
        {            
            throw new NotImplementedException("Read functionality for this component has not been fully implemented yet.");
        }

        /// <summary>
        /// Formats and arranges the Data within this <see cref="INIComponent"/> object for writing to a file on disk.
        /// </summary>
        /// <returns><see cref="string"/> that can be written to a file</returns>
        internal abstract string Write();

        #endregion Helper Methods

        #region Functional Methods
        /// <summary>
        /// <para>Formats the data contained within this <see cref="INIComponent"/> object into Readable Text.</para>
        /// <para>Readable Text does not always match the underlying file format and should not be used to save text based files such as .ini, .lst, or .txtpb files.</para>
        /// <para>Instead, use ToFileOutput() to format all text or binary data for writing to a file.</para>
        /// </summary>
        /// <returns>A formatted <see cref="string"/> with the Data contained within the <see cref="GameFile"/> object.</returns>
        public new abstract string ToString();
        
        #region Equality Functions
        public bool Equals(INIComponent? other)
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

            if (other is not INIComponent comparator)
                return false;
            else
                return Equals(comparator);
        }
        public abstract override int GetHashCode();
        public static bool operator ==(INIComponent comparator1, INIComponent comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(INIComponent comparator1, INIComponent comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

    }
}
