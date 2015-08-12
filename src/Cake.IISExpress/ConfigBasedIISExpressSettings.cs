using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigBasedIISExpressSettings : IISExpressSettings
    {
        /// <summary>
        ///     /config:config-file
        ///     The full path to the applicationhost.config file. The default value used (when not set)
        ///     is the IISExpress\config\applicationhost.config file that is located in the user's Documents folder.
        ///     This path may be relative to the working directory.
        /// </summary>
        public FilePath ConfigFilePath { get; set; }

        /// <summary>
        ///     (OPTIONAL)
        ///     /site:site-name
        ///     The name of the site to launch, as described in the applicationhost.config file.
        /// </summary>
        public string SiteNameToLaunch { get; set; }

        /// <summary>
        ///     (OPTIONAL)
        ///     /siteid:site-id
        ///     The ID of the site to launch, as described in the applicationhost.config file.
        /// </summary>
        public int? SiteIdToLaunch { get; set; }
    }
}