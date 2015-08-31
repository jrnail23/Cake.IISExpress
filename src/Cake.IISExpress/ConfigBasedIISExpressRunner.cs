using System;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    /// </summary>
    public class ConfigBasedIISExpressRunner : IISExpressRunner<ConfigBasedIISExpressSettings>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigBasedIISExpressRunner" /> class.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="environment"></param>
        /// <param name="processRunner"></param>
        /// <param name="globber"></param>
        /// <param name="registry"></param>
        public ConfigBasedIISExpressRunner(IFileSystem fileSystem, ICakeEnvironment environment,
            IProcessRunner processRunner, IGlobber globber, IRegistry registry)
            : base(fileSystem, environment, processRunner, globber, registry)
        {
        }

        /// <summary>
        ///     Builds arguments specific to the implemented execution strategy
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected override ProcessArgumentBuilder BuildArguments(
            ConfigBasedIISExpressSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            var arguments = new ProcessArgumentBuilder();

            var hasSiteNameToLaunch = !string.IsNullOrWhiteSpace(settings.SiteNameToLaunch);

            if (hasSiteNameToLaunch)
            {
                arguments.Append("/site:'" + settings.SiteNameToLaunch + "'");
            }

            var hasSiteIdToLaunch = settings.SiteIdToLaunch.HasValue;

            if (hasSiteIdToLaunch)
            {
                arguments.Append("/siteid:" + settings.SiteIdToLaunch);
            }

            if (hasSiteIdToLaunch && hasSiteNameToLaunch)
            {
                throw new InvalidOperationException(
                    "SiteName and SiteId should not be used together.");
            }

            if (settings.ConfigFilePath != null)
            {
                if (!FileSystem.Exist(settings.ConfigFilePath))
                {
                    throw new CakeException(
                        "IIS Express configuration file '" + settings.ConfigFilePath +
                        "' does not exist.");
                }

                arguments.Append("/config:'" + settings.ConfigFilePath.MakeAbsolute(Environment) +
                                 "'");
            }

            if (!string.IsNullOrEmpty(settings.AppPoolToLaunch))
            {
                arguments.Append("/apppool:" + settings.AppPoolToLaunch);
            }

            return arguments;
        }
    }
}