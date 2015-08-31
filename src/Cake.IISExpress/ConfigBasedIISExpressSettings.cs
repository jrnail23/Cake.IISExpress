using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    /// </summary>
    public class ConfigBasedIISExpressSettings : IISExpressSettings
    {
        /// <summary>
        ///     /config:value
        ///     The full path to the applicationhost.config file. The default value used (when not set)
        ///     is the IISExpress\config\applicationhost.config file that is located in the user's Documents folder.
        ///     This path may be relative to the working directory.
        /// </summary>
        public FilePath ConfigFilePath { get; set; }

        /// <summary>
        ///     (OPTIONAL)
        ///     /site:value
        ///     The name of the site to launch, as described in the applicationhost.config file.
        /// </summary>
        public string SiteNameToLaunch { get; set; }

        /// <summary>
        ///     (OPTIONAL)
        ///     /siteid:value
        ///     The ID of the site to launch, as described in the applicationhost.config file.
        /// </summary>
        public int? SiteIdToLaunch { get; set; }

        /// <summary>
        ///     (OPTIONAL)
        ///     /apppool:value
        ///     Gets or sets the name of the application pool to launch -- this will launch all sites configured to use this app pool in the same process.
        /// </summary>
        /// <value>
        ///     The name of the application pool to launch.
        /// </value>
        public string AppPoolToLaunch { get; set; }
    }
}