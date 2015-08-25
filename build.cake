
            //////////////////////////////////////////////////////////////////////
            // ARGUMENTS
            //////////////////////////////////////////////////////////////////////

            var target = Argument("target", "Default");
            var configuration = Argument("configuration", "Release");
            var semVersionSuffix = Argument("semanticVersionSuffix", "");

            //////////////////////////////////////////////////////////////////////
            // PREPARATION
            //////////////////////////////////////////////////////////////////////

            // Get whether or not this is a local build.
            var local = BuildSystem.IsLocalBuild;
            var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
            var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;

            // Parse release notes.
            var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");

            // Get version.
            var buildNumber = AppVeyor.Environment.Build.Number;
            var version = releaseNotes.Version.ToString();

            if (!string.IsNullOrEmpty(semVersionSuffix))
            {
                semVersionSuffix = semVersionSuffix.StartsWith("-")
                    ? semVersionSuffix
                    : "-" + semVersionSuffix;
            }

            var semVersion = local
                ? version + semVersionSuffix
                : (version + string.Concat("-build-", buildNumber));

            // Define directories.
            var buildDir = Directory("./src/Cake.IISExpress/bin") + Directory(configuration);
            var buildResultDir = Directory("./build") + Directory("v" + semVersion);
            var testResultsDir = buildResultDir + Directory("test-results");
            var nugetRoot = buildResultDir + Directory("nuget");
            var binDir = buildResultDir + Directory("bin");

            ///////////////////////////////////////////////////////////////////////////////
            // SETUP / TEARDOWN
            ///////////////////////////////////////////////////////////////////////////////

            Setup(() => { Information("Building version {0} of Cake.IISExpress.", semVersion); });

            //////////////////////////////////////////////////////////////////////
            // TASKS
            //////////////////////////////////////////////////////////////////////

            Task("Clean")
                .Does(
                    () =>
                    {
                        CleanDirectories(new DirectoryPath[]
                        {buildResultDir, binDir, testResultsDir, nugetRoot});
                    });

            Task("Restore-NuGet-Packages")
                .IsDependentOn("Clean")
                .Does(() =>
                {
                    NuGetRestore("./src/Cake.IISExpress.sln",
                        new NuGetRestoreSettings
                        {
                            Source = new List<string>
                            {
                                "https://www.nuget.org/api/v2/",
                                "https://www.myget.org/F/roslyn-nightly/"
                            }
                        });
                });

            Task("Patch-Assembly-Info")
                .IsDependentOn("Restore-NuGet-Packages")
                .Does(() =>
                {
                    var file = "./src/SolutionInfo.cs";
                    CreateAssemblyInfo(file,
                        new AssemblyInfoSettings
                        {
                            Product = "Cake.IISExpress",
                            Version = version,
                            FileVersion = version,
                            InformationalVersion = semVersion,
                            Copyright =
                                "Copyright (c) James Nail and contributors " + DateTime.UtcNow.Year
                        });
                });

            Task("Build")
                .IsDependentOn("Patch-Assembly-Info")
                .Does(() =>
                {
                    MSBuild("./src/Cake.IISExpress.sln",
                        settings => settings.SetConfiguration(configuration)
                            .WithProperty("TreatWarningsAsErrors", "true")
                            //.UseToolVersion(MSBuildToolVersion.Default)
                            .SetVerbosity(Verbosity.Minimal)
                            .SetNodeReuse(false));
                });

            Task("Run-Unit-Tests")
                .IsDependentOn("Build")
                .Does(() =>
                {
                    XUnit("./src/**/bin/" + configuration + "/*.Tests.dll",
                        new XUnitSettings
                        {
                            OutputDirectory = testResultsDir
                        });
                });

            Task("Copy-Files")
                .IsDependentOn("Run-Unit-Tests")
                .Does(() =>
                {
                    CopyFileToDirectory(buildDir + File("Cake.IISExpress.dll"), binDir);
                    CopyFileToDirectory(buildDir + File("Cake.IISExpress.xml"), binDir);
                    CopyFiles(new FilePath[] {"LICENSE", "README.md", "ReleaseNotes.md"}, binDir);
                });

            Task("Zip-Files")
                .IsDependentOn("Copy-Files")
                .Does(() =>
                {
                    var packageFile = File("Cake.IISExpress-bin-v" + semVersion + ".zip");
                    var packagePath = buildResultDir + packageFile;

                    Zip(binDir, packagePath);
                });

            Task("Create-NuGet-Packages")
                .IsDependentOn("Copy-Files")
                .Does(() =>
                {
                    // Create Cake package.
                    NuGetPack("./nuspec/Cake.IISExpress.nuspec",
                        new NuGetPackSettings
                        {
                            Version = semVersion,
                            ReleaseNotes = releaseNotes.Notes.ToArray(),
                            BasePath = binDir,
                            OutputDirectory = nugetRoot,
                            Symbols = false,
                            NoPackageAnalysis = false
                        });
                });

            Task("Update-AppVeyor-Build-Number")
                .WithCriteria(() => isRunningOnAppVeyor)
                .Does(() => { AppVeyor.UpdateBuildVersion(semVersion); });

            Task("Upload-AppVeyor-Artifacts")
                .IsDependentOn("Package")
                .WithCriteria(() => isRunningOnAppVeyor)
                .Does(() =>
                {
                    var artifact = buildResultDir + File("Cake.IISExpress-bin-v" + semVersion + ".zip");
                    AppVeyor.UploadArtifact(artifact);
                });

            Task("Publish-Local")
                .WithCriteria(() => local)
                .Does(() =>
                {
                    // Get the path to the package.
                    var package = nugetRoot + File("Cake.IISExpress." + semVersion + ".nupkg");

                    // Push the package.
                    NuGetPush(package, new NuGetPushSettings
                    {
                        Source = @"\\localhost\NuGetCache",
                        ApiKey = "xxx"
                    });
                });

            Task("Publish-MyGet")
                .WithCriteria(() => !local)
                .WithCriteria(() => !isPullRequest)
                .Does(() =>
                {
                    // Resolve the API key.
                    var apiKey = EnvironmentVariable("MYGET_API_KEY");
                    if (string.IsNullOrEmpty(apiKey))
                    {
                        throw new InvalidOperationException("Could not resolve MyGet API key.");
                    }

                    // Get the path to the package.
                    var package = nugetRoot + File("Cake.IISExpress." + semVersion + ".nupkg");

                    // Push the package.
                    NuGetPush(package,
                        new NuGetPushSettings
                        {
                            Source = "https://www.myget.org/F/cake/api/v2/package",
                            ApiKey = apiKey
                        });
                });

            //////////////////////////////////////////////////////////////////////
            // TASK TARGETS
            //////////////////////////////////////////////////////////////////////

            Task("Package")
                .IsDependentOn("Zip-Files")
                .IsDependentOn("Create-NuGet-Packages");

            Task("Default")
                .IsDependentOn("Package").IsDependentOn("Publish-Local");

            Task("AppVeyor")
                .IsDependentOn("Update-AppVeyor-Build-Number")
                .IsDependentOn("Upload-AppVeyor-Artifacts")
                .IsDependentOn("Publish-MyGet");

            //////////////////////////////////////////////////////////////////////
            // EXECUTION
            //////////////////////////////////////////////////////////////////////

            RunTarget(target);


