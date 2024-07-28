using FalconDataCartridge.Enums;
using System.Collections.ObjectModel;
using System.Text;

namespace FalconDataCartridge.Components
{
    /// <summary>
    /// Represents the FCC_AGB Section of a DTC File
    /// </summary>
    public class FCCAGB : INIComponent, IEquatable<FCCAGB>
    {
        #region Properties
        /// <summary>
        /// List of AGB Profile
        /// </summary>
        public Collection<AGB_Profile> Profiles {  get => profiles; set => profiles = value; }       
        #endregion Properties // Checked

        #region Fields
        private Collection<AGB_Profile> profiles = [];
        #endregion Fields

        #region Helper Methods           
        internal override bool Read(string data)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(data);

            try
            {
                // Check if the string is empty or does not contain the [FCC_AIM] flag           
                if (data.Length == 0 | !data.Contains(SectionFlag, StringComparison.CurrentCulture))
                    throw new InvalidDataException("The supplied string does not contain " + SectionFlag + " Data");

                // Find the [FCC_AIM] tag and read each line
                using StringReader sr = new(data[data.IndexOf(SectionFlag)..]);
                string? s = sr.ReadLine();
                                
                profiles.Clear();
                double c1ad1 = 0, c1ad2 = 0, c2ad = 0;
                int id = 1, c2ba = 0, spacing = 0, pulse = 0, angle = 0, submode = 0, fuze = 0;
                bool pair = false;

                while (s != null)
                {

                    s = sr.ReadLine();
                    if (s is null || s.Contains('['))
                        break;

                    string[] delims = ["_", "="];
                    s = s.Replace("Profile", "").Replace("Release", "");

                    string[] PGM = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    if (id != int.Parse(PGM[0]))
                    {
                        profiles.Add(new(
                        (FCR_Submode)submode, (FuzeMode)fuze, pair, spacing, pulse, angle, c1ad1, c1ad2, c2ad, c2ba, id));
                        id = int.Parse(PGM[0]);
                    }

                    switch (PGM[1], PGM[2])
                    {
                        case ("C1", "AD1"):
                            c1ad1 = double.Parse(PGM[3]);
                            continue;
                        case ("C1", "AD2"):
                            c1ad2 = double.Parse(PGM[3]);
                            continue;
                        case ("C2", "AD"):
                            c2ad = double.Parse(PGM[3]);
                            continue;
                        case ("C2", "BA"):
                            c2ba = int.Parse(PGM[3]);
                            continue;
                    }
                    if (PGM[1] == "Submode") submode = int.Parse(PGM[2]);
                    else if (PGM[1] == "Fuze") fuze = int.Parse(PGM[2]);
                    else if (PGM[1] == "SGL/PAIR") pair = Convert.ToBoolean(int.Parse(PGM[2]));
                    else if (PGM[1] == "Spacing") spacing = int.Parse(PGM[2]);
                    else if (PGM[1] == "Pulse") pulse = int.Parse(PGM[2]);
                    else if (PGM[1] == "Angle") angle = int.Parse(PGM[2]);

                }

                profiles.Add(new(
                        (FCR_Submode)submode, (FuzeMode)fuze, pair, spacing, pulse, angle, c1ad1, c1ad2, c2ad, c2ba, id));
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
        
        
        #endregion Helper Methods

        #region Functional Methods

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("************* FCC Air-to-Ground Bomb Configuration *************");
            foreach (AGB_Profile profile in profiles)
                sb.Append(profile.ToString());

            return sb.ToString();
        }

        internal override string Write()
        {
            if (!_IncludeInOutput)
                return "";
            StringBuilder sb = new(SectionFlag + Environment.NewLine);
            foreach (AGB_Profile profile in profiles)
                sb.Append(profile.Write());
            return sb.ToString();
        }

        #region Equality Functions
        public bool Equals(FCCAGB? other)
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

            if (other is not FCCAGB comparator)
                return false;
            else
                return Equals(comparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2539 + SectionFlag.GetHashCode();
                if (Profiles is not null)
                    if (Profiles.Count > 0)
                        foreach (AGB_Profile profile in profiles) hash ^= profile.GetHashCode();

                return hash;
            }
        }
        public static bool operator ==(FCCAGB comparator1, FCCAGB comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return Equals(comparator1, comparator2);

            return comparator1.Equals(comparator2);
        }
        public static bool operator !=(FCCAGB comparator1, FCCAGB comparator2)
        {
            if (comparator1 is null || comparator2 is null)
                return !Equals(comparator1, comparator2);

            return !comparator1.Equals(comparator2);
        }
        #endregion Equality Functions

        #endregion Functional Methods

