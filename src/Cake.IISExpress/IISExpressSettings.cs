namespace Cake.IISExpress
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IISExpressSettings
    {
        /// <summary>
        /// 
        /// </summary>
        protected IISExpressSettings()
        {
            EnableSystemTray = true;
            TraceLevel = TraceLevel.None;
        }

        /// <summary>
        ///     Enables or disables the system tray application. The default value is true.
        ///     Corresponds to /systray:[boolean] command line argument
        /// </summary>
        public bool EnableSystemTray { get; set; }

        /// <summary>
        ///     Corresponds to /trace:debug-trace-level
        ///     Valid values are info or i,warning or w,error or e.
        ///     Default is None (no /trace argument will be used)
        /// </summary>
        public TraceLevel TraceLevel { get; set; }
    }
}