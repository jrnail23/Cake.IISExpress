using System;
using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    /// 
    /// </summary>
    public class AppPathBasedIISExpressSettings : IISExpressSettings
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appPath"></param>
        public AppPathBasedIISExpressSettings(DirectoryPath appPath)
        {
            if (appPath == null)
                throw new ArgumentNullException("appPath");
            AppPath = appPath;
            PortNumber = 8080;
            ClrVersion = ClrVersion.Version40;
        }

        /// <summary>
        ///     /path:app-path
        ///     The full physical path of the application to run.
        ///     This should be the folder that contains the application's web.config file.
        ///     This path may be relative to the working directory.
        /// </summary>
        public DirectoryPath AppPath { get; set; }

        /// <summary>
        ///     /port:port-number
        ///     The port to which the application will bind. The default value is 8080.
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        /// /clr:clr-version 
        /// The .NET Framework version (e.g. v2.0) to use to run the application. 
        /// The default value is v4.0.
        /// </summary>
        public ClrVersion ClrVersion { get; set; }
    }
}