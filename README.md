# FalconDataCartridge
Library to interact with the Falcon DTC Files.

NOTE: I use the same base class for Application files across several projects so I moved the AppFile Class into the Utilities Project, as well as Logging functionas. In order for this to compile, you will need to pull the Utilities project and add it as a reference to your project, or at least grab the dll from the /bin folder. When this is ready for release, I will make sure the Utilities dll is included in the output for the build, but for now it needs to be manually included because I'm still making some minor tweaks to it.

TODO: Need a complete list of all the possible Cockpit and Lighting configurations. Currently limited to those in the default DTC file.

