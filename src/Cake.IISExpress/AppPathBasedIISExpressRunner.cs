using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    ///     IIS Express runner, used when specifying the app path to run.
    /// </summary>
    public class AppPathBasedIISExpressRunner : IISExpressRunner<AppPathBasedIISExpressSettings>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AppPathBasedIISExpressRunner" /> class.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="environment"></param>
        /// <param name="processRunner"></param>
        /// <param name="globber"></param>
        /// <param name="registry"></param>
        /// <param name="log"></param>
        public AppPathBasedIISExpressRunner(IFileSystem fileSystem, ICakeEnvironment environment,
            IProcessRunner processRunner, IGlobber globber, IRegistry registry, ICakeLog log)
            : base(fileSystem, environment, processRunner, globber, registry, log)
        {
        }

        /// <summary>
        ///     Builds arguments specific to the implemented execution strategy
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected override ProcessArgumentBuilder BuildArguments(
            AppPathBasedIISExpressSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            var appPath = settings.AppPath.IsRelative
                ? settings.AppPath.MakeAbsolute(Environment)
                : settings.AppPath;

            if (!FileSystem.Exist(appPath))
            {
                throw new CakeException("AppPath '" + appPath + "' does not exist.");
            }

            var arguments = new ProcessArgumentBuilder();

            arguments.Append("/path:\"" + appPath + "\"");

            // don't add switch for the default value
            if (settings.PortNumber != 8080)
            {
                arguments.Append("/port:" + settings.PortNumber);
            }

            // only handle the non-default case
            if (settings.ClrVersion == ClrVersion.Version20)
            {
                arguments.Append("/clr:v2.0");
            }

            return arguments;
        }
    }
}