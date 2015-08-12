using Cake.Core;
using Cake.Core.IO;

namespace Cake.IISExpress
{
    /// <summary>
    /// IIS Express runner, used when specifying the app path to run.
    /// </summary>
    public class AppPathBasedIISExpressRunner : IISExpressRunner<AppPathBasedIISExpressSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppPathBasedIISExpressRunner"/> class.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="environment"></param>
        /// <param name="processRunner"></param>
        /// <param name="globber"></param>
        /// <param name="registry"></param>
        public AppPathBasedIISExpressRunner(IFileSystem fileSystem, ICakeEnvironment environment,
            IProcessRunner processRunner, IGlobber globber, IRegistry registry)
            : base(fileSystem, environment, processRunner, globber, registry)
        {
        }

        /// <summary>
        /// Runs the process.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        /// <exception cref="Cake.Core.CakeException"></exception>
        public override IProcess RunProcess(AppPathBasedIISExpressSettings settings)
        {

            var appPath = settings.AppPath.IsRelative
                ? settings.AppPath.MakeAbsolute(Environment)
                : settings.AppPath;

            if (!FileSystem.Exist(appPath))
            {
                throw new CakeException(string.Format("AppPath '{0}' does not exist.",
                    appPath));
            }

            var arguments = new ProcessArgumentBuilder();

            arguments.Append(string.Format("/path:'{0}'", appPath));

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

            if (settings.TraceLevel != TraceLevel.None)
            {
                arguments.Append(string.Format("/trace:{0}",
                    settings.TraceLevel.ToString().ToLowerInvariant()));
            }

            if (!settings.EnableSystemTray)
            {
                arguments.Append("/systray:false");
            }

            return base.RunProcess(settings, arguments);
        }
    }
}