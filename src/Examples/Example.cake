#addin "Cake.IISExpress" "\\localhost\NugetCache"
using Cake.IISExpress;

// ARGUMENTS
var target = Argument("target", "Default");
var iisExpressConfigFile = File(Argument<string>("applicationHostConfigFile"));
var siteName = Argument("siteName", "");

// STATE
IAdvProcess iisExpressProcess = null;

Task("Start-WebServer")
    .Does(ctx =>
    {
        var settings = new ConfigBasedIISExpressSettings
        {
            ConfigFilePath = iisExpressConfigFile,
            WaitForStartup = 1000
        };

        if (!string.IsNullOrEmpty(siteName))
        {
            settings.SiteNameToLaunch = siteName;
        }

        Verbose("ConfigBasedIISExpressSettings.ConfigFilePath: {0}",
            settings.ConfigFilePath);

        iisExpressProcess = StartIISExpress(settings);

        iisExpressProcess.Exited +=
            (sender, args) =>
            {
                Information("IIS Express exited with code: {0}", args.ExitCode);
            };

        iisExpressProcess.OutputDataReceived += (sender, args) =>
        {
            if (args.Output != null)
            {
                Information("IIS Express output: {0}", args.Output);
            }
        };

        iisExpressProcess.ErrorDataReceived +=
            (sender, args) => { Error("IIS Express error: {0}", args.Output); };
    });

Task("Hit-WebServer")
    .IsDependentOn("Start-WebServer")
    .Does(cc =>
    {
        Verbose("About to download file");

        DownloadFile("http://localhost:51111/");
    });

Task("Default")
    .IsDependentOn("Hit-WebServer");

Teardown(() =>
{
    if (iisExpressProcess == null)
    {
        return;
    }

    try
    {
        iisExpressProcess.Kill();
    }
    finally
    {
        iisExpressProcess.Dispose();
        Context.Log.Information("Disposed IIS Express process");
    }
});

RunTarget(target);