        #region Constructors
        /// <summary>
        /// Initializes a default instance of the <see cref="FCC_AGB"/> object
        /// </summary>
        public FCCAGB()
        {
            SectionFlag = "[FCC_AGB]";
            for (int i = 0; i < 2; i++) profiles.Add(new());
        }
        /// <summary>
        /// Initializes an instance of the <see cref="FCC_AGB"/> object using the values contained in <paramref name="fcc"/>
        /// </summary>
        /// <param name="fcc">The <see cref="FCC_AGB"/> object with the values to copy</param>
        public FCCAGB(FCCAGB fcc)
        {
            SectionFlag = "[FCC_AGB]";
            foreach (AGB_Profile profile in fcc.profiles)
                profiles.Add(new AGB_Profile(profile));
        }
        /// <summary>
        /// Initializes a new <see cref="FCCAGB"/> object with the supplied Profiles
        /// </summary>
        /// <param name="profiles"><see cref="Collection{t}"/> of <see cref="AGB_Profile"/> objects used to initialize this <see cref="FCCAGB"/></param>
        public FCCAGB(ICollection<AGB_Profile> profiles)
        {
            SectionFlag = "[FCC_AGB]";
            this.profiles.Clear();
            foreach (AGB_Profile profile in profiles)
                this.profiles.Add(profile);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FCCAGM"/> object with the values supplied in <paramref name="initializationData"/>
        /// </summary>
        /// <param name="initializationData"><see cref="string"/> object with initialization values</param>
        public FCCAGB(string initializationData)
            : this()
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(initializationData);
            Read(initializationData);
        }
        #endregion Constructors

        #region Sub Classes
        /// <summary>
        /// A Pre-Configured Bombing Profile for the DTC AG Mode
        /// </summary>
        public class AGB_Profile
        {
            #region Properties
            /// <summary>
            /// Bombing Submode
            /// </summary>
            public FCR_Submode Submode { get; set; } = FCR_Submode.CCRP;
            /// <summary>
            /// Arming Fuze Mode
            /// </summary>
            public FuzeMode Fuze { get; set; } = FuzeMode.NOSE;
            /// <summary>
            /// When <see langword="true"/> releases weapons in Pairs, otherwise releases weapons in singles
            /// </summary>
            public bool PairRelease { get; set; } = false;
            /// <summary>
            /// Impact Spacing of multiple weapon releases
            /// </summary>
            public int Spacing { get; set; } = 210;
            /// <summary>
            /// Number of Release Pulses, aka Ripple Count
            /// </summary>
            public int ReleasePulses { get; set; } = 1;
            /// <summary>
            /// Weapon Release Angle
            /// </summary>
            public int ReleaseAngle { get; set; } = 45;
            /// <summary>
            /// Arming Delay 1
            /// </summary>
            public double Configuration_1_ArmingDelay1 { get; set; } = 400;
            /// <summary>
            /// Arming Delay 2
            /// </summary>
            public double Configuration_1_ArmingDelay2 { get; set; } = 600;
            /// <summary>
            /// Arming Delay
            /// </summary>
            public double Configuration_2_ArmingDelay { get; set; } = 150;
            /// <summary>
            /// Burst Altitude
            /// </summary>
            public int Configuration_2_BurstAltitude { get; set; } = 750;
            /// <summary>
            /// Profile Identifier
            /// </summary>
            public int ProfileID { get; set; } = 1;

            #endregion Properties // Checked

            #region Functional Methods
            /// <summary>
            /// Formats the Data of this Component into a <see cref="string"/> for output to a file
            /// </summary>
            /// <returns>Component Data formatted as a <see cref="string"/> object</returns>
            internal string Write()
            {
                StringBuilder sb = new();
                sb.AppendLine("Profile" + ProfileID + "_Submode=" + (int)Submode);
                sb.AppendLine("Profile" + ProfileID + "_Fuze=" + (int)Fuze);
                sb.AppendLine("Profile" + ProfileID + "_SGL/PAIR=" + (PairRelease ? 1 : 0));
                sb.AppendLine("Profile" + ProfileID + "_Release_Spacing=" + Spacing);
                sb.AppendLine("Profile" + ProfileID + "_Release_Pulse=" + ReleasePulses);
                sb.AppendLine("Profile" + ProfileID + "_Release_Angle=" + ReleaseAngle);
                sb.AppendLine("Profile" + ProfileID + "_C1_AD1=" + Configuration_1_ArmingDelay1.ToString("F6"));
                sb.AppendLine("Profile" + ProfileID + "_C1_AD2=" + Configuration_1_ArmingDelay2.ToString("F6"));
                sb.AppendLine("Profile" + ProfileID + "_C2_AD=" + Configuration_2_ArmingDelay.ToString("F6"));
                sb.AppendLine("Profile" + ProfileID + "_C2_BA=" + Configuration_2_BurstAltitude);

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
                sb.AppendLine("     AGB Profile" + ProfileID + ":" + Environment.NewLine);
                sb.AppendLine("        Profile: " + ProfileID);
                sb.AppendLine("        Submode: " + Submode);
                sb.AppendLine("        Fuze: " + Fuze);
                sb.AppendLine("        SGL/PAIR: " + (PairRelease ? 1 : 0));
                sb.AppendLine("        Release Spacing: " + Spacing);
                sb.AppendLine("        Release Pulse: " + ReleasePulses);
                sb.AppendLine("        Release Angle: " + ReleaseAngle);
                sb.AppendLine("        C1 AD1: " + Configuration_1_ArmingDelay1);
                sb.AppendLine("        C1 AD2: " + Configuration_1_ArmingDelay2);
                sb.AppendLine("        C2 AD: " + Configuration_2_ArmingDelay);
                sb.AppendLine("        C2 BA: " + Configuration_2_BurstAltitude);

                return sb.ToString();
            }

