// ARGUMENTS
var target = Argument("target", "Default");
var iisExpressConfigFile = File(Argument<string>("applicationHostConfigFile"));

// STATE
IProcess iisExpressProcess = null;

// TASKS
Task("Start-WebServer")
    .Does(
        () =>
        {
            iisExpressProcess = this.IISExpress(new ConfigBasedIISExpressSettings
            {
                ConfigFilePath = iisExpressConfigFile
            });
        });

Task("Hit-WebServer")
    .IsDependentOn("Start-WebServer")
    .Does(cc =>
    {
        foreach (var line in iisExpressProcess.GetStandardOutput())
        {
            if (line.StartsWith("Enter 'Q' to stop IIS Express"))
            {
                break;
            }
        }
        Verbose("About to download file");

        DownloadFile("http://localhost:8080/");
    });

Task("Default")
    .IsDependentOn("Hit-WebServer");

// CLEANUP
Teardown(() =>
{
    if (iisExpressProcess != null)
    {
        iisExpressProcess.Kill();
        iisExpressProcess.Dispose();
        Context.Log.Information("Disposed IIS Express process");
    }
});

RunTarget(target);
