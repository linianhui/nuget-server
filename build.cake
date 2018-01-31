#addin "Cake.IIS"
#addin "Cake.Hosts"
#addin "Cake.Powershell"

/// params
var target = Argument("target", "default");

var solution = "./nuget.server.sln";

var website = new {
    host = "nuget.server.local",
    path = "./src/webapp",
    appPoolName = "nuget.server.local"
};

/// clean task
Task("clean")
    .Does(() =>
{
	CleanDirectories("./src/**/bin");
});


/// restor nuget packages task
Task("restore")
    .Does(() =>
{
    NuGetRestore(solution);
});

/// build task
Task("build")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
{
    MSBuild(solution, new MSBuildSettings {
		Verbosity = Verbosity.Minimal
    });
});

/// deploy task
Task("deploy")
    .Does(() =>
{
    DeleteSite(website.host);

    CreateWebsite(new WebsiteSettings()
    {
        Name = website.host,
        Binding = IISBindings.Http.SetHostName(website.host)
                                  .SetIpAddress("*")
                                  .SetPort(80),
        ServerAutoStart = true,
        PhysicalDirectory = website.path,
        ApplicationPool = new ApplicationPoolSettings()
        {
            Name = website.appPoolName,
            IdentityType = IdentityType.LocalSystem,
            MaxProcesses = 1,
            ManagedRuntimeVersion = "v4.0"
        }
    });
    
    AddHostsRecord("127.0.0.1", website.host);
});

/// open browser task
Task("open-browser")
    .Does(() =>
{
    StartPowershellScript("Start-Process", args =>
    {
        args.Append("chrome.exe")
            .Append("'-incognito'")
            .Append(",'http://" + website.host + "/'");
    });
});


/// default task
Task("default")
.IsDependentOn("build")
.IsDependentOn("deploy")
.IsDependentOn("open-browser");

RunTarget(target);