            #region Equality Functions
            public bool Equals(AGB_Profile? other)
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

                if (other is not AGB_Profile comparator)
                    return false;
                else
                    return Equals(comparator);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 2539;
                    hash = hash * 5483 + ProfileID.GetHashCode();
                    hash = hash * 5483 + Submode.GetHashCode();
                    hash = hash * 5483 + Fuze.GetHashCode();
                    hash = hash * 5483 + PairRelease.GetHashCode();
                    hash = hash * 5483 + Spacing.GetHashCode();
                    hash = hash * 5483 + ReleasePulses.GetHashCode();
                    hash = hash * 5483 + ReleaseAngle.GetHashCode();
                    hash = hash * 5483 + Configuration_1_ArmingDelay1.GetHashCode();
                    hash = hash * 5483 + Configuration_1_ArmingDelay2.GetHashCode();
                    hash = hash * 5483 + Configuration_2_ArmingDelay.GetHashCode();
                    hash = hash * 5483 + Configuration_2_BurstAltitude.GetHashCode();

                    return hash;
                }
            }
            public static bool operator ==(AGB_Profile comparator1, AGB_Profile comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return Equals(comparator1, comparator2);

                return comparator1.Equals(comparator2);
            }
            public static bool operator !=(AGB_Profile comparator1, AGB_Profile comparator2)
            {
                if (comparator1 is null || comparator2 is null)
                    return !Equals(comparator1, comparator2);

                return !comparator1.Equals(comparator2);
            }
            #endregion Equality Functions

            #endregion Funcitonal Methods

            #region Constructors
            /// <summary>
            /// Initializes a default instance of the <see cref="AGB_Profile"/> object
            /// </summary>
            public AGB_Profile() { }
            /// <summary>
            /// Initializes a new instance of the <see cref="AGB_Profile"/> object with the data in <paramref name="profile"/>
            /// </summary>
            /// <param name="profile">The <see cref="AGB_Profile"/> object with the values to copy</param>
            public AGB_Profile(AGB_Profile profile)
            : this()
            {
                Submode = profile.Submode;
                Fuze = profile.Fuze;
                PairRelease = profile.PairRelease;
                Spacing = profile.Spacing;
                ReleasePulses = profile.ReleasePulses;
                ReleaseAngle = profile.ReleaseAngle;
                Configuration_1_ArmingDelay1 = profile.Configuration_1_ArmingDelay1;
                Configuration_1_ArmingDelay2 = profile.Configuration_1_ArmingDelay2;
                Configuration_2_ArmingDelay = profile.Configuration_2_ArmingDelay;
                Configuration_2_BurstAltitude = profile.Configuration_2_BurstAltitude;
                ProfileID = profile.ProfileID;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="AGB_Profile"/> object with the supplied values
            /// </summary>
            /// <param name="submode">Selected Submode</param>
            /// <param name="fuze">Fuze Setting</param>
            /// <param name="pair">Releases in Pairs when <see langword="true"/>, otherwise in singles</param>
            /// <param name="spacing">Impact spacing </param>
            /// <param name="pulse">Number of pulses to execute with a Pickle Event (Ripple Count)</param>
            /// <param name="releaseAngle">Release Angle of the weapon</param>
            /// <param name="ca1_ad1">Arming Delay 1 for Configuration 1</param>
            /// <param name="ca1_ad2">Arming Delay 2 for Configuration 1</param>
            /// <param name="ca2_ad">Arming Delay for Configuration 2</param>
            /// <param name="ca2_ba">Burst Altitude for Burst Weapons</param>
            /// <param name="profileid">Identifies which Bombing Profile this <see cref="AGB_Profile> represents"/></param>
            public AGB_Profile(FCR_Submode submode, FuzeMode fuze, bool pair, int spacing, int pulse, int releaseAngle, double ca1_ad1, double ca1_ad2, double ca2_ad, int ca2_ba, int profileid)
            : this()
            {
                Submode = submode;
                Fuze = fuze;
                PairRelease = pair;
                Spacing = spacing;
                ReleasePulses = pulse;
                ReleaseAngle = releaseAngle;
                Configuration_1_ArmingDelay1 = ca1_ad1;
                Configuration_1_ArmingDelay2 = ca1_ad2;
                Configuration_2_ArmingDelay = ca2_ad;
                Configuration_2_BurstAltitude = ca2_ba;
                ProfileID = profileid;
            }

            #endregion Constructors

        }

        #endregion Sub Classes
    }
}
